using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace HighlightingSystem
{
	[DisallowMultipleComponent]
	public partial class Highlighter : MonoBehaviour
	{
		private enum Mode
		{
			None, 
			Highlighter, 
			Occluder, 
			HighlighterSeeThrough, 
			OccluderSeeThrough, 
		}

		// Constants (don't touch this!)
		#region Constants
		// 2 * PI constant for sine flashing
		private const float doublePI = 2f * Mathf.PI;
		
		// Occlusion color
		private readonly Color occluderColor = new Color(0f, 0f, 0f, 0f);

		// ZTest LEqual
		private const int zTestLessEqual = (int)CompareFunction.LessEqual;
		
		// ZTest Always
		private const int zTestAlways = (int)CompareFunction.Always;

		// Highlighting modes rendered in that order
		static private readonly Mode[] renderingOrder = new Mode[] { Mode.Highlighter, Mode.Occluder, Mode.HighlighterSeeThrough, Mode.OccluderSeeThrough };
		#endregion

		#region Static Fields
		// List of all highlighters in scene
		static private HashSet<Highlighter> highlighters = new HashSet<Highlighter>();

		// Global highlighting shaders ZWrite property
		// Set to unusual default value to force initialization on start
		static private int zWrite = -1;

		// Global highlighting shaders Offset Factor property
		// Set to unusual default value to force initialization on start
		static private float offsetFactor = float.NaN;

		// Global highlighting shaders Offset Units property
		// Set to unusual default value to force initialization on start
		static private float offsetUnits = float.NaN;
		#endregion

		#region Public Fields
		/// <summary>
		/// Controls see-through mode for highlighters or occluders. When set to true - highlighter in this mode will not be occluded by anything (except for see-through occluders). Occluder in this mode will overlap any highlighting.
		/// </summary>
		[HideInInspector]
		public bool seeThrough;

		/// <summary>
		/// Controls occluder mode. Note that non-see-through highlighting occluders will be enabled only when frame depth buffer is not available!
		/// </summary>
		[HideInInspector]
		public bool occluder;
		#endregion

		#region Private Fields
		// Cached transform component reference
		private Transform tr;

		// Cached Renderers
		private List<HighlighterRenderer> highlightableRenderers = new List<HighlighterRenderer>();

		// Renderers reinitialization is required flag
		private bool renderersDirty;

		// Static list to prevent unnecessary memory allocations when grabbing renderer components
		static private List<Component> sComponents = new List<Component>(4);

		// Highlighting mode
		private Mode mode;
		
		// Current ZTest value
		private bool zTest;
		
		// Current Stencil Ref value
		private bool stencilRef;

		// One-frame highlighting flag
		private int _once = -1;
		private bool once
		{
			get { return _once == Time.frameCount; }
			set { _once = value ? Time.frameCount : -1; }
		}

		// Flashing enabled flag
		private bool flashing;

		// Current highlighting color
		private Color currentColor;
		
		// Current transition value
		private float transitionValue;

		// Current Transition target
		private float transitionTarget;

		// Transition duration
		private float transitionTime;

		// One-frame highlighting color
		private Color onceColor;
		
		// Flashing frequency (times per second)
		private float flashingFreq;
		
		// Flashing from color
		private Color flashingColorMin;
		
		// Flashing to color
		private Color flashingColorMax;

		// Constant highlighting color
		private Color constantColor;

		// Opaque shader cached reference
		static private Shader _opaqueShader;
		static public Shader opaqueShader
		{
			get
			{
				if (_opaqueShader == null)
				{
					_opaqueShader = Shader.Find("Hidden/Highlighted/Opaque");
				}
				return _opaqueShader;
			}
		}
		
		// Transparent shader cached reference
		static private Shader _transparentShader;
		static public Shader transparentShader
		{
			get
			{
				if (_transparentShader == null)
				{
					_transparentShader = Shader.Find("Hidden/Highlighted/Transparent");
				}
				return _transparentShader;
			}
		}
		
		// Shared (for this component) replacement material for opaque geometry highlighting
		private Material _opaqueMaterial;
		private Material opaqueMaterial
		{
			get
			{
				if (_opaqueMaterial == null)
				{
					_opaqueMaterial = new Material(opaqueShader);
					
					// Make sure that ShaderPropertyIDs is initialized
					ShaderPropertyID.Initialize();
					
					// Make sure that shader will have proper default values
					_opaqueMaterial.SetInt(ShaderPropertyID._ZTest, GetZTest(zTest));
					_opaqueMaterial.SetInt(ShaderPropertyID._StencilRef, GetStencilRef(stencilRef));
				}
				return _opaqueMaterial;
			}
		}
		#endregion

		#region MonoBehaviour
		// 
		private void Awake()
		{
			ShaderPropertyID.Initialize();

			tr = GetComponent<Transform>();

			renderersDirty = true;
			seeThrough = zTest = true;
			mode = Mode.None;
			stencilRef = true;

			// Initial highlighting state
			once = false;
			flashing = false;
			occluder = false;
			transitionValue = transitionTarget = 0f;
			onceColor = Color.red;
			flashingFreq = 2f;
			flashingColorMin = new Color(0f, 1f, 1f, 0f);
			flashingColorMax = new Color(0f, 1f, 1f, 1f);
			constantColor = Color.yellow;
		}
		
		// 
		private void OnEnable()
		{
			highlighters.Add(this);
		}

		// 
		private void OnDisable()
		{
			highlighters.Remove(this);
			
			ClearRenderers();

			// Reset highlighting parameters to default values
			renderersDirty = true;
			once = false;
			flashing = false;
			transitionValue = transitionTarget = 0f;

			/* 
			// Reset custom parameters of the highlighting
			occluder = false;
			seeThrough = false;
			onceColor = Color.red;
			flashingFreq = 2f;
			flashingColorMin = new Color(0f, 1f, 1f, 0f);
			flashingColorMax = new Color(0f, 1f, 1f, 1f);
			constantColor = Color.yellow;
			transitionTime = 0f;
			*/
		}

		// 
		private void Update()
		{
			UpdateTransition();
		}
		#endregion

		#region Private Methods
		// Clear cached renderers
		private void ClearRenderers()
		{
			for (int i = highlightableRenderers.Count - 1; i >= 0; i--)
			{
				HighlighterRenderer renderer = highlightableRenderers[i];
				renderer.SetState(false);
			}
			highlightableRenderers.Clear();
		}

		// This method defines the way in which renderers are initialized
		private void UpdateRenderers()
		{
			if (renderersDirty)
			{
				ClearRenderers();

				// Find all renderers which should be controlled with this Highlighter component
				List<Renderer> renderers = new List<Renderer>();
				GrabRenderers(tr, renderers);

				// Cache found renderers
				for (int i = 0, imax = renderers.Count; i < imax; i++)
				{
					GameObject rg = renderers[i].gameObject;
					HighlighterRenderer renderer = rg.GetComponent<HighlighterRenderer>();
					if (renderer == null) { renderer = rg.AddComponent<HighlighterRenderer>(); }
					renderer.SetState(true);

					renderer.Initialize(opaqueMaterial, transparentShader);
					highlightableRenderers.Add(renderer);
				}
				
				renderersDirty = false;
			}
		}

		// Recursively follows hierarchy of objects from t, searches for Renderers and adds them to the list. 
		// Breaks if HighlighterBlocker or another Highlighter component found
		private void GrabRenderers(Transform t, List<Renderer> renderers)
		{
			GameObject g = t.gameObject;

			// Find all Renderers of all types on current GameObject g and add them to the renderers list
			for (int i = 0, imax = types.Count; i < imax; i++)
			{
				g.GetComponents(types[i], sComponents);
				for (int j = 0, jmax = sComponents.Count; j < jmax; j++)
				{
                    renderers.Add(sComponents[j] as Renderer);
				}
			}
			sComponents.Clear();
			
			// Return if transform t doesn't have any children
			int childCount = t.childCount;
			if (childCount == 0) { return; }
			
			// Recursively cache renderers on all child GameObjects
			for (int i = 0; i < childCount; i++)
			{
				Transform childTransform = t.GetChild(i);

				// Do not cache Renderers of this childTransform in case it has it's own Highlighter component
				Highlighter h = childTransform.GetComponent<Highlighter>();
				if (h != null) { continue; }

				// Do not cache Renderers of this childTransform in case HighlighterBlocker found
				HighlighterBlocker hb = childTransform.GetComponent<HighlighterBlocker>();
				if (hb != null) { continue; }
				
				GrabRenderers(childTransform, renderers);
			}
		}

		// Sets ZTest and Stencil Ref parameters on all materials of all renderers of this object
		private void UpdateShaderParams(bool zt, bool sr)
		{
			// ZTest
			if (zTest != zt)
			{
				zTest = zt;
				int ztInt = GetZTest(zTest);
				opaqueMaterial.SetInt(ShaderPropertyID._ZTest, ztInt);
				for (int i = 0; i < highlightableRenderers.Count; i++) 
				{
					highlightableRenderers[i].SetZTestForTransparent(ztInt);
				}
			}
			
			// Stencil Ref
			if (stencilRef != sr)
			{
				stencilRef = sr;
				int srInt = GetStencilRef(stencilRef);
				opaqueMaterial.SetInt(ShaderPropertyID._StencilRef, srInt);
				for (int i = 0; i < highlightableRenderers.Count; i++)
				{
					highlightableRenderers[i].SetStencilRefForTransparent(srInt);
				}
			}
		}

		// Updates highlighting color
		private void UpdateColors()
		{
			if (once)
			{
				currentColor = onceColor;
			}
			else if (flashing)
			{
				// Flashing frequency is not affected by Time.timeScale
				currentColor = Color.Lerp(flashingColorMin, flashingColorMax, 0.5f * Mathf.Sin(Time.realtimeSinceStartup * flashingFreq * doublePI) + 0.5f);
			}
			else if (transitionValue > 0f)
			{
				currentColor = constantColor;
				currentColor.a *= transitionValue;
			}
			else if (occluder)
			{
				currentColor = occluderColor;
			}
			else
			{
				return;
			}

			// Apply color
			opaqueMaterial.SetColor(ShaderPropertyID._Color, currentColor);
			for (int i = 0; i < highlightableRenderers.Count; i++)
			{
				highlightableRenderers[i].SetColorForTransparent(currentColor);
			}
		}

		// Update transition value
		private void UpdateTransition()
		{
			if (transitionValue != transitionTarget)
			{
				if (transitionTime <= 0f)
				{
					transitionValue = transitionTarget;
				}
				else
				{
					float dir = (transitionTarget > 0f ? 1f : -1f);
					transitionValue = Mathf.Clamp01(transitionValue + (dir * Time.unscaledDeltaTime) / transitionTime);
				}
			}
		}

		// 
		private void FillBufferInternal(CommandBuffer buffer, Mode m, bool depthAvailable)
		{
			UpdateRenderers();

			bool isHighlighter = once || flashing || (transitionValue > 0f);
			bool isOccluder = occluder && (seeThrough || !depthAvailable);

			// Update mode
			mode = Mode.None;
			if (isHighlighter)
			{
				mode = seeThrough ? Mode.HighlighterSeeThrough : Mode.Highlighter;
			}
			else if (isOccluder)
			{
				mode = seeThrough ? Mode.OccluderSeeThrough : Mode.Occluder;
			}

			if (mode == Mode.None || mode != m) { return; }

			if (isHighlighter)
			{
				// ZTest = (seeThrough ? Always : LEqual), StencilRef = 1
				UpdateShaderParams(seeThrough, true);
			}
			else if (isOccluder)
			{
				// ZTest = LEqual, StencilRef = seeThrough ? 1 : 0
				UpdateShaderParams(false, seeThrough);
			}
			UpdateColors();

			// Fill CommandBuffer with this highlighter rendering commands
			for (int i = highlightableRenderers.Count - 1; i >= 0; i--)
			{
				// To avoid null-reference exceptions when cached renderer has been removed but ReinitMaterials wasn't been called
				HighlighterRenderer renderer = highlightableRenderers[i];
				if (renderer == null)
				{
					highlightableRenderers.RemoveAt(i);
				}
				// Try to fill buffer
				else if (!renderer.FillBuffer(buffer))
				{
					highlightableRenderers.RemoveAt(i);
					renderer.SetState(false);
				}
			}
		}
		#endregion

		#region Static Methods
		// Fill CommandBuffer with highlighters rendering commands
		static public void FillBuffer(CommandBuffer buffer, bool depthAvailable)
		{
			for (int i = 0; i < renderingOrder.Length; i++)
			{
				Mode mode = renderingOrder[i];

				var e = highlighters.GetEnumerator();
				while (e.MoveNext())
				{
					Highlighter highlighter = e.Current;
					highlighter.FillBufferInternal(buffer, mode, depthAvailable);
				}
			}
		}

		// Returns integer value for ZTest, which can be passed directly to the shaders (true = Always, false = LEqual)
		static private int GetZTest(bool enabled)
		{
			return enabled ? zTestAlways : zTestLessEqual;
		}

		// Returns integer value for StencilRef, which can be passed directly to the shaders (true = 1, false = 0)
		static private int GetStencilRef(bool enabled)
		{
			return enabled ? 1 : 0;
		}

		// Set highlighting shaders global ZWrite property
		static public void SetZWrite(int value)
		{
			if (zWrite == value) { return; }
			zWrite = value;
			Shader.SetGlobalInt(ShaderPropertyID._HighlightingZWrite, zWrite);
		}
		
		// Set highlighting shaders global OffsetFactor property
		static public void SetOffsetFactor(float value)
		{
			if (offsetFactor == value) { return; }
			offsetFactor = value;
			Shader.SetGlobalFloat(ShaderPropertyID._HighlightingOffsetFactor, offsetFactor);
		}

		// Set highlighting shaders global OffsetUnits property
		static public void SetOffsetUnits(float value)
		{
			if (offsetUnits == value) { return; }
			offsetUnits = value;
			Shader.SetGlobalFloat(ShaderPropertyID._HighlightingOffsetUnits, offsetUnits);
		}
		#endregion
	}
}