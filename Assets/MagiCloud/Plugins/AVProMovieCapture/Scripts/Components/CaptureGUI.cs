using UnityEngine;
using System.Text;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	/// <summary>
	/// Uses IMGUI to render a GUI to control video capture.  This is mainly used for the demos.
	/// </summary>
	[AddComponentMenu("AVPro Movie Capture/Capture GUI", 300)]
	public class CaptureGUI : MonoBehaviour
	{
		public CaptureBase _movieCapture;
		public bool _showUI = true;
		public bool _whenRecordingAutoHideUI = true;
		public GUISkin _guiSkin;

		// GUI
		private int _shownSection = -1;
		private string[] _videoCodecNames = new string[0];
		private string[] _audioCodecNames = new string[0];
		private bool[] _videoCodecConfigurable;
		private bool[] _audioCodecConfigurable;
		private string[] _audioDeviceNames = new string[0];
		private string[] _downScales = new string[0];
		private string[] _frameRates = new string[0];
		private string[] _outputType = new string[0];
		private int _downScaleIndex;
		private int _frameRateIndex;
		private Vector2 _videoPos = Vector2.zero;
		private Vector2 _audioPos = Vector2.zero;
		private Vector2 _audioCodecPos = Vector2.zero;

		// Status
		private long _lastFileSize;
		private uint _lastEncodedMinutes;
		private uint _lastEncodedSeconds;
		private uint _lastEncodedFrame;

		private void Start()
		{
			if (_movieCapture != null)
			{
				CreateGUI();
			}
		}

		private void CreateGUI()
		{
			_outputType = new string[3];
			_outputType[0] = "Video File";
			_outputType[1] = "Image Sequence";
			_outputType[2] = "Named Pipe";
			_downScales = new string[6];
			_downScales[0] = "Original";
			_downScales[1] = "Half";
			_downScales[2] = "Quarter";
			_downScales[3] = "Eighth";
			_downScales[4] = "Sixteenth";
			_downScales[5] = "Custom";
			switch (_movieCapture._downScale)
			{
				default:
				case CaptureBase.DownScale.Original:
					_downScaleIndex = 0;
					break;
				case CaptureBase.DownScale.Half:
					_downScaleIndex = 1;
					break;
				case CaptureBase.DownScale.Quarter:
					_downScaleIndex = 2;
					break;
				case CaptureBase.DownScale.Eighth:
					_downScaleIndex = 3;
					break;
				case CaptureBase.DownScale.Sixteenth:
					_downScaleIndex = 4;
					break;
				case CaptureBase.DownScale.Custom:
					_downScaleIndex = 5;
					break;
			}

			_frameRates = new string[11];
			_frameRates[0] = "1";
			_frameRates[1] = "10";
			_frameRates[2] = "15";
			_frameRates[3] = "24";
			_frameRates[4] = "25";
			_frameRates[5] = "30";
			_frameRates[6] = "50";
			_frameRates[7] = "60";
			_frameRates[8] = "75";
			_frameRates[9] = "90";
			_frameRates[10] = "120";
			switch (_movieCapture._frameRate)
			{
				default:
				case CaptureBase.FrameRate.One:
					_frameRateIndex = 0;
					break;
				case CaptureBase.FrameRate.Ten:
					_frameRateIndex = 1;
					break;
				case CaptureBase.FrameRate.Fifteen:
					_frameRateIndex = 2;
					break;
				case CaptureBase.FrameRate.TwentyFour:
					_frameRateIndex = 3;
					break;
				case CaptureBase.FrameRate.TwentyFive:
					_frameRateIndex = 4;
					break;
				case CaptureBase.FrameRate.Thirty:
					_frameRateIndex = 5;
					break;
				case CaptureBase.FrameRate.Fifty:
					_frameRateIndex = 6;
					break;
				case CaptureBase.FrameRate.Sixty:
					_frameRateIndex = 7;
					break;
				case CaptureBase.FrameRate.SeventyFive:
					_frameRateIndex = 8;
					break;
				case CaptureBase.FrameRate.Ninety:
					_frameRateIndex = 9;
					break;
				case CaptureBase.FrameRate.OneTwenty:
					_frameRateIndex = 10;
					break;
			}

			int numVideoCodecs = NativePlugin.GetNumAVIVideoCodecs();
			if (numVideoCodecs > 0)
			{
				_videoCodecNames = new string[numVideoCodecs + 2];
				_videoCodecNames[0] = "Uncompressed";
				_videoCodecNames[1] = "Media Foundation H.264(MP4)";
				_videoCodecConfigurable = new bool[numVideoCodecs];
				for (int i = 0; i < numVideoCodecs; i++)
				{
					_videoCodecNames[i + 2] = NativePlugin.GetAVIVideoCodecName(i);
					_videoCodecConfigurable[i] = NativePlugin.IsConfigureVideoCodecSupported(i);
				}
			}

			int numAudioDevices = NativePlugin.GetNumAVIAudioInputDevices();
			if (numAudioDevices > 0)
			{
				_audioDeviceNames = new string[numAudioDevices + 1];
				_audioDeviceNames[0] = "Unity";
				for (int i = 0; i < numAudioDevices; i++)
				{
					_audioDeviceNames[i + 1] = NativePlugin.GetAVIAudioInputDeviceName(i);
				}
			}

			int numAudioCodecs = NativePlugin.GetNumAVIAudioCodecs();
			if (numAudioCodecs > 0)
			{
				_audioCodecNames = new string[numAudioCodecs + 1];
				_audioCodecNames[0] = "Uncompressed";
				_audioCodecConfigurable = new bool[numAudioCodecs];
				for (int i = 0; i < numAudioCodecs; i++)
				{
					_audioCodecNames[i + 1] = NativePlugin.GetAVIAudioCodecName(i);
					_audioCodecConfigurable[i] = NativePlugin.IsConfigureAudioCodecSupported(i);
				}
			}

			_movieCapture.SelectCodec(false);
			_movieCapture.SelectAudioCodec(false);
			_movieCapture.SelectAudioDevice(false);
		}

		private void OnGUI()
		{
			GUI.skin = _guiSkin;
			GUI.depth = -10;
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1920f * 1.5f, Screen.height / 1080f * 1.5f, 1f));

			if (_showUI)
			{
				GUILayout.Window(4, new Rect(0f, 0f, 450f, 256f), MyWindow, "AVPro Movie Capture UI");
			}
		}

		private void MyWindow(int id)
		{
			if (_movieCapture == null)
			{
				GUILayout.Label("CaptureGUI - No CaptureFrom component set");
				return;
			}

			if (_movieCapture.IsCapturing())
			{
				GUI_RecordingStatus();
				return;
			}

			GUILayout.BeginVertical();

			if (_movieCapture != null)
			{
				GUILayout.Label("Resolution:");
				GUILayout.BeginHorizontal();
				_downScaleIndex = GUILayout.SelectionGrid(_downScaleIndex, _downScales, _downScales.Length);
				switch (_downScaleIndex)
				{
					case 0:
						_movieCapture._downScale = CaptureBase.DownScale.Original;
						break;
					case 1:
						_movieCapture._downScale = CaptureBase.DownScale.Half;
						break;
					case 2:
						_movieCapture._downScale = CaptureBase.DownScale.Quarter;
						break;
					case 3:
						_movieCapture._downScale = CaptureBase.DownScale.Eighth;
						break;
					case 4:
						_movieCapture._downScale = CaptureBase.DownScale.Sixteenth;
						break;
					case 5:
						_movieCapture._downScale = CaptureBase.DownScale.Custom;
						break;
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUILayout.Width(256));
				if (_movieCapture._downScale == CaptureBase.DownScale.Custom)
				{
					string maxWidthString = GUILayout.TextField(Mathf.FloorToInt(_movieCapture._maxVideoSize.x).ToString(), 4);
					int maxWidth = 0;
					if (int.TryParse(maxWidthString, out maxWidth))
					{
						_movieCapture._maxVideoSize.x = Mathf.Clamp(maxWidth, 0, 16384);
					}

					GUILayout.Label("x", GUILayout.Width(20));

					string maxHeightString = GUILayout.TextField(Mathf.FloorToInt(_movieCapture._maxVideoSize.y).ToString(), 4);
					int maxHeight = 0;
					if (int.TryParse(maxHeightString, out maxHeight))
					{
						_movieCapture._maxVideoSize.y = Mathf.Clamp(maxHeight, 0, 16384);
					}
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Frame Rate:");
				_frameRateIndex = GUILayout.SelectionGrid(_frameRateIndex, _frameRates, _frameRates.Length);
				switch (_frameRateIndex)
				{
					case 0:
						_movieCapture._frameRate = CaptureBase.FrameRate.One;
						break;
					case 1:
						_movieCapture._frameRate = CaptureBase.FrameRate.Ten;
						break;
					case 2:
						_movieCapture._frameRate = CaptureBase.FrameRate.Fifteen;
						break;
					case 3:
						_movieCapture._frameRate = CaptureBase.FrameRate.TwentyFour;
						break;
					case 4:
						_movieCapture._frameRate = CaptureBase.FrameRate.TwentyFive;
						break;
					case 5:
						_movieCapture._frameRate = CaptureBase.FrameRate.Thirty;
						break;
					case 6:
						_movieCapture._frameRate = CaptureBase.FrameRate.Fifty;
						break;
					case 7:
						_movieCapture._frameRate = CaptureBase.FrameRate.Sixty;
						break;
					case 8:
						_movieCapture._frameRate = CaptureBase.FrameRate.SeventyFive;
						break;
					case 9:
						_movieCapture._frameRate = CaptureBase.FrameRate.Ninety;
						break;
					case 10:
						_movieCapture._frameRate = CaptureBase.FrameRate.OneTwenty;
						break;
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(16f);

				GUILayout.BeginHorizontal();
				GUILayout.Label("Output:", GUILayout.ExpandWidth(false));
				_movieCapture._outputType = (CaptureBase.OutputType)GUILayout.SelectionGrid((int)_movieCapture._outputType, _outputType, _outputType.Length);
				GUILayout.EndHorizontal();

				GUILayout.Space(16f);

				_movieCapture._isRealTime = GUILayout.Toggle(_movieCapture._isRealTime, "RealTime");

				GUILayout.Space(16f);


				if (_movieCapture._outputType == CaptureBase.OutputType.VideoFile)
				{
					// Video Codec
					GUILayout.BeginHorizontal();
					if (_shownSection != 0)
					{
						if (GUILayout.Button("+", GUILayout.Width(24)))
						{
							_shownSection = 0;
						}
					}
					else
					{
						if (GUILayout.Button("-", GUILayout.Width(24)))
						{
							_shownSection = -1;
						}
					}
					GUILayout.Label("Using Video Codec: " + _movieCapture._codecName);
					if (_movieCapture._codecIndex >= 0 && _videoCodecConfigurable[_movieCapture._codecIndex])
					{
						GUILayout.Space(16f);
						if (GUILayout.Button("Configure Codec"))
						{
							NativePlugin.ConfigureVideoCodec(_movieCapture._codecIndex);
						}
					}
					GUILayout.EndHorizontal();

					if (_videoCodecNames != null && _shownSection == 0)
					{
						GUILayout.Label("Select Video Codec:");
						_videoPos = GUILayout.BeginScrollView(_videoPos, GUILayout.Height(100));
						int newCodecIndex = GUILayout.SelectionGrid(-1, _videoCodecNames, 1) - 2;
						GUILayout.EndScrollView();

						if (newCodecIndex >= -2)
						{
							_movieCapture._codecIndex = newCodecIndex;
							if (_movieCapture._codecIndex >= 0)
							{
								_movieCapture._codecName = _videoCodecNames[_movieCapture._codecIndex + 2];
								_movieCapture._useMediaFoundationH264 = false;
							}
							else
							{
								if (_movieCapture._codecIndex == -2)
								{
									_movieCapture._codecName = "Uncompressed";
									_movieCapture._useMediaFoundationH264 = false;
								}
								else
								{
									if (_movieCapture._codecIndex == -1)
									{
										_movieCapture._codecName = "Media Foundation H.264(MP4)";
										_movieCapture._useMediaFoundationH264 = true;
									}
								}
							}

							_shownSection = -1;
						}

						GUILayout.Space(16f);
					}


					GUI.enabled = _movieCapture._isRealTime;
					_movieCapture._noAudio = !GUILayout.Toggle(!_movieCapture._noAudio, "Record Audio");
					if (GUI.enabled)
						GUI.enabled = !_movieCapture._noAudio;

					// Audio Device
					GUILayout.BeginHorizontal();
					if (_shownSection != 1)
					{
						if (GUILayout.Button("+", GUILayout.Width(24)))
						{
							_shownSection = 1;
						}
					}
					else
					{
						if (GUILayout.Button("-", GUILayout.Width(24)))
						{
							_shownSection = -1;
						}
					}
					GUILayout.Label("Using Audio Source: " + _movieCapture._audioDeviceName);
					GUILayout.EndHorizontal();
					if (_audioDeviceNames != null && _shownSection == 1)
					{
						GUILayout.Label("Select Audio Source:");
						_audioPos = GUILayout.BeginScrollView(_audioPos, GUILayout.Height(100));
						int newAudioIndex = GUILayout.SelectionGrid(-1, _audioDeviceNames, 1) - 1;
						GUILayout.EndScrollView();

						if (newAudioIndex >= -1)
						{
							_movieCapture._audioDeviceIndex = newAudioIndex;
							if (_movieCapture._audioDeviceIndex >= 0)
								_movieCapture._audioDeviceName = _audioDeviceNames[_movieCapture._audioDeviceIndex + 1];
							else
								_movieCapture._audioDeviceName = "Unity";

							_shownSection = -1;
						}

						GUILayout.Space(16f);
					}



					// Audio Codec
					GUILayout.BeginHorizontal();
					if (_shownSection != 2)
					{
						if (GUILayout.Button("+", GUILayout.Width(24)))
						{
							_shownSection = 2;
						}
					}
					else
					{
						if (GUILayout.Button("-", GUILayout.Width(24)))
						{
							_shownSection = -1;
						}
					}
					GUILayout.Label("Using Audio Codec: " + _movieCapture._audioCodecName);
					if (_movieCapture._audioCodecIndex >= 0 && _audioCodecConfigurable[_movieCapture._audioCodecIndex])
					{
						GUILayout.Space(16f);
						if (GUILayout.Button("Configure Codec"))
						{
							NativePlugin.ConfigureAudioCodec(_movieCapture._audioCodecIndex);
						}
					}
					GUILayout.EndHorizontal();

					if (_audioCodecNames != null && _shownSection == 2)
					{
						GUILayout.Label("Select Audio Codec:");
						_audioCodecPos = GUILayout.BeginScrollView(_audioCodecPos, GUILayout.Height(100));
						int newCodecIndex = GUILayout.SelectionGrid(-1, _audioCodecNames, 1) - 1;
						GUILayout.EndScrollView();

						if (newCodecIndex >= -1)
						{
							_movieCapture._audioCodecIndex = newCodecIndex;
							if (_movieCapture._audioCodecIndex >= 0)
								_movieCapture._audioCodecName = _audioCodecNames[_movieCapture._audioCodecIndex + 1];
							else
								_movieCapture._audioCodecName = "Uncompressed";

							_shownSection = -1;
						}

						GUILayout.Space(16f);
					}

					GUI.enabled = true;

					GUILayout.Space(16f);
				}

				GUILayout.BeginHorizontal();
				GUILayout.Label("Filename Prefix & Ext: ");
				_movieCapture._autoFilenamePrefix = GUILayout.TextField(_movieCapture._autoFilenamePrefix, 64);
				if (_movieCapture._outputType == CaptureBase.OutputType.VideoFile)
				{
					_movieCapture._autoFilenameExtension = GUILayout.TextField(_movieCapture._autoFilenameExtension, 8);
				}
				else if (_movieCapture._outputType == CaptureBase.OutputType.ImageSequence)
				{
					GUILayout.TextField("png", 8);
				}
				else if (_movieCapture._outputType == CaptureBase.OutputType.NamedPipe)
				{

				}
				GUILayout.EndHorizontal();
				GUILayout.Space(16f);
				GUILayout.Space(16f);

				if (_whenRecordingAutoHideUI)
				{
					GUILayout.Label("(Press CTRL-F5 to stop capture)");
				}

				GUILayout.BeginHorizontal();
				if (!_movieCapture.IsCapturing())
				{
					GUI.color = Color.green;
					if (GUILayout.Button(_movieCapture._isRealTime?"Start Capture":"Start Render"))
					{
						StartCapture();
					}
					GUI.color = Color.white;
				}
				else
				{
					/*if (!_movieCapture.IsPaused())
					{
						if (GUILayout.Button("Pause Capture"))
						{
							PauseCapture();
						}
					}
					else
					{
						if (GUILayout.Button("Resume Capture"))
						{
							ResumeCapture();
						}					
					}

					if (GUILayout.Button("Cancel Capture"))
					{
						CancelCapture();
					}
					if (GUILayout.Button("Stop Capture"))
					{
						StopCapture();
					}*/
				}
				GUILayout.EndHorizontal();

				if (_movieCapture.IsCapturing())
				{
					if (!string.IsNullOrEmpty(_movieCapture.LastFilePath))
					{
						GUILayout.Label("Writing file: '" + System.IO.Path.GetFileName(_movieCapture.LastFilePath) + "'");
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(CaptureBase.LastFileSaved))
					{
						GUILayout.Space(16f);
						GUILayout.Label("Last file written: '" + System.IO.Path.GetFileName(CaptureBase.LastFileSaved) + "'");

						GUILayout.BeginHorizontal();
						if (GUILayout.Button("Browse"))
						{
							Utils.ShowInExplorer(CaptureBase.LastFileSaved);
						}
						Color prevColor = GUI.color;
						GUI.color = Color.cyan;
						if (GUILayout.Button("View Last Capture"))
						{
							Utils.OpenInDefaultApp(CaptureBase.LastFileSaved);
						}
						GUI.color = prevColor;
						
						GUILayout.EndHorizontal();
					}
				}
			}

			GUILayout.EndVertical();
		}

		private void GUI_RecordingStatus()
		{
			GUILayout.Space(8.0f);
			GUILayout.Label("Output", "box");
			GUILayout.BeginVertical("box");

			DrawGuiField("Recording to", System.IO.Path.GetFileName(_movieCapture.LastFilePath));
			GUILayout.Space(8.0f);

			GUILayout.Label("Video", "box");
			DrawGuiField("Dimensions", _movieCapture.GetRecordingWidth() + "x" + _movieCapture.GetRecordingHeight() + " @ " + ((int)_movieCapture._frameRate).ToString() + "hz");
			if (_movieCapture._outputType == CaptureBase.OutputType.VideoFile)
			{
				DrawGuiField("Codec", _movieCapture._codecName);
			}
			else if (_movieCapture._outputType == CaptureBase.OutputType.ImageSequence)
			{
				DrawGuiField("Codec",_movieCapture._imageSequenceFormat.ToString());
			}

			if (_movieCapture._outputType == CaptureBase.OutputType.VideoFile)
			{
				if (!_movieCapture._noAudio && _movieCapture._isRealTime)
				{
					GUILayout.Label("Audio", "box");
					DrawGuiField("Source", _movieCapture._audioDeviceName);
					DrawGuiField("Codec", _movieCapture._audioCodecName);
					if (_movieCapture._audioDeviceName == "Unity")
					{
						DrawGuiField("Sample Rate", _movieCapture._unityAudioSampleRate.ToString() + "hz");
						DrawGuiField("Channels", _movieCapture._unityAudioChannelCount.ToString());
					}
				}
			}

			GUILayout.EndVertical();

			GUILayout.Space(8.0f);

			GUILayout.Label("Stats", "box");
			GUILayout.BeginVertical("box");

			if (_movieCapture.FPS > 0f)
			{
				Color originalColor = GUI.color;
				if (_movieCapture._isRealTime)
				{
					float fpsDelta = (_movieCapture.FPS - (int)_movieCapture._frameRate);
					GUI.color = Color.red;
					if (fpsDelta > -10)
					{
						GUI.color = Color.yellow;
					}
					if (fpsDelta > -2)
					{
						GUI.color = Color.green;
					}
				}

				DrawGuiField("Capture Rate", string.Format("{0:0.##} / {1} FPS", _movieCapture.FPS, (int)_movieCapture._frameRate));

				GUI.color = originalColor;
			}
			else
			{
				DrawGuiField("Capture Rate", string.Format(".. / {0} FPS", (int)_movieCapture._frameRate));
			}

			DrawGuiField("File Size", ((float)_lastFileSize / (1024f * 1024f)).ToString("F1") + "MB");
			DrawGuiField("Video Length", _lastEncodedMinutes.ToString("00") + ":" + _lastEncodedSeconds.ToString("00") + "." + _lastEncodedFrame.ToString("000"));

			GUILayout.Label("Dropped Frames", "box");
			DrawGuiField("In Unity", _movieCapture.NumDroppedFrames.ToString());
			DrawGuiField("In Encoder ", _movieCapture.NumDroppedEncoderFrames.ToString());
			if (!_movieCapture._noAudio)
			{
				if (_movieCapture._audioCapture && _movieCapture._audioDeviceName == "Unity")
				{
					DrawGuiField("Audio Overflows", _movieCapture._audioCapture.OverflowCount.ToString());
				}
			}

			GUILayout.EndVertical();

			GUILayout.BeginHorizontal();

			if (!_movieCapture.IsPaused())
			{
				GUI.backgroundColor = Color.yellow;
				if (GUILayout.Button("Pause Capture"))
				{
					PauseCapture();
				}
			}
			else
			{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Resume Capture"))
				{
					ResumeCapture();
				}
			}

			GUI.backgroundColor = Color.cyan;
			if (GUILayout.Button("Cancel Capture"))
			{
				CancelCapture();
			}
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("Stop Capture"))
			{
				StopCapture();
			}
			GUI.backgroundColor = Color.white;

			GUILayout.EndHorizontal();
		}

		private void DrawGuiField(string a, string b)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(a);
			GUILayout.FlexibleSpace();
			GUILayout.Label(b);
			GUILayout.EndHorizontal();
		}

		private void StartCapture()
		{
			_lastFileSize = 0;
			_lastEncodedMinutes = _lastEncodedSeconds = _lastEncodedFrame = 0;
			if (_whenRecordingAutoHideUI)
			{
				_showUI = false;
			}
			if (_movieCapture != null)
			{
				_movieCapture.StartCapture();
			}
		}

		private void StopCapture()
		{
			if (_movieCapture != null)
			{
				_movieCapture.StopCapture();
			}
		}

		private void CancelCapture()
		{
			if (_movieCapture != null)
			{
				_movieCapture.CancelCapture();
			}
		}

		private void ResumeCapture()
		{
			if (_movieCapture != null)
			{
				_movieCapture.ResumeCapture();
			}
		}

		private void PauseCapture()
		{
			if (_movieCapture != null)
			{
				_movieCapture.PauseCapture();
			}
		}

		private void Update()
		{
			if (_movieCapture != null)
			{
				if (_whenRecordingAutoHideUI && !_showUI)
				{
					if (!_movieCapture.IsCapturing())
						_showUI = true;
				}

				if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F5))
				{
					if (_movieCapture.IsCapturing())
					{
						_movieCapture.StopCapture();
					}
				}

				if (_movieCapture.IsCapturing())
				{
					_lastFileSize = _movieCapture.GetCaptureFileSize();
					if (!_movieCapture._isRealTime)
					{
						_lastEncodedSeconds = (uint)Mathf.FloorToInt((float)_movieCapture.NumEncodedFrames / (float)_movieCapture._frameRate);
					}
					else
					{
						_lastEncodedSeconds = _movieCapture.TotalEncodedSeconds;
					}
					_lastEncodedMinutes = _lastEncodedSeconds / 60;
					_lastEncodedSeconds = _lastEncodedSeconds % 60;
					_lastEncodedFrame = _movieCapture.NumEncodedFrames % (uint)_movieCapture._frameRate;
				}
			}
		}
	}
}