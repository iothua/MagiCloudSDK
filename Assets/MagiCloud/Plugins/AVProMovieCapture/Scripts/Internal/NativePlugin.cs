#if UNITY_5_4_OR_NEWER || (UNITY_5 && !UNITY_5_0 && !UNITY_5_1)
	#define AVPRO_MOVIECAPTURE_ISSUEPLUGINEVENT_52
#endif

using UnityEngine;
using System.Text;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	public enum StereoPacking
	{
		None,
		TopBottom,
		LeftRight,
	}

	public enum StopMode
	{
		None,
		FramesEncoded,
		SecondsEncoded,
		SecondsElapsed,
	}

	public enum ImageSequenceFormat
	{
		PNG,
	}

	public enum FileWriterType
	{
		AVI = 0,
		MediaFoundation = 1,
		PNG = 2,
	}

	public class NativePlugin
	{
		public enum PixelFormat
		{
			RGBA32,
			BGRA32,             // Note: This is the native format for Unity textures with red and blue swapped.
			YCbCr422_YUY2,
			YCbCr422_UYVY,
			YCbCr422_HDYC,
		}

		// Used by GL.IssuePluginEvent
		public const int PluginID = 0xFA30000;
		public enum PluginEvent
		{
			CaptureFrameBuffer = 0,
			FreeResources = 1,
		}

		public const string ScriptVersion = "3.7.0";
		public const string ExpectedPluginVersion = "3.6.10";

		public const int MaxRenderWidth = 16384;
		public const int MaxRenderHeight = 16384;

#if AVPRO_MOVIECAPTURE_ISSUEPLUGINEVENT_52
		[DllImport("AVProMovieCapture")]
		public static extern System.IntPtr GetRenderEventFunc();
		[DllImport("AVProMovieCapture")]
		public static extern System.IntPtr GetFreeResourcesEventFunc();
#endif

		//////////////////////////////////////////////////////////////////////////
		// Global Init/Deinit

		[DllImport("AVProMovieCapture")]
		public static extern bool Init();

		[DllImport("AVProMovieCapture")]
		public static extern void Deinit();

		public static string GetPluginVersionString()
		{
			return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(GetPluginVersion());
		}

		[DllImport("AVProMovieCapture")]
		public static extern bool IsTrialVersion();

		//////////////////////////////////////////////////////////////////////////
		// Video Codecs

		[DllImport("AVProMovieCapture")]
		public static extern int GetNumAVIVideoCodecs();

		[DllImport("AVProMovieCapture")]
		public static extern bool IsConfigureVideoCodecSupported(int index);

		[DllImport("AVProMovieCapture")]
		public static extern void ConfigureVideoCodec(int index);

		public static string GetAVIVideoCodecName(int index)
		{
			string result = "Invalid";
			StringBuilder nameBuffer = new StringBuilder(256);
			if (GetAVIVideoCodecName(index, nameBuffer, nameBuffer.Capacity))
			{
				result = nameBuffer.ToString();
			}
			return result;
		}


		//////////////////////////////////////////////////////////////////////////
		// Audio Codecs

		[DllImport("AVProMovieCapture")]
		public static extern int GetNumAVIAudioCodecs();

		[DllImport("AVProMovieCapture")]
		public static extern bool IsConfigureAudioCodecSupported(int index);

		[DllImport("AVProMovieCapture")]
		public static extern void ConfigureAudioCodec(int index);

		public static string GetAVIAudioCodecName(int index)
		{
			string result = "Invalid";
			StringBuilder nameBuffer = new StringBuilder(256);
			if (GetAVIAudioCodecName(index, nameBuffer, nameBuffer.Capacity))
			{
				result = nameBuffer.ToString();
			}
			return result;
		}

		//////////////////////////////////////////////////////////////////////////
		// Audio Devices

		[DllImport("AVProMovieCapture")]
		public static extern int GetNumAVIAudioInputDevices();

		public static string GetAVIAudioInputDeviceName(int index)
		{
			string result = "Invalid";
			StringBuilder nameBuffer = new StringBuilder(256);
			if (GetAVIAudioInputDeviceName(index, nameBuffer, nameBuffer.Capacity))
			{
				result = nameBuffer.ToString();
			}
			return result;
		}

		//////////////////////////////////////////////////////////////////////////
		// Create the recorder

		[DllImport("AVProMovieCapture")]
		public static extern int CreateRecorderVideo([MarshalAs(UnmanagedType.LPWStr)] string filename, uint width, uint height, int frameRate, int format,
												bool isTopDown, int videoCodecIndex, bool hasAudio, int audioSampleRate, int audioChannelCount, int audioInputDeviceIndex, int audioCodecIndex, 
												bool isRealTime, bool useMediaFoundation, bool supportAlpha, bool forceGpuFlush);

		[DllImport("AVProMovieCapture")]
		public static extern int CreateRecorderImages([MarshalAs(UnmanagedType.LPWStr)] string filename, uint width, uint height, int frameRate, int format,
												bool isTopDown, bool isRealTime, int imageFormatType, bool supportAlpha, bool forceGpuFlush, int startFrame);

		[DllImport("AVProMovieCapture")]
		public static extern int CreateRecorderPipe([MarshalAs(UnmanagedType.LPWStr)] string filename, uint width, uint height, int frameRate, int format, 
												bool isTopDown, bool supportAlpha, bool forceGpuFlush);

		//////////////////////////////////////////////////////////////////////////
		// Update recorder

		[DllImport("AVProMovieCapture")]
		public static extern bool Start(int handle);

		[DllImport("AVProMovieCapture")]
		public static extern bool IsNewFrameDue(int handle);

		[DllImport("AVProMovieCapture")]
		public static extern void EncodeFrame(int handle, System.IntPtr data);

		[DllImport("AVProMovieCapture")]
		public static extern void EncodeAudio(int handle, System.IntPtr data, uint length);

		[DllImport("AVProMovieCapture")]
		public static extern void EncodeFrameWithAudio(int handle, System.IntPtr videoData, System.IntPtr audioData, uint audioLength);

		[DllImport("AVProMovieCapture")]
		public static extern void Pause(int handle);

		[DllImport("AVProMovieCapture")]
		public static extern void Stop(int handle, bool skipPendingFrames);

		[DllImport("AVProMovieCapture")]
		public static extern void SetTexturePointer(int handle, System.IntPtr texture);

		//////////////////////////////////////////////////////////////////////////
		// Destroy recorder

		[DllImport("AVProMovieCapture")]
		public static extern void FreeRecorder(int handle);

		//////////////////////////////////////////////////////////////////////////
		// Debugging

		[DllImport("AVProMovieCapture")]
		public static extern uint GetNumDroppedFrames(int handle);

		[DllImport("AVProMovieCapture")]
		public static extern uint GetNumDroppedEncoderFrames(int handle);

		[DllImport("AVProMovieCapture")]
		public static extern uint GetNumEncodedFrames(int handle);

		[DllImport("AVProMovieCapture")]
		public static extern uint GetEncodedSeconds(int handle);

		//////////////////////////////////////////////////////////////////////////
		// Private internal functions

		[DllImport("AVProMovieCapture")]
		private static extern System.IntPtr GetPluginVersion();

		[DllImport("AVProMovieCapture")]
		private static extern bool GetAVIVideoCodecName(int index, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, int nameBufferLength);

		[DllImport("AVProMovieCapture")]
		private static extern bool GetAVIAudioCodecName(int index, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, int nameBufferLength);

		[DllImport("AVProMovieCapture")]
		private static extern bool GetAVIAudioInputDeviceName(int index, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, int nameBufferLength);
	}
}