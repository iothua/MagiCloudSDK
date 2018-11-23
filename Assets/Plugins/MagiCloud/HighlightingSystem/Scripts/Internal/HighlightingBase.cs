using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace HighlightingSystem
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class HighlightingBase : MonoBehaviour
	{
		#region Static Fields and Constants
		static protected readonly Color colorClear = new Color(0f, 0f, 0f, 0f);
		static protected readonly string renderBufferName = "HighlightingSystem";
		static protected readonly Matrix4x4 identityMatrix = Matrix4x4.identity;
		protected const CameraEvent queue = CameraEvent.BeforeImageEffectsOpaque;
		
		static protected RenderTargetIdentifier cameraTargetID;
		
		static protected Mesh quad;

		// Current graphics device version
		static protected GraphicsDeviceType device = GraphicsDeviceType.Null;

		// Same check as in HLSLSupport.cginc
		static protected bool uvStartsAtTop
		{
			get
			{
				return
					device == GraphicsDeviceType.Direct3D9			// SHADER_API_D3D9
					|| device == GraphicsDeviceType.Xbox360			// SHADER_API_XBOX360
					|| device == GraphicsDeviceType.PlayStation3		// SHADER_API_PS3
					|| device == GraphicsDeviceType.Direct3D11		// SHADER_API_D3D11
					|| device == GraphicsDeviceType.PlayStationVita	// SHADER_API_PSP2
					|| device == GraphicsDeviceType.PlayStation4		// SHADER_API_PSSL
					|| device == GraphicsDeviceType.Metal			// SHADER_API_METAL
					// device == GraphicsDeviceType.?				// SHADER_API_D3D11_9X - Direct3D 11 “feature level 9.x” target for Windows Store & Windows Phone.
					// device == GraphicsDeviceType.?				// SHADER_API_WIIU - Nintendo WiiU
					;
			}
		}
		#endregion
		
		#region Public Fields
		// True if supported on this platform
		public bool isSupported
		{
			get
			{
				return CheckSupported(false);
			}
		}

		// Depth offset factor for highlighting shaders
		public float offsetFactor = 0f;

		// Depth offset units for highlighting shaders
		public float offsetUnits = 0f;

		// Highlighting buffer size downsample factor
		public int downsampleFactor
		{
			get { return _downsampleFactor; }
			set
			{
				if (_downsampleFactor != value)
				{
					// Is power of two check
					if ((value != 0) && ((value & (value - 1)) == 0))
					{
						_downsampleFactor = value;
					}
					else
					{
						Debug.LogWarning("HighlightingSystem : Prevented attempt to set incorrect downsample factor value.");
					}
				}
			}
		}
		
		// Blur iterations
		public int iterations
		{
			get { return _iterations; }
			set
			{
				if (_iterations != value)
				{
					_iterations = value;
				}
			}
		}
		
		// Blur minimal spread
		public float blurMinSpread
		{
			get { return _blurMinSpread; }
			set
			{
				if (_blurMinSpread != value)
				{
					_blurMinSpread = value;
				}
			}
		}
		
		// Blur spread per iteration
		public float blurSpread
		{
			get { return _blurSpread; }
			set
			{
				if (_blurSpread != value)
				{
					_blurSpread = value;
				}
			}
		}
		
		// Blurring intensity for the blur material
		public float blurIntensity
		{
			get { return _blurIntensity; }
			set
			{
				if (_blurIntensity != value)
				{
					_blurIntensity = value;
					if (Application.isPlaying)
					{
						blurMaterial.SetFloat(ShaderPropertyID._Intensity, _blurIntensity);
					}
				}
			}
		}

		// Blitter component reference (optional)
		public HighlightingBlitter blitter
		{
			get { return _blitter; }
			set
			{
				if (_blitter != null)
				{
					_blitter.Unregister(this);
				}
				_blitter = value;
				if (_blitter != null)
				{
					_blitter.Register(this);
				}
			}
		}
		#endregion
		
		#region Protected Fields
		protected CommandBuffer renderBuffer;

		protected int cachedWidth = -1;
		protected int cachedHeight = -1;
		protected int cachedAA = -1;

		[FormerlySerializedAs("downsampleFactor")]
		[SerializeField]
		protected int _downsampleFactor = 4;

		[FormerlySerializedAs("iterations")]
		[SerializeField]
		protected int _iterations = 2;

		[FormerlySerializedAs("blurMinSpread")]
		[SerializeField]
		protected float _blurMinSpread = 0.65f;

		[FormerlySerializedAs("blurSpread")]
		[SerializeField]
		protected float _blurSpread = 0.25f;

		[SerializeField]
		protected float _blurIntensity = 0.3f;

		[SerializeField]
		protected HighlightingBlitter _blitter;

		// RenderTargetidentifier for the highlightingBuffer RenderTexture
		protected RenderTargetIdentifier highlightingBufferID;

		// RenderTexture with highlighting buffer
		protected RenderTexture highlightingBuffer = null;

		// Camera reference
		protected Camera cam = null;

		// True if framebuffer depth data is currently available (it is required for the highlighting occlusion feature)
		protected bool isDepthAvailable = true;

		// Material parameters
		protected const int BLUR = 0;
		protected const int CUT = 1;
		protected const int COMP = 2;
		static protected readonly string[] shaderPaths = new string[]
		{
			"Hidden/Highlighted/Blur", 
			"Hidden/Highlighted/Cut", 
			"Hidden/Highlighted/Composite", 
		};
		static protected Shader[] shaders;
		static protected Material[] materials;

		// Dynamic materials
		protected Material blurMaterial;
		protected Material cutMaterial;
		protected Material compMaterial;

		static protected bool initialized = false;

		static protected HashSet<Camera> cameras = new HashSet<Camera>();
		#endregion

		#region MonoBehaviour
		// 
		protected virtual void OnEnable()
		{
			Initialize();

			if (!CheckSupported(true))
			{
				enabled = false;
				Debug.LogError("HighlightingSystem : Highlighting System has been disabled due to unsupported Unity features on the current platform!");
				return;
			}

			blurMaterial = new Material(materials[BLUR]);
			cutMaterial = new Material(materials[CUT]);
			compMaterial = new Material(materials[COMP]);
			
			// Set initial intensity in blur material
			blurMaterial.SetFloat(ShaderPropertyID._Intensity, _blurIntensity);

			renderBuffer = new CommandBuffer();
			renderBuffer.name = renderBufferName;

			cam = GetComponent<Camera>();

			cameras.Add(cam);

			cam.AddCommandBuffer(queue, renderBuffer);

			if (_blitter != null)
			{
				_blitter.Register(this);
			}
		}
		
		// 
		protected virtual void OnDisable()
		{
			cameras.Remove(cam);

			if (renderBuffer != null)
			{
				cam.RemoveCommandBuffer(queue, renderBuffer);
				renderBuffer = null;
			}

			if (highlightingBuffer != null && highlightingBuffer.IsCreated())
			{
				highlightingBuffer.Release();
				highlightingBuffer = null;
			}

			if (_blitter != null)
			{
				_blitter.Unregister(this);
			}
		}

		// 
		protected virtual void OnPreRender()
		{
			bool updateHighlightingBuffer = false;
			int aa = GetAA();
			
			bool depthAvailable = (aa == 1);

			if (cam.actualRenderingPath == RenderingPath.Forward || cam.actualRenderingPath == RenderingPath.VertexLit)
			{
				// In case MSAA is enabled in forward/vertex lit rendeirng paths - depth buffer is not available
				if (aa > 1)
				{
					depthAvailable = false;
				}

				// In case camera clearFlags is set to 'Depth only' or 'Don't clear' in forward/vertex lit rendeirng paths - depth buffer is not available
				if (cam.clearFlags == CameraClearFlags.Depth || cam.clearFlags == CameraClearFlags.Nothing)
				{
					depthAvailable = false;
				}
			}

			// Check if framebuffer depth data availability has changed
			if (isDepthAvailable != depthAvailable)
			{
				updateHighlightingBuffer = true;
				isDepthAvailable = depthAvailable;
				// Update ZWrite value for all highlighting shaders correspondingly (isDepthAvailable ? ZWrite Off : ZWrite On)
				Highlighter.SetZWrite(isDepthAvailable ? 0 : 1);
				if (isDepthAvailable)
				{
					Debug.LogWarning("HighlightingSystem : Framebuffer depth data is available back again. Depth occlusion enabled, highlighting occluders disabled. (This message is harmless)");
				}
				else
				{
					Debug.LogWarning("HighlightingSystem : Framebuffer depth data is not available. Depth occlusion disabled, highlighting occluders enabled. (This message is harmless)");
				}
			}

			updateHighlightingBuffer |= (highlightingBuffer == null || cam.pixelWidth != cachedWidth || cam.pixelHeight != cachedHeight || aa != cachedAA);

			if (updateHighlightingBuffer)
			{
				if (highlightingBuffer != null && highlightingBuffer.IsCreated())
				{
					highlightingBuffer.Release();
				}

				cachedWidth = cam.pixelWidth;
				cachedHeight = cam.pixelHeight;
				cachedAA = aa;
				
				highlightingBuffer = new RenderTexture(cachedWidth, cachedHeight, isDepthAvailable ? 0 : 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
				highlightingBuffer.antiAliasing = cachedAA;
				highlightingBuffer.filterMode = FilterMode.Point;
				highlightingBuffer.useMipMap = false;
				highlightingBuffer.wrapMode = TextureWrapMode.Clamp;
				if (!highlightingBuffer.Create())
				{
					Debug.LogError("HighlightingSystem : UpdateHighlightingBuffer() : Failed to create highlightingBuffer RenderTexture!");
				}
				
				highlightingBufferID = new RenderTargetIdentifier(highlightingBuffer);
				cutMaterial.SetTexture(ShaderPropertyID._HighlightingBuffer, highlightingBuffer);
				compMaterial.SetTexture(ShaderPropertyID._HighlightingBuffer, highlightingBuffer);
				
				Vector4 v = new Vector4(1f / (float)highlightingBuffer.width, 1f / (float)highlightingBuffer.height, 0f, 0f);
				cutMaterial.SetVector(ShaderPropertyID._HighlightingBufferTexelSize, v);
			}

			// Set global depth offset properties for highlighting shaders to the values which has this HighlightingBase component
			Highlighter.SetOffsetFactor(offsetFactor);
			Highlighter.SetOffsetUnits(offsetUnits);

			RebuildCommandBuffer();
		}

		// Do not remove! 
		// Having this method in this script is necessary to support multiple cameras with different clear flags even in case custom blitter is being used
		protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			if (blitter == null)
			{
				Blit(src, dst);
			}
			else
			{
				Graphics.Blit(src, dst);
			}
		}
		#endregion

		#region Internal
		// 
		static public bool IsHighlightingCamera(Camera cam)
		{
			return cameras.Contains(cam);
		}

		// 
		static protected void Initialize()
		{
			if (initialized) { return; }

			// Determine graphics device type
			device = SystemInfo.graphicsDeviceType;

			// Initialize shader property constants
			ShaderPropertyID.Initialize();

			// Initialize shaders and materials
			int l = shaderPaths.Length;
			shaders = new Shader[l];
			materials = new Material[l];
			for (int i = 0; i < l; i++)
			{
				Shader shader = Shader.Find(shaderPaths[i]);
				shaders[i] = shader;
				
				Material material = new Material(shader);
				materials[i] = material;
			}

			// Initialize static RenderTargetIdentifier(s)
			cameraTargetID = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);

			// Create static quad mesh
			CreateQuad();

			initialized = true;
		}

		// 
		static protected void CreateQuad()
		{
			if (quad == null)
			{
				quad = new Mesh();
			}
			else
			{
				quad.Clear();
			}
			
			float y1 = -1f;
			float y2 = 1f;
			
			if (uvStartsAtTop)
			{
				y1 = 1f;
				y2 = -1f;
			}
			
			quad.vertices = new Vector3[]
			{
				new Vector3(-1f, y1, 0f), // Bottom-Left
				new Vector3(-1f, y2, 0f), // Upper-Left
				new Vector3( 1f, y2, 0f), // Upper-Right
				new Vector3( 1f, y1, 0f)  // Bottom-Right
			};
			
			quad.uv = new Vector2[]
			{
				new Vector2(0f, 0f), 
				new Vector2(0f, 1f), 
				new Vector2(1f, 1f), 
				new Vector2(1f, 0f)
			};
			
			quad.colors = new Color[]
			{
				colorClear, 
				colorClear, 
				colorClear, 
				colorClear
			};
			
			quad.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
		}

		// 
		protected virtual int GetAA()
		{
			int aa = QualitySettings.antiAliasing;
			if (aa == 0) { aa = 1; }

			// Reset aa value to 1 in case camera is in DeferredLighting or DeferredShading Rendering Path
			if (cam.actualRenderingPath == RenderingPath.DeferredLighting || cam.actualRenderingPath == RenderingPath.DeferredShading) { aa = 1; }

			return aa;
		}

		// 
		protected virtual bool CheckSupported(bool verbose)
		{
			bool supported = true;

			// Image Effects supported?
			if (!SystemInfo.supportsImageEffects)
			{
				if (verbose) { Debug.LogError("HighlightingSystem : Image effects is not supported on this platform!"); }
				supported = false;
			}
			
			// Render Textures supported?
			if (!SystemInfo.supportsRenderTextures)
			{
				if (verbose) { Debug.LogError("HighlightingSystem : RenderTextures is not supported on this platform!"); }
				supported = false;
			}
			
			// Required Render Texture Format supported?
			if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
			{
				if (verbose) { Debug.LogError("HighlightingSystem : RenderTextureFormat.ARGB32 is not supported on this platform!"); }
				supported = false;
			}

			// Stencil buffer supported?
			if (SystemInfo.supportsStencil < 1)
			{
				if (verbose) { Debug.LogError("HighlightingSystem : Stencil buffer is not supported on this platform!"); }
				supported = false;
			}
			
			// HighlightingOpaque shader supported?
			if (!Highlighter.opaqueShader.isSupported)
			{
				if (verbose) { Debug.LogError("HighlightingSystem : HighlightingOpaque shader is not supported on this platform!"); }
				supported = false;
			}
			
			// HighlightingTransparent shader supported?
			if (!Highlighter.transparentShader.isSupported)
			{
				if (verbose) { Debug.LogError("HighlightingSystem : HighlightingTransparent shader is not supported on this platform!"); }
				supported = false;
			}

			// Highlighting shaders supported?
			for (int i = 0; i < shaders.Length; i++)
			{
				Shader shader = shaders[i];
				if (!shader.isSupported)
				{
					if (verbose) { Debug.LogError("HighlightingSystem : Shader '" + shader.name + "' is not supported on this platform!"); }
					supported = false;
				}
			}

			return supported;
		}

		// 
		protected virtual void RebuildCommandBuffer()
		{
			renderBuffer.Clear();

			RenderTargetIdentifier depthID = isDepthAvailable ? cameraTargetID : highlightingBufferID;

			// Prepare and clear render target
			renderBuffer.SetRenderTarget(highlightingBufferID, depthID);
			renderBuffer.ClearRenderTarget(!isDepthAvailable, true, colorClear);

			// Fill buffer with highlighters rendering commands
			Highlighter.FillBuffer(renderBuffer, isDepthAvailable);

			// Create two buffers for blurring the image
			RenderTargetIdentifier blur1ID = new RenderTargetIdentifier(ShaderPropertyID._HighlightingBlur1);
			RenderTargetIdentifier blur2ID = new RenderTargetIdentifier(ShaderPropertyID._HighlightingBlur2);

			int width = highlightingBuffer.width / _downsampleFactor;
			int height = highlightingBuffer.height / _downsampleFactor;

			renderBuffer.GetTemporaryRT(ShaderPropertyID._HighlightingBlur1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
			renderBuffer.GetTemporaryRT(ShaderPropertyID._HighlightingBlur2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

			renderBuffer.Blit(highlightingBufferID, blur1ID);

			// Blur the small texture
			bool oddEven = true;
			for (int i = 0; i < _iterations; i++)
			{
				float off = _blurMinSpread + _blurSpread * i;
				renderBuffer.SetGlobalFloat(ShaderPropertyID._HighlightingBlurOffset, off);
				
				if (oddEven)
				{
					renderBuffer.Blit(blur1ID, blur2ID, blurMaterial);
				}
				else
				{
					renderBuffer.Blit(blur2ID, blur1ID, blurMaterial);
				}
				
				oddEven = !oddEven;
			}

			// Upscale blurred texture and cut stencil from it
			renderBuffer.SetGlobalTexture(ShaderPropertyID._HighlightingBlurred, oddEven ? blur1ID : blur2ID);
			renderBuffer.SetRenderTarget(highlightingBufferID, depthID);
			renderBuffer.DrawMesh(quad, identityMatrix, cutMaterial);

			// Cleanup
			renderBuffer.ReleaseTemporaryRT(ShaderPropertyID._HighlightingBlur1);
			renderBuffer.ReleaseTemporaryRT(ShaderPropertyID._HighlightingBlur2);
		}

		// Blit highlighting result to the destination RenderTexture
		public virtual void Blit(RenderTexture src, RenderTexture dst)
		{
			Graphics.Blit(src, dst, compMaterial);
		}
		#endregion
	}
}