using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	public class Utils
	{
		/// <summary>
		/// The "main" camera isn't necessarily the one the gets rendered last to screen,
		/// so we sort all cameras by depth and find the one with no render target
		/// </summary>
		public static Camera GetUltimateRenderCamera()
		{
			Camera result = Camera.main;

			{
				// Iterate all enabled cameras
				float highestDepth = float.MinValue;
				Camera[] enabledCameras = Camera.allCameras;
				for (int cameraIndex = 0; cameraIndex < enabledCameras.Length; cameraIndex++)
				{
					Camera camera = enabledCameras[cameraIndex];
					// Ignore null cameras
					if (camera != null)
					{
						// Ignore cameras that are hidden or have a targetTexture
						bool isHidden = (camera.hideFlags & HideFlags.HideInHierarchy) == HideFlags.HideInHierarchy;
						if (!isHidden && camera.targetTexture == null)
						{
							// Ignore cameras that render nothing
							if (camera.pixelRect.width > 0f && camera.pixelHeight > 0f)
							{
								// Keep the one with highest depth
								// TODO: handle the case where camera depths are equal - which one is first then?
								if (camera.depth >= highestDepth)
								{
									highestDepth = camera.depth;
									result = camera;
								}
							}
						}
					}
				}
			}

			return result;
		}

		public static bool HasContributingCameras(Camera parentCamera)
		{
			bool result = true;

			// If the camera doesn't clear the target completely then it may have other contributing cameras
			if (parentCamera.rect == new Rect(0f, 0f, 1f, 1f))
			{
				if (parentCamera.clearFlags == CameraClearFlags.Skybox ||
					parentCamera.clearFlags == CameraClearFlags.Color)
				{
					result = false;
				}
			}

			return result;
		}

		/// <summary>
		/// Returns a list of cameras sorted in render order from first to last that contribute to the rendering to parentCamera
		/// </summary>
		public static Camera[] FindContributingCameras(Camera parentCamera)
		{
			System.Collections.Generic.List<Camera> validCameras = new System.Collections.Generic.List<Camera>(8);
			{
				// Iterate all enabled/disabled cameras (they may become enabled later on)
				Camera[] allCameras = (Camera[])Resources.FindObjectsOfTypeAll(typeof(Camera));
				for (int cameraIndex = 0; cameraIndex < allCameras.Length; cameraIndex++)
				{
					Camera camera = allCameras[cameraIndex];
					// Ignore null cameras and camera that is the parent
					if (camera != null && camera != parentCamera)
					{
						// Only allow cameras with depth less or equal to parent camera
						if (camera.depth <= parentCamera.depth)
						{
							// Ignore cameras that are hidden or have a targetTexture that doesn't match the parent
							bool isHidden = (camera.hideFlags & HideFlags.HideInHierarchy) == HideFlags.HideInHierarchy;
							if (!isHidden && camera.targetTexture == parentCamera.targetTexture)
							{
								// Ignore cameras that render nothing
								if (camera.pixelRect.width > 0 && camera.pixelHeight > 0)
								{
									validCameras.Add(camera);
								}
							}
						}
					}
				}
			}

			if (validCameras.Count > 1)
			{
				// Sort by depth (render order)
				// TODO: handle the case where camera depths are equal - which one is first then?
				validCameras.Sort(delegate (Camera a, Camera b)
				{
					if (a != b)	// Pre .Net 4.6.2 Sort() can compare an elements with itself
					{
						if (a.depth < b.depth)
						{
							return -1;
						}
						else if (a.depth > b.depth)
						{
							return 1;
						}
						else if (a.depth == b.depth)
						{
							UnityEngine.Debug.LogWarning("[AVProMovieCapture] Cameras '" + a.name + "' and '" + b.name + "' have the same depth value - unable to determine render order: " + a.depth);
						}
					}
					return 0;
				});

				// Starting from the last camera to render, find the first one that clears the screen completely
				for (int i = (validCameras.Count - 1); i >= 0; i--)
				{
					if (validCameras[i].rect == new Rect(0f, 0f, 1f, 1f))
					{
						if (validCameras[i].clearFlags == CameraClearFlags.Skybox ||
							validCameras[i].clearFlags == CameraClearFlags.Color)
						{
							// Remove all cameras before this
							validCameras.RemoveRange(0, i);
							break;
						}
					}
				}
			}

			return validCameras.ToArray();
		}

		public static bool ShowInExplorer(string itemPath)
		{
			bool result = false;

#if !UNITY_WEBPLAYER
			itemPath = Path.GetFullPath(itemPath.Replace(@"/", @"\"));   // explorer doesn't like front slashes
			if (File.Exists(itemPath))
			{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				Process.Start("explorer.exe", "/select," + itemPath);
#endif
				result = true;
			}
			else if (Directory.Exists(itemPath))
			{
				// NOTE: We use OpenURL() instead of the explorer process so that it opens explorer inside the folder
				UnityEngine.Application.OpenURL(itemPath);
				result = true;
			}
			
#endif

			return result;
		}

		public static bool OpenInDefaultApp(string itemPath)
		{
			bool result = false;

			itemPath = Path.GetFullPath(itemPath.Replace(@"/", @"\"));
			if (File.Exists(itemPath))
			{
				UnityEngine.Application.OpenURL(itemPath);
				result = true;
			}
			else if (Directory.Exists(itemPath))
			{
				UnityEngine.Application.OpenURL(itemPath);
				result = true;
			}

			return result;
		}

		public static long GetFileSize(string filename)
		{
#if UNITY_WEBPLAYER
			return 0;
#else
			System.IO.FileInfo fi = new System.IO.FileInfo(filename);
			return fi.Length;
#endif
		}


#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
														out ulong lpFreeBytesAvailable,
														out ulong lpTotalNumberOfBytes,
														out ulong lpTotalNumberOfFreeBytes);

		public static bool DriveFreeBytes(string folderName, out ulong freespace)
		{
			freespace = 0;
			if (string.IsNullOrEmpty(folderName))
			{
				throw new System.ArgumentNullException("folderName");
			}

			if (!folderName.EndsWith("\\"))
			{
				folderName += '\\';
			}

			ulong free = 0, dummy1 = 0, dummy2 = 0;

			if (GetDiskFreeSpaceEx(folderName, out free, out dummy1, out dummy2))
			{
				freespace = free;
				return true;
			}
			else
			{
				return false;
			}
		}
#endif
	}
}