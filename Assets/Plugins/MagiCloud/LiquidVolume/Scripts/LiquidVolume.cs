using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LiquidVolumeFX {
	public enum TOPOLOGY {
		Sphere = 0,
		Cylinder = 1,
		Cube = 2,
		Irregular = 10
	}

	public enum DETAIL {
		Simple = 0,
		Default = 10,
		DefaultNoFlask = 11,
		BumpTexture = 20,
		Reflections = 30,
		Smoke = 40,
	}


	[ExecuteInEditMode]
	[HelpURL ("http://kronnect.com/taptapgo")]
	[AddComponentMenu ("Effects/Liquid Volume")]
	[DisallowMultipleComponent]
	public class LiquidVolume : MonoBehaviour {

		[SerializeField]
		TOPOLOGY _topology = TOPOLOGY.Sphere;

		public TOPOLOGY topology {
			get { return _topology; }
			set {
				if (_topology != value) {
					if (_topology == TOPOLOGY.Irregular)
						CleanupCommandBuffer ();
					_topology = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		DETAIL _detail = DETAIL.Default;

		public DETAIL detail {
			get { return _detail; }
			set {
				if (_detail != value) {
					_detail = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1)]
		float _level = 0.5f;

		public float level {
			get { return _level; }
			set {
				if (_level != Mathf.Clamp01 (value)) {
					_level = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[ColorUsage (true)]
		Color _liquidColor1 = new Color (0, 1, 0, 0.1f);

		public Color liquidColor1 {
			get { return _liquidColor1; }
			set {
				if (_liquidColor1 != value) {
					_liquidColor1 = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0.1f, 4.85f)]
		float _liquidScale1 = 1f;

		public float liquidScale1 {
			get { return _liquidScale1; }
			set {
				if (_liquidScale1 != value) {
					_liquidScale1 = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[ColorUsage (true)]
		Color _liquidColor2 = new Color (1, 0, 0, 0.3f);

		public Color liquidColor2 {
			get { return _liquidColor2; }
			set {
				if (_liquidColor2 != value) {
					_liquidColor2 = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (2f, 4.85f)]
		float _liquidScale2 = 5f;

		public float liquidScale2 {
			get { return _liquidScale2; }
			set {
				if (_liquidScale2 != value) {
					_liquidScale2 = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1)]
		float _alpha = 1f;

		public float alpha {
			get { return _alpha; }
			set {
				if (_alpha != Mathf.Clamp01 (value)) {
					_alpha = Mathf.Clamp01 (value);
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[ColorUsage (false)]
		Color _emissionColor = new Color (0, 0, 0);

		public Color emissionColor {
			get { return _emissionColor; }
			set {
				if (_emissionColor != value) {
					_emissionColor = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[SerializeField]
		[Range (0, 8f)]
		float _emissionBrightness;

		public float emissionBrightness {
			get { return _emissionBrightness; }
			set {
				if (_emissionBrightness != value) {
					_emissionBrightness = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[SerializeField]
		bool _ditherShadows = true;

		public bool ditherShadows {
			get { return _ditherShadows; }
			set {
				if (_ditherShadows != value) {
					_ditherShadows = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1)]
		float _murkiness = 1.0f;

		public float murkiness {
			get { return _murkiness; }
			set {
				if (_murkiness != value) {
					_murkiness = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1f)]
		float _turbulence1 = 0.5f;

		public float turbulence1 {
			get { return _turbulence1; }
			set {
				if (_turbulence1 != value) {
					_turbulence1 = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1f)]
		float _turbulence2 = 0.2f;

		public float turbulence2 {
			get { return _turbulence2; }
			set {
				if (_turbulence2 != value) {
					_turbulence2 = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		float _frecuency = 1f;

		public float frecuency {
			get { return _frecuency; }
			set {
				if (_frecuency != value) {
					_frecuency = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0f, 2f)]
		float _speed = 1f;

		public float speed {
			get { return _speed; }
			set {
				if (_speed != value) {
					_speed = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 5f)]
		float _sparklingIntensity = 0.1f;

		public float sparklingIntensity {
			get { return _sparklingIntensity; }
			set {
				if (_sparklingIntensity != value) {
					_sparklingIntensity = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1)]
		float _sparklingAmount = 0.2f;

		public float sparklingAmount {
			get { return _sparklingAmount; }
			set {
				if (_sparklingAmount != value) {
					_sparklingAmount = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 10)]
		float _deepObscurance = 2.0f;

		public float deepObscurance {
			get { return _deepObscurance; }
			set {
				if (_deepObscurance != value) {
					_deepObscurance = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[ColorUsage (true)]
		Color _foamColor = new Color (1, 1, 1, 0.65f);

		public Color foamColor {
			get { return _foamColor; }
			set {
				if (_foamColor != value) {
					_foamColor = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0.01f, 1f)]
		float _foamScale = 0.2f;

		public float foamScale {
			get { return _foamScale; }
			set {
				if (_foamScale != value) {
					_foamScale = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 0.1f)]
		float _foamThickness = 0.04f;

		public float foamThickness {
			get { return _foamThickness; }
			set {
				if (_foamThickness != value) {
					_foamThickness = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (-1, 1)]
		float _foamDensity = 0.5f;

		public float foamDensity {
			get { return _foamDensity; }
			set {
				if (_foamDensity != value) {
					_foamDensity = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (8, 100)]
		float _foamWeight = 10f;

		public float foamWeight {
			get { return _foamWeight; }
			set {
				if (_foamWeight != value) {
					_foamWeight = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1)]
		float _foamTurbulence = 1f;

		public float foamTurbulence {
			get { return _foamTurbulence; }
			set {
				if (_foamTurbulence != value) {
					_foamTurbulence = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		bool _foamVisibleFromBottom = true;

		public bool foamVisibleFromBottom {
			get { return _foamVisibleFromBottom; }
			set {
				if (_foamVisibleFromBottom != value) {
					_foamVisibleFromBottom = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		bool _smokeEnabled = true;

		public bool smokeEnabled {
			get { return _smokeEnabled; }
			set {
				if (_smokeEnabled != value) {
					_smokeEnabled = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[ColorUsage (true)]
		[SerializeField]
		Color _smokeColor = new Color (0.7f, 0.7f, 0.7f, 0.25f);

		public Color smokeColor {
			get { return _smokeColor; }
			set {
				if (_smokeColor != value) {
					_smokeColor = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0.01f, 1f)]
		float _smokeScale = 0.25f;

		public float smokeScale {
			get { return _smokeScale; }
			set {
				if (_smokeScale != value) {
					_smokeScale = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 10f)]
		float _smokeBaseObscurance = 2.0f;

		public float smokeBaseObscurance {
			get { return _smokeBaseObscurance; }
			set {
				if (_smokeBaseObscurance != value) {
					_smokeBaseObscurance = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 20f)]
		float _smokeSpeed = 5.0f;

		public float smokeSpeed {
			get { return _smokeSpeed; }
			set {
				if (_smokeSpeed != value) {
					_smokeSpeed = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
        bool _fixMesh;

        public bool fixMesh {
            get { return _fixMesh; }
            set {
                if (_fixMesh != value) {
                    _fixMesh = value;
                    UpdateMaterialProperties();
                }
            }
        }

        public Mesh originalMesh;
        public Vector3 originalPivotOffset;

        [SerializeField]
        Vector3 _pivotOffset;

        public Vector3 pivotOffset {
            get { return _pivotOffset; }
            set {
                if (_pivotOffset != value) {
                    _pivotOffset = value;
                    UpdateMaterialProperties();
                }
            }
        }
        [SerializeField]
        [Range(0, 1.5f)]
        float _upperLimit = 1.5f;

		public float upperLimit {
			get { return _upperLimit; }
			set {
				if (_upperLimit != value) {
					_upperLimit = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
        [Range(-1.5f, 1.5f)]
        float _lowerLimit = -1.5f;

		public float lowerLimit {
			get { return _lowerLimit; }
			set {
				if (_lowerLimit != value) {
					_lowerLimit = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		int _subMeshIndex = -1;

		public int subMeshIndex {
			get { return _subMeshIndex; }
			set {
				if (_subMeshIndex != value) {
					_subMeshIndex = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[ColorUsage (true)]
		Color _flaskTint = new Color (0, 0, 0, 1);

		public Color flaskTint {
			get { return _flaskTint; }
			set {
				if (_flaskTint != value) {
					_flaskTint = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1)]
		float _flaskThickness = 0.03f;

		public float flaskThickness {
			get { return _flaskThickness; }
			set {
				if (_flaskThickness != value) {
					_flaskThickness = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1)]
		float _flaskGlossinessExternal = 0.767f;

		public float flaskGlossinessExternal {
			get { return _flaskGlossinessExternal; }
			set {
				if (_flaskGlossinessExternal != value) {
					_flaskGlossinessExternal = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1)]
		float _flaskGlossinessInternal = 0.5f;

		public float flaskGlossinessInternal {
			get { return _flaskGlossinessInternal; }
			set {
				if (_flaskGlossinessInternal != value) {
					_flaskGlossinessInternal = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		bool _scatteringEnabled = false;

		public bool scatteringEnabled {
			get { return _scatteringEnabled; }
			set {
				if (_scatteringEnabled != value) {
					_scatteringEnabled = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (1, 9)]
		int _scatteringPower = 5;

		public int scatteringPower {
			get { return _scatteringPower; }
			set {
				if (_scatteringPower != value) {
					_scatteringPower = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1)]
		float _scatteringAmount = 0.3f;

		public float scatteringAmount {
			get { return _scatteringAmount; }
			set {
				if (_scatteringAmount != value) {
					_scatteringAmount = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		bool _refractionBlur = true;

		public bool refractionBlur {
			get { return _refractionBlur; }
			set {
				if (_refractionBlur != value) {
					_refractionBlur = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 1)]
		float _blurIntensity = 0.75f;

		public float blurIntensity {
			get { return _blurIntensity; }
			set {
				if (_blurIntensity != Mathf.Clamp01 (value)) {
					_blurIntensity = Mathf.Clamp01 (value);
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		int _liquidRaySteps = 10;

		public int liquidRaySteps {
			get { return _liquidRaySteps; }
			set {
				if (_liquidRaySteps != value) {
					_liquidRaySteps = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		int _foamRaySteps = 7;

		public int foamRaySteps {
			get { return _foamRaySteps; }
			set {
				if (_foamRaySteps != value) {
					_foamRaySteps = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		int _smokeRaySteps = 5;

		public int smokeRaySteps {
			get { return _smokeRaySteps; }
			set {
				if (_smokeRaySteps != value) {
					_smokeRaySteps = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		Texture2D _bumpMap;

		public Texture2D bumpMap {
			get { return _bumpMap; }
			set {
				if (_bumpMap != value) {
					_bumpMap = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 10f)]
		float _bumpDistortionScale = 1f;

		public float bumpDistortionScale {
			get { return _bumpDistortionScale; }
			set {
				if (_bumpDistortionScale != value) {
					_bumpDistortionScale = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		Vector2 _bumpDistortionOffset;

		public Vector2 bumpDistortionOffset {
			get { return _bumpDistortionOffset; }
			set {
				if (_bumpDistortionOffset != value) {
					_bumpDistortionOffset = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		Texture2D _distortionMap;

		public Texture2D distortionMap {
			get { return _distortionMap; }
			set {
				if (_distortionMap != value) {
					_distortionMap = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[SerializeField]
		Texture2D _texture;

		public Texture2D texture {
			get { return _texture; }
			set {
				if (_texture != value) {
					_texture = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		Vector2
			_textureScale = Vector2.one;

		public Vector2 textureScale {
			get { return _textureScale; }
			set {
				if (_textureScale != value) {
					_textureScale = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		Vector2
			_textureOffset;

		public Vector2 textureOffset {
			get { return _textureOffset; }
			set {
				if (_textureOffset != value) {
					_textureOffset = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0, 10f)]
		float
			_distortionAmount = 1f;

		public float distortionAmount {
			get { return _distortionAmount; }
			set {
				if (_distortionAmount != value) {
					_distortionAmount = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		bool _depthAware = false;

		public bool depthAware {
			get { return _depthAware; }
			set {
				if (_depthAware != value) {
					_depthAware = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		float _depthAwareOffset = 0f;

		public float depthAwareOffset {
			get { return _depthAwareOffset; }
			set {
				if (_depthAwareOffset != value) {
					_depthAwareOffset = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[SerializeField]
		bool _depthAwareCustomPass = false;

		public bool depthAwareCustomPass {
			get { return _depthAwareCustomPass; }
			set {
				if (_depthAwareCustomPass != value) {
					_depthAwareCustomPass = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[SerializeField]
		[Range (0, 5f)]
		float _doubleSidedBias = 0.05f;

		public float doubleSidedBias {
			get { return _doubleSidedBias; }
			set {
				if (_doubleSidedBias != value) {
					_doubleSidedBias = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[SerializeField]
		[Range (0, 1f)]
		float _rotationLevelBias = 0f;

		public float rotationLevelBias {
			get { return _rotationLevelBias; }
			set {
				if (_rotationLevelBias != value) {
					_rotationLevelBias = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[SerializeField]
		bool _ignoreGravity = false;

		public bool ignoreGravity {
			get { return _ignoreGravity; }
			set {
				if (_ignoreGravity != value) {
					_ignoreGravity = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[SerializeField]
		bool _reactToForces = false;

		public bool reactToForces {
			get { return _reactToForces; }
			set {
				if (_reactToForces != value) {
					_reactToForces = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[SerializeField]
		[Range (0, 1f)]
		float _textureAlpha = 1f;

		public float textureAlpha {
			get { return _textureAlpha; }
			set {
				if (_textureAlpha != value) {
					_textureAlpha = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		Vector3 _extentsScale = Vector3.one;

		public Vector3 extentsScale {
			get { return _extentsScale; }
			set {
				if (_extentsScale != value) {
					_extentsScale = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (1, 3)]
		int _noiseVariation = 1;

		public int noiseVariation {
			get { return _noiseVariation; }
			set {
				if (_noiseVariation != value) {
					_noiseVariation = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		bool _allowViewFromInside = false;

		public bool allowViewFromInside {
			get { return _allowViewFromInside; }
			set {
				if (_allowViewFromInside != value) {
					_allowViewFromInside = value;
					lastDistanceToCam = -1;
					CheckInsideOut ();
				}
			}
		}

		
		[SerializeField]
		bool
			_debugSpillPoint = false;

		public bool debugSpillPoint {
			get { return _debugSpillPoint; }
			set {
				if (_debugSpillPoint != value) {
					_debugSpillPoint = value;
				}
			}
		}

		[SerializeField]
		int
			_renderQueue = 3001;

		public int renderQueue {
			get { return _renderQueue; }
			set {
				if (_renderQueue != value) {
					_renderQueue = value;
					UpdateMaterialProperties ();
				}
			}
		}


		[SerializeField]
		Cubemap _reflectionTexture;

		public Cubemap reflectionTexture {
			get { return _reflectionTexture; }
			set {
				if (_reflectionTexture != value) {
					_reflectionTexture = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (1f, 5f)]
		float _physicsMass = 1f;

		public float physicsMass {
			get { return _physicsMass; }
			set {
				if (_physicsMass != value) {
					_physicsMass = value;
					UpdateMaterialProperties ();
				}
			}
		}

		[SerializeField]
		[Range (0.0f, 0.2f)]
		float _physicsAngularDamp = 0.02f;

		public float physicsAngularDamp {
			get { return _physicsAngularDamp; }
			set {
				if (_physicsAngularDamp != value) {
					_physicsAngularDamp = value;
					UpdateMaterialProperties ();
				}
			}
		}


		// ---- INTERNAL CODE ----
		const string SHADER_KEYWORD_DEPTH_AWARE = "LIQUID_VOLUME_DEPTH_AWARE";
		const string SHADER_KEYWORD_DEPTH_AWARE_CUSTOM_PASS = "LIQUID_VOLUME_DEPTH_AWARE_PASS";
		const string SHADER_KEYWORD_NON_AABB = "LIQUID_VOLUME_NON_AABB";
		const string SHADER_KEYWORD_IGNORE_GRAVITY = "LIQUID_VOLUME_IGNORE_GRAVITY";
		const string SHADER_KEYWORD_SPHERE = "LIQUID_VOLUME_SPHERE";
		const string SHADER_KEYWORD_CUBE = "LIQUID_VOLUME_CUBE";
		const string SHADER_KEYWORD_CYLINDER = "LIQUID_VOLUME_CYLINDER";
		const string SHADER_KEYWORD_IRREGULAR = "LIQUID_VOLUME_IRREGULAR";
		const string SHADER_KEYWORD_SMOKE = "LIQUID_VOLUME_SMOKE";
		const string SHADER_KEYWORD_SCATTERING = "LIQUID_VOLUME_SCATTERING";
		const string SPILL_POINT_GIZMO = "SpillPointGizmo";

		[SerializeField]
		Material liqMat;

		Material liqMatSimple, liqMatDefault, liqMatDefaultNoFlask, liqMatBump, liqMatReflections, liqMatSmoke, liqMatMultiple;
		Mesh mesh;
		Renderer mr;
		bool wasRefractionBlur, wasBackBuffer, wasFrontBuffer;
		Vector3 lastPosition, lastScale;
		Quaternion lastRotation;
		List<string> shaderKeywords;
		bool camInside;
		float lastDistanceToCam;
		DETAIL currentDetail;
		Vector4 turb;
		float turbulenceSpeed;
		float liquidLevelPos;
		bool shouldUpdateMaterialProperties;

		// Mesh info
		List<Vector3> vertices;
		float baseTopWidthRatio = 1f;

		// Physics
		Vector3 prevVelocity, prev2Velocity, inertia, lastAvgVelocity;
		float angularVelocity, angularInertia;
		float turbulenceDueForces;
		Quaternion liquidRot;

		float prevThickness;

		// Spill point debug
		GameObject spillPointGizmo;
		static string[] defaultContainerNames = new string[] {
			"GLASS",
			"CONTAINER",
			"BOTTLE",
			"POTION",
			"FLASK",
			"LIQUID"
		};

		#region Gameloop events

		void OnEnable () {
			if (!gameObject.activeInHierarchy)
				return;
			turb.z = 1f;
			turbulenceDueForces = 0f;
			turbulenceSpeed = 1f;
			liquidRot = transform.rotation;
			currentDetail = _detail;
			lastPosition = transform.position;
			lastRotation = transform.rotation;
			lastScale = transform.localScale;
			prevThickness = _flaskThickness;
			if (_depthAwareCustomPass && transform.parent == null) {
				_depthAwareCustomPass = false;
			}
			CleanupCommandBuffer ();
			RefreshMaterialProperties ();
		}

		void Reset () {
			// Try to assign propert topology based on mesh
			if (mesh == null)
				return;

			if (mesh.vertexCount == 24) {
				topology = TOPOLOGY.Cube;
			} else {
				Renderer renderer = GetComponent<Renderer> ();
				if (renderer == null) {
					if (mesh.bounds.extents.y > mesh.bounds.extents.x) {
						topology = TOPOLOGY.Cylinder;
					}
				} else if (renderer.bounds.extents.y > renderer.bounds.extents.x) {
					topology = TOPOLOGY.Cylinder;
					if (!Application.isPlaying) {
						if (transform.rotation.eulerAngles != Vector3.zero && (mesh.bounds.extents.y <= mesh.bounds.extents.x || mesh.bounds.extents.y <= mesh.bounds.extents.z)) {
							Debug.LogWarning ("Intrinsic model rotation detected. Consider using the Bake Transform and/or Center Pivot options in Advanced section.");
						}
					}

				}
			}
		}


		void OnDestroy () {
			CleanupCommandBuffer ();
			liqMat = null;
			if (liqMatDefault != null) {
				DestroyImmediate (liqMatDefault);
				liqMatDefault = null;
			}
			if (liqMatDefaultNoFlask != null) {
				DestroyImmediate (liqMatDefaultNoFlask);
				liqMatDefaultNoFlask = null;
			}
			if (liqMatSimple != null) {
				DestroyImmediate (liqMatSimple);
				liqMatSimple = null;
			}
			if (liqMatBump != null) {
				DestroyImmediate (liqMatBump);
				liqMatBump = null;
			}
			if (liqMatReflections != null) {
				DestroyImmediate (liqMatReflections);
				liqMatReflections = null;
			}
		}

		public void OnWillRenderObject () {
			var act = gameObject.activeInHierarchy && enabled;

			if (shouldUpdateMaterialProperties) {
				shouldUpdateMaterialProperties = false;
				RefreshMaterialProperties ();
			}

			if (act && _depthAware) {
				Camera.current.depthTextureMode |= DepthTextureMode.Depth;
			}

			if (act && _allowViewFromInside) {
				CheckInsideOut ();
			}

			if (!act || (!_refractionBlur && wasRefractionBlur)) {
				LiquidVolume.CleanupRefractionBuffer ();
				wasRefractionBlur = false;
			} else if (_refractionBlur) {
				LiquidVolume.SetupRefractionBuffer ();
				wasRefractionBlur = true;
			}

			UpdateAnimations ();

			if (!act || (_topology != TOPOLOGY.Irregular && wasBackBuffer)) {
				LiquidVolume.CleanupBackFacesBuffer ();
				wasBackBuffer = false;
			} else if (_topology == TOPOLOGY.Irregular) {
				LiquidVolume.SetupBackFacesBuffer (GetComponent<Renderer> ());
				wasBackBuffer = true;
			}

			if (!act || (!_depthAwareCustomPass && wasFrontBuffer)) {
				LiquidVolume.CleanupFrontFacesBuffer ();
				wasFrontBuffer = false;
			} else if (_depthAwareCustomPass) {
				Transform parent = transform.parent;
				if (parent != null) {
					Renderer parentRenderer = parent.GetComponent<Renderer> ();
					if (parentRenderer != null) {
						LiquidVolume.SetupFrontFacesBuffer (parentRenderer);
						wasFrontBuffer = true;
					}
				}
			}
			if (_debugSpillPoint) {
				UpdateSpillPointGizmo ();
			}
		}

		void FixedUpdate () {
			turbulenceSpeed += Time.deltaTime * 3f * _speed;
			liqMat.SetFloat ("_TurbulenceSpeed", turbulenceSpeed * 4f);
		}


		void OnDidApplyAnimationProperties () {	// support for animating property based fields
			shouldUpdateMaterialProperties = true;
		}

		void OnDisable () {
			CleanupCommandBuffer ();
		}

		#endregion

		#region Internal stuff

		void ReadVertices () {
			if (mesh == null)
				return;
			if (vertices == null) {
				vertices = new List<Vector3> (mesh.vertices);
			} else {
				vertices.Clear ();
				vertices.AddRange (mesh.vertices);
			}
			vertices.Sort ((Vector3 v0, Vector3 v1) => {
				return v1.y.CompareTo (v0.y);
			});

			// Compute base cap with top cap ratio
			int vertexCount = vertices.Count;
			float maxBaseDist = 0.00001f;
			float baseY = mesh.bounds.min.y + mesh.bounds.size.y * 0.15f;
			for (int k = 0; k < vertexCount; k++) {
				if (vertices [k].y < baseY) {
					float dist = vertices [k].magnitude;
					if (dist > maxBaseDist)
						maxBaseDist = dist;
				}
			}
			float maxBaseTop = 0.00001f;
			float topY = mesh.bounds.min.y + mesh.bounds.size.y * 0.85f;
			for (int k = 0; k < vertexCount; k++) {
				if (vertices [k].y > topY) {
					float dist = vertices [k].magnitude;
					if (dist > maxBaseTop)
						maxBaseTop = dist;
				}
			}
			baseTopWidthRatio = maxBaseDist / maxBaseTop;
		}


		void UpdateAnimations () {
			// Check proper scale
			switch (topology) {
			case TOPOLOGY.Sphere:
				if (transform.localScale.y != transform.localScale.x || transform.localScale.z != transform.localScale.x)
					transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.x, transform.localScale.x);
				break;
			case TOPOLOGY.Cylinder:
				if (transform.localScale.z != transform.localScale.x)
					transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y, transform.localScale.x);
				break;
			}


			if (liqMat != null) {
				Vector3 turbDir = Vector3.right;
				Quaternion rot = transform.rotation;
				if (_reactToForces) {
					Quaternion instantRot = transform.rotation;
					float dt = Time.smoothDeltaTime;
					if (Application.isPlaying && dt > 0) {
						Vector3 instantVelocity = (transform.position - lastPosition) / dt;
						Vector3 avgVelocity = (prev2Velocity + prevVelocity + instantVelocity) / 3f;
						prev2Velocity = prevVelocity;
						prevVelocity = instantVelocity;

						Vector3 instantAccel = (avgVelocity - lastAvgVelocity);
						lastAvgVelocity = avgVelocity;
						inertia += avgVelocity;

						float accelMag = instantAccel.magnitude;
						float force = Mathf.Max (accelMag / _physicsMass - _physicsAngularDamp, 0f);
						angularInertia += force;
						angularVelocity += angularInertia;
						if (angularVelocity > 0) {
							angularInertia -= Mathf.Abs (angularVelocity) * _physicsMass / 100f;
						} else if (angularVelocity < 0) {
							angularInertia += Mathf.Abs (angularVelocity) * _physicsMass / 100f;
						}
						float damp = 1f - _physicsAngularDamp;
						angularInertia *= damp;
						inertia *= damp;

						float mag = Mathf.Clamp (angularVelocity, -90f, 90f);
						turbDir = inertia.normalized;
						Vector3 axis = Vector3.Cross (turbDir, Vector3.down);
						instantRot = Quaternion.AngleAxis (mag, axis);

						float cinematic = Mathf.Abs (angularInertia) + Mathf.Abs (angularVelocity);
						turbulenceDueForces = Mathf.Min (0.5f / _physicsMass, turbulenceDueForces + cinematic / 1000f);
						turbulenceDueForces *= damp;
					} else {
						turbulenceDueForces = 0;
					}

					if (_topology == TOPOLOGY.Sphere) {
						liquidRot = Quaternion.Lerp (liquidRot, instantRot, 0.1f);
						rot = liquidRot;
					}
				}
				Matrix4x4 m = Matrix4x4.TRS (Vector3.zero, rot, Vector3.one);
				liqMat.SetMatrix ("_Rot", m.inverse);
				if (_topology != TOPOLOGY.Sphere) {
					float tx = turbDir.x;
					turbDir.x += (turbDir.z - turbDir.x) * 0.25f;
					turbDir.z += (tx - turbDir.z) * 0.25f;
				}
				turb.z = turbDir.x;
				turb.w = turbDir.z;
			}

			if (_reactToForces || transform.position != lastPosition || transform.localScale != lastScale || transform.rotation != lastRotation) {
				UpdateLevels ();
			}
		}

		public void UpdateMaterialProperties () {
			if (Application.isPlaying) {
				shouldUpdateMaterialProperties = true;
			} else {
				RefreshMaterialProperties ();
			}

		}

		void RefreshMaterialProperties () {
			if (!gameObject.activeInHierarchy)
				return;

			switch (_detail) {
			case DETAIL.Simple:
				if (liqMatSimple == null) {
					liqMatSimple = Instantiate (Resources.Load<Material> ("Materials/LiquidVolumeSimple")) as Material;
				}
				liqMat = liqMatSimple;
				break;
			case DETAIL.DefaultNoFlask:
				if (liqMatDefaultNoFlask == null) {
					liqMatDefaultNoFlask = Instantiate (Resources.Load<Material> ("Materials/LiquidVolumeDefaultNoFlask")) as Material;
				}
				liqMat = liqMatDefaultNoFlask;
				break;
			case DETAIL.BumpTexture:
				if (liqMatBump == null) {
					liqMatBump = Instantiate (Resources.Load<Material> ("Materials/LiquidVolumeBump")) as Material;
				}
				liqMat = liqMatBump;
				break;
			case DETAIL.Reflections:
				if (liqMatReflections == null) {
					liqMatReflections = Instantiate (Resources.Load<Material> ("Materials/LiquidVolumeReflections")) as Material;
				}
				liqMat = liqMatReflections;
				break;
			case DETAIL.Smoke:
				if (liqMatSmoke == null) {
					liqMatSmoke = Instantiate (Resources.Load<Material> ("Materials/LiquidVolumeSmoke")) as Material;
				}
				liqMat = liqMatSmoke;
				break;
			default:
				if (liqMatDefault == null) {
					liqMatDefault = Instantiate (Resources.Load<Material> ("Materials/LiquidVolumeDefault")) as Material;
				}
				liqMat = liqMatDefault;
				break;
			}

			if (liqMat == null)
				return;

            CheckMeshDisplacement();

			_level = Mathf.Clamp01 (_level);

			UpdateLevels ();
			if (mr == null)
				return;

			// Try to compute submesh index heuristically if this is the first time the liquid has been added to a multi-material mesh
			if (mr.sharedMaterials != null) {
				if (_subMeshIndex < 0) {
					for (int w = 0; w < defaultContainerNames.Length; w++) {
						if (_subMeshIndex >= 0)
							break;
						for (int k = 0; k < mr.sharedMaterials.Length; k++) {
							if (mr.sharedMaterials [k] != null && mr.sharedMaterials [k].name.ToUpper ().Contains (defaultContainerNames [w])) {
								_subMeshIndex = k;
								break;
							}
						}
					}
				}
			}
			if (_subMeshIndex < 0)
				_subMeshIndex = 0;

			if (mr.sharedMaterials != null && mr.sharedMaterials.Length > 1 && _subMeshIndex >= 0 && _subMeshIndex < mr.sharedMaterials.Length) {
				Material[] mats = mr.sharedMaterials;
				mats [_subMeshIndex] = liqMat;
				mr.sharedMaterials = mats;
			} else {
				mr.sharedMaterial = liqMat;
			}

			if (currentDetail != _detail) {
				currentDetail = _detail;
				if (_detail == DETAIL.Reflections) {
					_flaskTint = Color.white;
					_flaskGlossinessExternal = 0.1f;
				} else {
					_flaskTint = new Color (0, 0, 0, 1f);
				}
			}

			liqMat.SetColor ("_Color1", ApplyGlobalAlpha (_liquidColor1));
			liqMat.SetColor ("_Color2", ApplyGlobalAlpha (_liquidColor2));
			liqMat.SetColor ("_EmissionColor", _emissionColor * _emissionBrightness);

			liqMat.SetFloat ("_Glossiness", _flaskGlossinessExternal);
			liqMat.SetVector ("_GlossinessInt", new Vector3 (_flaskGlossinessInternal * 96f + 1f, Mathf.Pow (2, _scatteringPower), _scatteringAmount));
			liqMat.SetFloat ("_DoubleSidedBias", _doubleSidedBias);

			liqMat.SetFloat ("_Muddy", _murkiness);
			liqMat.SetFloat ("_Alpha", _alpha);

			float alphaCombined = _alpha * Mathf.Clamp01 ((_liquidColor1.a + _liquidColor2.a) * 4f);
			if (_ditherShadows) {
				liqMat.SetFloat ("_AlphaCombined", alphaCombined);
			} else {
				liqMat.SetFloat ("_AlphaCombined", alphaCombined > 0 ? 1000f : 0f);
			}

			liqMat.SetFloat ("_SparklingIntensity", _sparklingIntensity * 250.0f);
			liqMat.SetFloat ("_SparklingThreshold", 1.0f - _sparklingAmount);
			liqMat.SetFloat ("_DeepAtten", _deepObscurance);
			liqMat.SetColor ("_SmokeColor", ApplyGlobalAlpha (_smokeColor));
			liqMat.SetFloat ("_SmokeAtten", _smokeBaseObscurance);
			liqMat.SetFloat ("_SmokeSpeed", _smokeSpeed);
			liqMat.SetFloat ("_LiquidRaySteps", _liquidRaySteps);
			liqMat.SetFloat ("_SmokeRaySteps", _smokeRaySteps);
			liqMat.SetFloat ("_FlaskBlurIntensity", _blurIntensity * (_refractionBlur ? 1f : 0f));
			liqMat.SetColor ("_FlaskTint", _flaskTint * _flaskTint.a);

			liqMat.SetColor ("_FoamColor", ApplyGlobalAlpha (_foamColor));
			liqMat.SetFloat ("_FoamRaySteps", _foamRaySteps);
			liqMat.SetFloat ("_FoamDensity", _foamDensity);
			liqMat.SetFloat ("_FoamWeight", _foamWeight);
			liqMat.SetFloat ("_FoamBottom", _foamVisibleFromBottom ? 1f : 0f);
			liqMat.SetFloat ("_FoamTurbulence", _foamTurbulence);

			liqMat.SetFloat ("_FlaskTexAlpha", _textureAlpha);

			if (_detail == DETAIL.BumpTexture) {
				liqMat.SetTexture ("_BumpMap", _bumpMap);
				liqMat.SetTextureScale ("_BumpMap", Vector2.one * _bumpDistortionScale);
				liqMat.SetTextureOffset ("_BumpMap", _bumpDistortionOffset);
				liqMat.SetTexture ("_DispMap", _distortionMap);
				liqMat.SetTextureScale ("_DispMap", Vector2.one * _bumpDistortionScale);
				liqMat.SetTextureOffset ("_DispMap", _bumpDistortionOffset);
				liqMat.SetFloat ("_DispAmount", _distortionAmount);
				liqMat.SetTexture ("_FlaskTex", _texture);
				liqMat.SetTextureScale ("_FlaskTex", _textureScale);
				liqMat.SetTextureOffset ("_FlaskTex", _textureOffset);
			}

			if (_detail == DETAIL.Reflections) {
				if (_reflectionTexture == null) {
					_reflectionTexture = Resources.Load<Cubemap> ("Textures/Reflections");
				}
				liqMat.SetTexture ("_RefractTex", _reflectionTexture);
			}

			Texture3D tex3d = Resources.Load<Texture3D> ("Textures/Noise3D" + _noiseVariation.ToString ());
			if (tex3d != null) {
				liqMat.SetTexture ("_NoiseTex", tex3d);
			}

			liqMat.renderQueue = _renderQueue;
			UpdateInsideOut ();

			if (_topology == TOPOLOGY.Irregular && prevThickness != _flaskThickness) {
				prevThickness = _flaskThickness;
				CleanupBackFacesBuffer ();
			}


		}

		Color ApplyGlobalAlpha (Color originalColor) {
			return new Color (originalColor.r, originalColor.g, originalColor.b, originalColor.a * _alpha);
		}


		void UpdateLevels () {
			if (liqMat == null)
				return;

			if (mesh == null) {
				MeshFilter mf = GetComponent<MeshFilter> ();
				if (mf != null) {
					mesh = mf.sharedMesh;
					mr = GetComponent<MeshRenderer> ();
				} else {
					SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer> ();
					if (smr != null) {
						mesh = smr.sharedMesh;
						mr = smr;
					}
				}
				ReadVertices ();
			}
			if (mesh == null || mr == null) {
				return;
			}

			Vector4 size = new Vector4 (mesh.bounds.extents.x * 2f * transform.lossyScale.x, mesh.bounds.extents.y * 2f * transform.lossyScale.y, mesh.bounds.extents.z * 2f * transform.lossyScale.z, 0);
			size.x *= _extentsScale.x;
			size.y *= _extentsScale.y;
			size.z *= _extentsScale.z;
			float maxWidth = Mathf.Max (size.x, size.z);

			Vector3 extents = _ignoreGravity ? new Vector3 (size.x * 0.5f, size.y * 0.5f, size.z * 0.5f) : mr.bounds.extents;
			extents *= (1f - _flaskThickness);
			extents.x *= _extentsScale.x;
			extents.y *= _extentsScale.y;
			extents.z *= _extentsScale.z;

			// Compensate levelpos with upperlimit
			float rotationAdjustment;
			if (_upperLimit < 1f && !_ignoreGravity) {
				float y1 = transform.TransformPoint (Vector3.up * extents.y).y;
				float y0 = transform.TransformPoint (Vector3.up * (extents.y * _upperLimit)).y;
				rotationAdjustment = Mathf.Max (y0 - y1, 0);
			} else {
				rotationAdjustment = 0;
			}

			// Compensate rotation in cylindrical shapes where mesh height is on another scale than width
			float thisLevel = _level;
			if (!_ignoreGravity && _rotationLevelBias > 0 && thisLevel > 0) {

				// Compute reference area without rotation
				float w = maxWidth * (1f - _flaskThickness);
				float sy = size.y * (1f - _flaskThickness);
				float h = sy * _level;
				float refArea = w * h;

				// Guess appropiate level in rotated flask
				Vector3 me = mesh.bounds.extents * (1f - _flaskThickness);
				Vector3 tl = transform.TransformPoint (new Vector3 (-me.x, me.y, 0));
				Vector3 tr = transform.TransformPoint (new Vector3 (me.x, me.y, 0));
				Vector3 bl = transform.TransformPoint (new Vector3 (-me.x * baseTopWidthRatio, -me.y, 0));
				Vector3 br = transform.TransformPoint (new Vector3 (me.x * baseTopWidthRatio, -me.y, 0));
				if (tl.y < bl.y || tr.y < br.y) {
					me.y *= -1f;
					tl = transform.TransformPoint (new Vector3 (-me.x, me.y, 0));
					tr = transform.TransformPoint (new Vector3 (me.x, me.y, 0));
					bl = transform.TransformPoint (new Vector3 (-me.x * baseTopWidthRatio, -me.y, 0));
					br = transform.TransformPoint (new Vector3 (me.x * baseTopWidthRatio, -me.y, 0));
				}
				float maxLevel = Mathf.Clamp01 (_level + 0.5f);
				float minLevel = Mathf.Clamp01 (_level - 0.5f);
				float resArea = 0;
				for (int i = 0; i < 12; i++) {
					thisLevel = (minLevel + maxLevel) * 0.5f;
					// Supposed liquid level pos?
					liquidLevelPos = transform.position.y - extents.y + extents.y * 2f * thisLevel;
					// Choose case
					if (bl.y <= liquidLevelPos && br.y <= liquidLevelPos) {
						// Case a) both bottom points are below surface level
						// Line intersection with liquid level
						Vector3 ll = GetIntersection (bl, tl, liquidLevelPos);
						Vector3 lr = GetIntersection (br, tr, liquidLevelPos);
						// 2 triangles
						if (bl.y < br.y) {
							resArea = GetTriangleArea (ll, lr, bl) + GetTriangleArea (lr, bl, br);
						} else {
							resArea = GetTriangleArea (ll, lr, br) + GetTriangleArea (ll, bl, br);
						}
					} else {
						// Case b) one of the bottom points are above surface
						// 1 triangle
						if (bl.y < br.y) {
							Vector3 ll = GetIntersection (bl, tl, liquidLevelPos);
							Vector3 lr = GetIntersection (bl, br, liquidLevelPos);
							resArea = GetTriangleArea (ll, lr, bl);
						} else {
							Vector3 ll = GetIntersection (br, tr, liquidLevelPos);
							Vector3 lr = GetIntersection (bl, br, liquidLevelPos);
							resArea = GetTriangleArea (ll, lr, br);
						}
					}
					if (resArea < refArea) {
						minLevel = thisLevel;
					} else {
						if (i >= 8)
							break;
						maxLevel = thisLevel;
					}
				}
				thisLevel = Mathf.Lerp (_level, thisLevel, _rotationLevelBias);
			} else {
				if (_level <= 0)
					thisLevel = -0.001f; // ensure it's below the flask thickness
			}
				
			liquidLevelPos = transform.position.y - extents.y;
			if (_detail != DETAIL.Smoke) {
				liquidLevelPos += extents.y * 2f * thisLevel + rotationAdjustment;
			}
			liqMat.SetFloat ("_LevelPos", liquidLevelPos);
			float upperLimit = mesh.bounds.extents.y * _extentsScale.y * _upperLimit;
			liqMat.SetFloat ("_UpperLimit", upperLimit);
			float lowerLimit = mesh.bounds.extents.y * _extentsScale.y * _lowerLimit;
			liqMat.SetFloat ("_LowerLimit", lowerLimit);
			float visibleLevel = (_level <= 0 || _level >= 1f) ? 0f : 1f;
			UpdateTurbulence ();
			float foamPos = transform.position.y - extents.y + (rotationAdjustment + extents.y * 2.0f * (thisLevel + _foamThickness)) * visibleLevel;
			liqMat.SetFloat ("_FoamMaxPos", foamPos);
			Vector3 thickness = new Vector3 (1.0f - _flaskThickness, (1.0f - _flaskThickness * maxWidth / size.z), (1.0f - _flaskThickness * maxWidth / size.z));
			liqMat.SetVector ("_FlaskThickness", thickness);
			size.w = size.x * 0.5f * thickness.x;
			liqMat.SetVector ("_Size", size);
			float scaleFactor = size.y * 0.5f * (1.0f - _flaskThickness * maxWidth / size.y);
			liqMat.SetVector ("_Scale", new Vector4 (_smokeScale / scaleFactor, _foamScale / scaleFactor, _liquidScale1 / scaleFactor, _liquidScale2 / scaleFactor));
			liqMat.SetVector ("_Center", transform.position);

			if (shaderKeywords == null) {
				shaderKeywords = new List<string> ();
			} else {
				shaderKeywords.Clear ();
			}

			if (_depthAware) {
				shaderKeywords.Add (SHADER_KEYWORD_DEPTH_AWARE);
				liqMat.SetFloat ("_DepthAwareOffset", _depthAwareOffset);
			}

			if (_depthAwareCustomPass) {
				shaderKeywords.Add (SHADER_KEYWORD_DEPTH_AWARE_CUSTOM_PASS);
			}

			if (_reactToForces && _topology == TOPOLOGY.Sphere) {
				shaderKeywords.Add (SHADER_KEYWORD_IGNORE_GRAVITY);
			} else if (_ignoreGravity) {
				shaderKeywords.Add (SHADER_KEYWORD_IGNORE_GRAVITY);
			} else if (transform.rotation.eulerAngles != Vector3.zero) {
				shaderKeywords.Add (SHADER_KEYWORD_NON_AABB);
			}
			switch (_topology) {
			case TOPOLOGY.Sphere:
				shaderKeywords.Add (SHADER_KEYWORD_SPHERE);
				break;
			case TOPOLOGY.Cube:
				shaderKeywords.Add (SHADER_KEYWORD_CUBE);
				break;
			case TOPOLOGY.Cylinder:
				shaderKeywords.Add (SHADER_KEYWORD_CYLINDER);
				break;
			default:
				shaderKeywords.Add (SHADER_KEYWORD_IRREGULAR);
				break;
			}
			if (_smokeEnabled && _smokeColor.a > 0)
				shaderKeywords.Add (SHADER_KEYWORD_SMOKE);
			if (_scatteringEnabled)
				shaderKeywords.Add (SHADER_KEYWORD_SCATTERING);

			liqMat.shaderKeywords = shaderKeywords.ToArray ();

			lastPosition = transform.position;
			lastScale = transform.localScale;
			lastRotation = transform.rotation;
		}

		Vector3 GetIntersection (Vector3 p0, Vector3 p1, float y) {
			float t = (y - p0.y) / (p1.y - p0.y);
			return p0 + t * (p1 - p0);
		}

		float GetTriangleArea (Vector3 p0, Vector3 p1, Vector3 p2) {
			Vector3 ab = p1 - p0;
			Vector3 ac = p2 - p0;
			return Vector3.Cross (ab, ac).magnitude * 0.5f;
		}

		void UpdateTurbulence () {
			if (liqMat == null)
				return;
			float visibleLevel = 1f; // (_level<=0 || _level>=1f) ? 0.1f: 1f;	// commented out to allow animation even level is 0 or full
			float isInsideContainer = (camInside && _allowViewFromInside) ? 0f : 1f;
			turb.x = _turbulence1 * visibleLevel * isInsideContainer;
			turb.y = Mathf.Max (_turbulence2, turbulenceDueForces) * visibleLevel * isInsideContainer;
			Vector4 shaderTurb = turb;
			shaderTurb.z *= 3.1415927f * _frecuency * 4f;
			shaderTurb.w *= 3.1415927f * _frecuency * 4f;
			liqMat.SetVector ("_Turbulence", shaderTurb);
		}

		void CheckInsideOut () {
			if (Camera.current == null || mr == null) {
				if (!_allowViewFromInside)
					UpdateInsideOut ();
				return;
			}

			Vector3 currentCamPos = Camera.current.transform.position;
			float currentDistanceToCam = (currentCamPos - transform.position).sqrMagnitude;
			if (currentDistanceToCam == lastDistanceToCam)
				return;
			lastDistanceToCam = currentDistanceToCam;

			// Check if position is inside container
			bool nowInside = false;
			switch (_topology) {
			case TOPOLOGY.Cube:
				nowInside = PointInAABB (currentCamPos);
				break;
			case TOPOLOGY.Cylinder:
				nowInside = PointInCylinder (currentCamPos);
				break;
			default:
				float diam = mesh.bounds.extents.x * 2f;
				nowInside = (currentCamPos - transform.position).sqrMagnitude < (diam * diam);
				break;
			}

			if (nowInside != camInside) {
				camInside = nowInside;
				UpdateInsideOut ();
			}
		}


		bool PointInAABB (Vector3 point) {
			point = transform.InverseTransformPoint (point);
			Vector3 ext = mesh.bounds.extents;
			if (point.x < ext.x && point.x > -ext.x &&
			    point.y < ext.y && point.y > -ext.y &&
			    point.z < ext.z && point.z > -ext.z) {
				return true;
			} else {
				return false;
			}
		}

		bool PointInCylinder (Vector3 point) {
			point = transform.InverseTransformPoint (point);
			Vector3 ext = mesh.bounds.extents;
			if (point.x < ext.x && point.x > -ext.x &&
			    point.y < ext.y && point.y > -ext.y &&
			    point.z < ext.z && point.z > -ext.z) {

				point.y = 0;
				Vector3 currentPos = transform.position;
				currentPos.y = 0;
				return (point - currentPos).sqrMagnitude < ext.x * ext.x;
			}
			return false;
		}


		void UpdateInsideOut () {
			if (liqMat == null)
				return;
			if (_allowViewFromInside && camInside) {
				liqMat.SetInt ("_CullMode", (int)UnityEngine.Rendering.CullMode.Front);
				liqMat.SetInt ("_ZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);
			} else {
				liqMat.SetInt ("_CullMode", (int)UnityEngine.Rendering.CullMode.Back);
				liqMat.SetInt ("_ZTestMode", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
			}
			UpdateTurbulence ();
		}

		#endregion


		#region CommandBuffer setup

		private void CleanupCommandBuffer () {
			LiquidVolume.CleanupRefractionBuffer ();
			LiquidVolume.CleanupBackFacesBuffer ();
			LiquidVolume.CleanupFrontFacesBuffer ();
		}

		static Dictionary<Camera, CommandBuffer> m_CamerasBlur = new Dictionary<Camera, CommandBuffer> ();
		static Material blurMat;

		internal static void CleanupRefractionBuffer () {
			foreach (var cam in m_CamerasBlur) {
				if (cam.Key) {
					cam.Key.RemoveCommandBuffer (CameraEvent.AfterImageEffectsOpaque, cam.Value);
				}
			}
			m_CamerasBlur.Clear ();
			if (blurMat != null) {
				DestroyImmediate (blurMat);
				blurMat = null;
			}
		}

		internal static void SetupRefractionBuffer () {
			var cam = Camera.current;
			if (!cam)
				return;

			CommandBuffer buf = null;
			if (m_CamerasBlur.ContainsKey (cam))
				return;

			if (blurMat == null) {
				blurMat = Instantiate (Resources.Load<Material> ("Materials/LiquidVolumeBlur")) as Material;
				blurMat.hideFlags = HideFlags.DontSave;
			}

			buf = new CommandBuffer ();
			buf.name = "Volumetric Liquid Background Blur";
			m_CamerasBlur [cam] = buf;

			int screenCopyID = Shader.PropertyToID ("_VLScreenCopyTexture");
			buf.GetTemporaryRT (screenCopyID, -1, -1, 0, FilterMode.Bilinear);
			buf.Blit (BuiltinRenderTextureType.CurrentActive, screenCopyID);

			int blurredID = Shader.PropertyToID ("_VLTemp1");
			int blurredID2 = Shader.PropertyToID ("_VLTemp2");
			buf.GetTemporaryRT (blurredID, -2, -2, 0, FilterMode.Bilinear);
			buf.GetTemporaryRT (blurredID2, -2, -2, 0, FilterMode.Bilinear);

			buf.Blit (screenCopyID, blurredID2, blurMat, 0);
			buf.ReleaseTemporaryRT (screenCopyID);

			buf.Blit (blurredID2, blurredID, blurMat, 1);
			buf.ReleaseTemporaryRT (blurredID2);

			buf.SetGlobalTexture ("_VLGrabBlurTexture", blurredID);

			cam.AddCommandBuffer (CameraEvent.AfterImageEffectsOpaque, buf);

		}

		#endregion

		#region Backbuffer

		static Dictionary<Camera, CommandBuffer> m_CamerasBackBuffer = new Dictionary<Camera, CommandBuffer> ();
		static Material backBufferMat;
		static List<Renderer> bbRenderers = new List<Renderer> ();

		internal static void SetupBackFacesBuffer (Renderer renderer) {
			var cam = Camera.current;
			if (!cam || renderer == null)
				return;

			if (!bbRenderers.Contains (renderer))
				CleanupBackFacesCameras ();

			CommandBuffer buf = null;
			if (m_CamerasBackBuffer.ContainsKey (cam)) {
				return;
			}

			if (backBufferMat == null) {
				backBufferMat = new Material (Shader.Find ("LiquidVolume/ZWriteBack")) as Material;
				backBufferMat.hideFlags = HideFlags.DontSave;
			}

			bbRenderers.Add (renderer);
			buf = new CommandBuffer ();
			buf.name = "Volumetric Liquid BackBuffer";
			m_CamerasBackBuffer [cam] = buf;

			int backBufferID = Shader.PropertyToID ("_VLBackBufferTexture");
			buf.GetTemporaryRT (backBufferID, -1, -1, 24, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			buf.SetRenderTarget (backBufferID);
			buf.ClearRenderTarget (true, true, new Color (0.9882353f, 0.4470558f, 0.75f, 0f), 0f);
			bbRenderers.ForEach ((Renderer obj) => {
				if (obj != null && obj.gameObject.activeSelf) {
					backBufferMat.SetFloat ("_FlaskThickness", 1.0f - obj.GetComponent<LiquidVolume> ().flaskThickness);
					buf.DrawRenderer (obj, backBufferMat);
				}
			});
			cam.AddCommandBuffer (CameraEvent.AfterImageEffectsOpaque, buf);
		}

		internal static void CleanupBackFacesBuffer () {
			CleanupBackFacesCameras ();
			if (backBufferMat != null) {
				DestroyImmediate (backBufferMat);
				backBufferMat = null;
			}
			bbRenderers.Clear ();
		}


		internal static void CleanupBackFacesCameras () {
			foreach (var cam in m_CamerasBackBuffer) {
				if (cam.Key) {
					cam.Key.RemoveCommandBuffer (CameraEvent.AfterImageEffectsOpaque, cam.Value);
				}
			}
			m_CamerasBackBuffer.Clear ();
		}

		#endregion

		#region FrontBuffer

		static Dictionary<Camera, CommandBuffer> m_CamerasFrontBuffer = new Dictionary<Camera, CommandBuffer> ();
		static Material frontBufferMat;
		static List<Renderer> fbRenderers = new List<Renderer> ();

		internal static void SetupFrontFacesBuffer (Renderer renderer) {
			var cam = Camera.current;
			if (!cam)
				return;

			if (!fbRenderers.Contains (renderer))
				CleanupFrontFacesCameras ();

			CommandBuffer buf = null;
			if (m_CamerasFrontBuffer.ContainsKey (cam)) {
				return;
			}

			if (frontBufferMat == null) {
				frontBufferMat = new Material (Shader.Find ("LiquidVolume/ZWriteFront")) as Material;
				frontBufferMat.hideFlags = HideFlags.DontSave;
			}

			fbRenderers.Add (renderer);
			buf = new CommandBuffer ();
			buf.name = "Volumetric Liquid FrontBuffer";
			m_CamerasFrontBuffer [cam] = buf;

			int frontBufferID = Shader.PropertyToID ("_VLFrontBufferTexture");
			buf.GetTemporaryRT (frontBufferID, -1, -1, 24, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			buf.SetRenderTarget (frontBufferID);
			buf.ClearRenderTarget (true, true, new Color (0.9882353f, 0.4470558f, 0.75f, 0f), 1f);
			fbRenderers.ForEach ((Renderer obj) => {
				buf.DrawRenderer (obj, frontBufferMat);
			});
			cam.AddCommandBuffer (CameraEvent.AfterImageEffectsOpaque, buf);
		}

		internal static void CleanupFrontFacesBuffer () {
			CleanupFrontFacesCameras ();
			if (frontBufferMat != null) {
				DestroyImmediate (frontBufferMat);
				frontBufferMat = null;
			}
			fbRenderers.Clear ();
		}


		internal static void CleanupFrontFacesCameras () {
			foreach (var cam in m_CamerasFrontBuffer) {
				if (cam.Key) {
					cam.Key.RemoveCommandBuffer (CameraEvent.AfterImageEffectsOpaque, cam.Value);
				}
			}
			m_CamerasFrontBuffer.Clear ();
		}

		#endregion



		#region Public API

		/// <summary>
		/// Returns the vertical position in world space coordinates of the liquid surface
		/// </summary>
		/// <value>The get liquid surface Y position.</value>
		public float liquidSurfaceYPosition {
			get {
				return liquidLevelPos;
			}
								
		}

		/// <summary>
		/// Computes approximate point where liquid starts pouring over the flask when it's rotated
		/// </summary>
		/// <returns><c>true</c>, if spill point is detected, <c>false</c> otherwise.</returns>
		/// <param name="spillPosition">Returned spill position in world space coordinates.</param>
		/// <param name="apertureStart">A value that determines where the aperture of the flask starts (0-1 where 0 is flask center and 1 is the very top).</param>
		public bool GetSpillPoint (out Vector3 spillPosition, float apertureStart = 1f) {
			float spillAmount = 0;
			return GetSpillPoint (out spillPosition, out spillAmount, apertureStart);
		}



		/// <summary>
		/// Computes approximate point where liquid starts pouring over the flask when it's rotated
		/// </summary>
		/// <returns><c>true</c>, if spill point is detected, <c>false</c> otherwise.</returns>
		/// <param name="spillPosition">Returned spill position in world space coordinates.</param>
		/// <param name="spillAmount">A returned value that represent the amount of liquid spilt.</param>
		/// <param name="apertureStart">A value that determines where the aperture of the flask starts (0-1 where 0 is flask center and 1 is the very top).</param>
		public bool GetSpillPoint (out Vector3 spillPosition, out float spillAmount, float apertureStart = 1f) {
			spillPosition = Vector3.zero;
			spillAmount = 0;
			if (mesh == null || vertices == null || _level <= 0)
				return false;

			float clampy = mesh.bounds.extents.y * apertureStart;
			Vector3 vt = transform.position;
			bool crossed = false;
			int verticesCount = vertices.Count;
			float miny = float.MaxValue;
			for (int k = 0; k < verticesCount; k++) {
				Vector3 vertex = vertices [k];
				if (vertex.y < clampy)
					break;
				vertex = transform.TransformPoint (vertex);
				if (vertex.y < liquidLevelPos && vertex.y < miny) {
					miny = vertex.y;
					vt = vertex;
					crossed = true;
				}
			}
			if (!crossed)
				return false;

			spillPosition = vt;
			spillAmount = (liquidLevelPos - vt.y) / (mesh.bounds.extents.y * 2f * transform.localScale.y);
			return true;
		}


		void UpdateSpillPointGizmo () {
			if (!_debugSpillPoint) {
				if (spillPointGizmo != null) {
					DestroyImmediate (spillPointGizmo.gameObject);
					spillPointGizmo = null;
				}
				return;
			} 

			if (spillPointGizmo == null) {
				Transform t = transform.Find (SPILL_POINT_GIZMO);
				if (t != null) {
					DestroyImmediate (t.gameObject);
				}
				spillPointGizmo = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				spillPointGizmo.name = SPILL_POINT_GIZMO;
				spillPointGizmo.transform.localScale = Vector3.one * 0.05f;
				spillPointGizmo.transform.SetParent (transform, true);
				Collider collider = spillPointGizmo.GetComponent<Collider> ();
				if (collider != null)
					DestroyImmediate (collider);
				MeshRenderer mr = spillPointGizmo.GetComponent<MeshRenderer> ();
				if (mr != null) {
					mr.sharedMaterial = Instantiate (mr.sharedMaterial);	// to avoid Editor (non playing) warning
					mr.sharedMaterial.hideFlags = HideFlags.DontSave;
					mr.sharedMaterial.color = Color.yellow;
				}
			}

			Vector3 spillPoint;
			if (GetSpillPoint (out spillPoint, 1f)) {
				spillPointGizmo.transform.position = spillPoint;
				spillPointGizmo.SetActive (true);
			} else {
				spillPointGizmo.SetActive (false);
			}

		}

		/// <summary>
		/// Applies current transform rotation and scale to the vertices and resets the transform rotation and scale to default values.
		/// This operation makes the game object transform point upright as normal game objects and is required for Liquid Volume to work on imported models that comes with a rotation
		/// </summary>
		public void BakeRotation () {

			MeshFilter mf = GetComponent<MeshFilter> ();
			Mesh mesh = Instantiate<Mesh> (mf.sharedMesh) as Mesh;
			Vector3[] vertices = mesh.vertices;
			Vector3 scale = transform.localScale;
			transform.localScale = Vector3.one;

			for (int k = 0; k < vertices.Length; k++) {
				vertices [k] = transform.TransformVector (vertices [k]);
			}
			mesh.vertices = vertices;
			mesh.RecalculateBounds ();
			mesh.RecalculateNormals ();
			mf.sharedMesh = mesh;

			transform.localRotation = Quaternion.Euler (0, 0, 0);
			transform.localScale = scale;

			RefreshCollider ();
		}

		/// <summary>
		/// This operation computes the geometric center of all vertices and displaces them so the pivot is centered in the model
		/// </summary>
		public void CenterPivot () {
            CenterPivot(Vector3.zero);
        }

        /// <summary>
        /// This operation computes the geometric center of all vertices and displaces them so the pivot is centered in the model
        /// </summary>
        public void CenterPivot(Vector3 offset) {
			MeshFilter mf = GetComponent<MeshFilter> ();
			Mesh mesh = Instantiate<Mesh> (mf.sharedMesh) as Mesh;
            mesh.name = mf.sharedMesh.name; // keep original name to detect if user assigns a different mesh to meshfilter and discard originalMesh reference
			Vector3[] vertices = mesh.vertices;

			Vector3 midPoint = Vector3.zero;
			for (int k = 0; k < vertices.Length; k++) {
				midPoint += vertices [k];
			}
			midPoint /= vertices.Length;
            midPoint += offset;
			for (int k = 0; k < vertices.Length; k++) {
				vertices [k] -= midPoint;
			}
			mesh.vertices = vertices;
			mesh.RecalculateBounds ();
			mf.sharedMesh = mesh;

			transform.localPosition += midPoint;

			RefreshCollider ();
		}

		public void RefreshCollider () {
			MeshCollider mc = GetComponent<MeshCollider> ();
			if (mc != null) {
				Mesh oldMesh = mc.sharedMesh;
				mc.sharedMesh = null;
				mc.sharedMesh = oldMesh;
			}
		}

		#endregion


#region Mesh Displacement

        void CheckMeshDisplacement() {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null) {
                originalMesh = null;
                return;
            }
            if (!_fixMesh) {
                RestoreOriginalMesh();
                originalMesh = null;
                return;
            }

            // Backup original mesh
            if (originalMesh == null || !(originalMesh.name.Equals(meshFilter.sharedMesh.name))) {
                originalMesh = meshFilter.sharedMesh;
            }

            if (meshFilter.sharedMesh != originalMesh) {
                RestoreOriginalMesh();
            }

            Vector3 pos = transform.localPosition;
            CenterPivot(_pivotOffset);
            originalPivotOffset = transform.localPosition - pos;
        }

        void RestoreOriginalMesh() {
            if (originalMesh == null) return;

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null) return;

            meshFilter.sharedMesh = originalMesh;
            transform.localPosition -= originalPivotOffset;
            RefreshCollider();
        }

#endregion

	}
}
