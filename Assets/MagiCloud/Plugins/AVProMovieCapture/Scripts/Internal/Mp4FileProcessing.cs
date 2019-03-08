#if UNITY_5_4_OR_NEWER || (UNITY_5 && !UNITY_5_0)
	#define RH_UNITY_FEATURE_DEBUG_ASSERT
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	// Reference: https://wiki.multimedia.cx/index.php/QuickTime_container
	// Reference: https://github.com/danielgtaylor/qtfaststart/blob/master/qtfaststart/processor.py
	// Reference: https://developer.apple.com/library/content/documentation/QuickTime/QTFF/QTFFPreface/qtffPreface.html
	public class MP4FileProcessing
	{
		private const int ChunkHeaderSize = 8;
		private const int CopyBufferSize = 4096*16;

		//private static uint Atom_ftyp = ChunkId("ftyp");		// file type
		private static uint Atom_moov = ChunkId("moov");		// movie header
		private static uint Atom_mdat  = ChunkId("mdat");		// movie data
		private static uint Atom_cmov  = ChunkId("cmov");		// compressed movie data
		private static uint Atom_trak  = ChunkId("trak");		// track header
		private static uint Atom_mdia  = ChunkId("mdia");		// media
		private static uint Atom_minf  = ChunkId("minf");		// media information
		private static uint Atom_stbl  = ChunkId("stbl");		// sample table
		private static uint Atom_stco  = ChunkId("stco");		// sample table chunk offsets (32-bit)
		private static uint Atom_co64  = ChunkId("co64");		// sample table chunk offsets (64-bit)
		
		private class Chunk
		{
			public uint id;
			public long size;			// includes the size of the chunk header, so next chunk is at size+offset
			public long offset;
		};

		private BinaryReader _reader;
		private Stream _writeFile;

		public static bool ApplyFastStart(string filePath, bool keepBackup)
		{
			if (!File.Exists(filePath))
			{
				Debug.LogError("File not found: " + filePath);
				return false;
			}
			string tempPath = filePath + "-" + System.Guid.NewGuid() + ".temp";
			
			bool result = ApplyFastStart(filePath, tempPath);
			if (result)
			{
				string backupPath = filePath + "-" + System.Guid.NewGuid() + ".backup";
				File.Move(filePath, backupPath);
				File.Move(tempPath, filePath);
				if (!keepBackup)
				{
					File.Delete(backupPath);
				}
			}

			if (File.Exists(tempPath))
			{
				File.Delete(tempPath);
			}

			return result;
		}

		public static bool ApplyFastStart(string srcPath, string dstPath)
		{
			if (!File.Exists(srcPath))
			{
				Debug.LogError("File not found: " + srcPath);
				return false;
			}
			
			using (Stream srcStream = new FileStream(srcPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (Stream dstStream = new FileStream(dstPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
				{
					MP4FileProcessing mp4 = new MP4FileProcessing();
					bool result = mp4.Open(srcStream, dstStream);
					mp4.Close();				
					return result;
				}
			}
		}

		public bool Open(Stream srcStream, Stream dstStream)
		{
			Close();

			_reader = new BinaryReader(srcStream);

			List<Chunk> rootChunks = ReadChildChunks(null);
		
			Chunk chunk_moov = GetFirstChunkOfType(Atom_moov, rootChunks);
			Chunk chunk_mdat = GetFirstChunkOfType(Atom_mdat, rootChunks);

			bool canReorder = (chunk_moov != null && chunk_mdat != null);
			bool isReorderNeeded = false;
			if (canReorder)
			{
				isReorderNeeded = (chunk_moov.offset > chunk_mdat.offset);

				if (isReorderNeeded)
				{
					if (ChunkContainsChildChunkWithId(chunk_moov, Atom_cmov))
					{
						Debug.LogError("moov chunk is compressed - unsupported");
						canReorder = false;
					}
				}
				else
				{
					Debug.Log("No reordering needed");
				}			
			}
			else
			{
				Debug.LogWarning("no chunk tags found - incorrect file format?");
			}

			// Reorder so that "moov" chunk is before any "mdat" chunks
			if (canReorder && isReorderNeeded)
			{
#if RH_UNITY_FEATURE_DEBUG_ASSERT
				Debug.Assert(chunk_moov.offset > chunk_mdat.offset);
#endif
				ulong bytesOffset = (ulong)(chunk_moov.size);

				//Debug.Log("offseting by: " + bytesOffset);

				_writeFile = dstStream;

				foreach (Chunk chunk in rootChunks)
				{
					if (chunk == chunk_mdat)
					{
						WriteChunk_moov(chunk_moov, bytesOffset);
						WriteChunk(chunk);
					}
					else if (chunk != chunk_moov)
					{
						WriteChunk(chunk);
					}
				}

				return true;
			}	

			return false;
		}

		public void Close()
		{
			if (_reader != null)
			{
				_reader.Close();
				_reader = null;
			}	
		}

		private static Chunk GetFirstChunkOfType(uint id, List<Chunk> chunks)
		{
			Chunk result = null;
			foreach (Chunk chunk in chunks)
			{
				if (chunk.id == id)
				{
					result = chunk;
					break;
				}
			}
			return result;
		}

		private List<Chunk> ReadChildChunks(Chunk parentChunk)
		{
			// Offset to start of parent chunk
			{
				long fileOffset = 0;
				if (parentChunk != null)
				{
					fileOffset = parentChunk.offset + ChunkHeaderSize;
				}
				_reader.BaseStream.Seek(fileOffset, SeekOrigin.Begin);
			}

			long chunkEnd = _reader.BaseStream.Length;
			if (parentChunk != null)
			{
				chunkEnd = parentChunk.offset + parentChunk.size;
			}

			List<Chunk> result = new List<Chunk>();
			if (_reader.BaseStream.Position < chunkEnd)
			{
				Chunk chunk = ReadChunkHeader();
				while (chunk != null && _reader.BaseStream.Position < chunkEnd)
				{
					result.Add(chunk);
					_reader.BaseStream.Seek(chunk.offset + chunk.size, SeekOrigin.Begin);
					chunk = ReadChunkHeader();
				}
			}
			return result;
		}

		private Chunk ReadChunkHeader()
		{
			Chunk chunk = null;

			if ((_reader.BaseStream.Length - _reader.BaseStream.Position) >= ChunkHeaderSize)
			{
				chunk = new Chunk();
				chunk.offset = _reader.BaseStream.Position;
				chunk.size = ReadUInt32();
				chunk.id = _reader.ReadUInt32();
				
				if (chunk.size == 1)
				{
					// NOTE: One indicates we need to read the extended 64-bit size
					chunk.size = (long)ReadUInt64();
				}
				if (chunk.size == 0)
				{
					// NOTE: Zero indicates that this is the last chunk, so the size is the remainder of the file			
					chunk.size = _reader.BaseStream.Length - chunk.offset;
				}
			}

			return chunk;
		}

		private bool ChunkContainsChildChunkWithId(Chunk chunk, uint id)
		{
			bool result = false;
			long endChunkPos = chunk.size + chunk.offset;
			_reader.BaseStream.Seek(chunk.offset, SeekOrigin.Begin);
			Chunk childChunk = ReadChunkHeader();
			while (childChunk != null && _reader.BaseStream.Position < endChunkPos)
			{
				if (childChunk.id == id)
				{
					result = true;
					break;
				}

				_reader.BaseStream.Seek(childChunk.offset + childChunk.size, SeekOrigin.Begin);
				childChunk = ReadChunkHeader();			
			}
			return result;
		}

		private void WriteChunk(Chunk chunk)
		{
			_reader.BaseStream.Seek(chunk.offset, SeekOrigin.Begin);
			CopyBytes(chunk.size);
		}

		private void WriteChunkHeader(Chunk chunk)
		{
			_reader.BaseStream.Seek(chunk.offset, SeekOrigin.Begin);
			// TODO: potential bug as ChunkHeaderSize could actually by 16 instead of 8 in cases where extended size is used
			CopyBytes(ChunkHeaderSize);
		}	

		private void CopyBytes(long numBytes)
		{
			byte[] buffer = new byte[CopyBufferSize];
			long remaining = numBytes;
			Stream readStream = _reader.BaseStream;
			while (remaining > 0)
			{
				int byteCount = buffer.Length;
				if (remaining < buffer.Length)
				{
					byteCount = (int)remaining;
				}
				readStream.Read(buffer, 0, byteCount);
				_writeFile.Write(buffer, 0, byteCount);
				remaining -= byteCount;
			}
		}	

		private void WriteChunk_moov(Chunk parentChunk, ulong byteOffset)
		{
			// Hierarchy of atoms to apply offsets to moov > trak > mdia > minf > stbl > "co64, stco"
			WriteChunkHeader(parentChunk);

			List<Chunk> children = ReadChildChunks(parentChunk);

			// TODO: potential bug as ChunkHeaderSize could actually by 16 instead of 8 in cases where extended size is used
			_reader.BaseStream.Seek(parentChunk.offset + ChunkHeaderSize, SeekOrigin.Begin);

			foreach (Chunk chunk in children)
			{
				if (chunk.id == Atom_stco)
				{
					WriteChunkHeader(chunk);
					CopyBytes(4);

					// Apply offsets
					uint chunkCount = ReadUInt32();
					WriteUInt32(chunkCount);
					for (int i = 0; i < chunkCount; i++)
					{
						// TODO: potential bug here if new offset is greater than 32-bit..
						uint offset = ReadUInt32();
						offset += (uint)byteOffset;
						WriteUInt32(offset);
					}			
				}
				else if (chunk.id == Atom_co64)
				{
					WriteChunkHeader(chunk);
					CopyBytes(4);

					// Apply offsets
					uint chunkCount = ReadUInt32();
					WriteUInt32(chunkCount);
					for (int i = 0; i < chunkCount; i++)
					{
						// TODO: potential bug here if new offset is greater than 64-bit..
						ulong offset = ReadUInt64();
						offset += byteOffset;
						WriteUInt64(offset);
					}
				}
				else if (chunk.id == Atom_trak ||
						chunk.id == Atom_mdia ||
						chunk.id == Atom_minf ||
						chunk.id == Atom_stbl)
				{
					// Go into these chunks searching for the offset chunks
					WriteChunk_moov(chunk, byteOffset);
				}
				else
				{
					// We don't care about this chunk so just copy it
					WriteChunk(chunk);
				}
			}
		}

		private UInt32 ReadUInt32()
		{
			byte[] data = _reader.ReadBytes(4);
			Array.Reverse(data);
			return BitConverter.ToUInt32(data, 0);
		}	

		private UInt64 ReadUInt64()
		{
			byte[] data = _reader.ReadBytes(8);
			Array.Reverse(data);
			return BitConverter.ToUInt64(data, 0);
		}

		private void WriteUInt32(UInt32 value, bool isBigEndian = true)
		{
			byte[] data = BitConverter.GetBytes(value);
			if (isBigEndian)
			{
				Array.Reverse(data);
			}
			_writeFile.Write(data, 0, data.Length);
		}

		private void WriteUInt64(UInt64 value)
		{
			byte[] data = BitConverter.GetBytes(value);
			Array.Reverse(data);
			_writeFile.Write(data, 0, data.Length);
		}

		private static string ChunkType(UInt32 id)
		{
			char a = (char)((id >> 0) & 255);
			char b = (char)((id >> 8) & 255);
			char c = (char)((id >> 16) & 255);
			char d = (char)((id >> 24) & 255);
			return string.Format("{0}{1}{2}{3}", a, b, c, d);
		}

		private static uint ChunkId(string id)
		{
			uint a = id[3];
			uint b = id[2];
			uint c = id[1];
			uint d = id[0];
			return (a << 24) | (b << 16) | (c << 8) | d;
		}
	}
}

#if false
public class Mp4FastStartTest : MonoBehaviour 
{
	public string _path = "R:/Products/Unity/AVProVideo/Media/BigBuckBunny_2160p24_h264_2Mbps.mp4";

	void Start () 
	{
		DateTime time = DateTime.Now;
		if (MP4FileProcessing.ApplyFastStart(_path, true))
		{
			DateTime time2 = DateTime.Now;
			Debug.Log("success!");
			Debug.Log("Took: " + (time2 - time).TotalMilliseconds + "ms");
		}
		else
		{
			Debug.LogWarning("Did not modify file");
		}
	}	
}
#endif