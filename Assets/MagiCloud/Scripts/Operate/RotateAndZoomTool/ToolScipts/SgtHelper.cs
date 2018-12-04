using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public static partial class SgtHelper
{
    public const string ShaderNamePrefix = "Hidden/Sgt";

    public const string ComponentMenuPrefix = "Space Graphics Toolkit/SGT ";

    public const string GameObjectMenuPrefix = "GameObject/Space Graphics Toolkit/";

    public static readonly float InscribedBox = 1.0f / Mathf.Sqrt(2.0f);

    public static readonly int QuadsPerMesh = 65000 / 4;

    private static List<Material> tempMaterials = new List<Material>();

    private static Stack<Random.State> seedStates = new Stack<Random.State>();

    public static void ClearCapacity<T>(List<T> list, int minCapacity)
    {
        if (list != null)
        {
            list.Clear();

            if (list.Capacity < minCapacity)
            {
                list.Capacity = minCapacity;
            }
        }
    }

    public static bool Enabled(Behaviour b)
    {
        return b != null && b.enabled == true && b.gameObject.activeInHierarchy == true;
    }

    public static T GetOrAddComponent<T>(GameObject gameObject, bool recordUndo = true)
        where T : Component
    {
        if (gameObject != null)
        {
            var component = gameObject.GetComponent<T>();

            if (component == null) component = AddComponent<T>(gameObject, recordUndo);

            return component;
        }

        return null;
    }

    public static T AddComponent<T>(GameObject gameObject, bool recordUndo = true)
        where T : Component
    {
        if (gameObject != null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == true)
            {
                return gameObject.AddComponent<T>();
            }
            else
            {
                if (recordUndo == true)
                {
                    return Undo.AddComponent<T>(gameObject);
                }
                else
                {
                    return gameObject.AddComponent<T>();
                }
            }
#else
			return gameObject.AddComponent<T>();
#endif
        }

        return null;
    }

    public static T Destroy<T>(T o)
        where T : Object
    {
        if (o != null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == true)
            {
                Object.Destroy(o);
            }
            else
            {
                Object.DestroyImmediate(o);
            }
#else
			Object.Destroy(o);
#endif
        }

        return null;
    }

    public static bool Zero(float v)
    {
        return Mathf.Approximately(v, 0.0f);
    }

    public static float Reciprocal(float v)
    {
        return Zero(v) == false ? 1.0f / v : 0.0f;
    }

    public static float Acos(float v)
    {
        if (v >= -1.0f && v <= 1.0f)
        {
            return Mathf.Acos(v);
        }

        return 0.0f;
    }

    public static Vector3 Reciprocal3(Vector3 xyz)
    {
        xyz.x = Reciprocal(xyz.x);
        xyz.y = Reciprocal(xyz.y);
        xyz.z = Reciprocal(xyz.z);
        return xyz;
    }

    public static float Divide(float a, float b)
    {
        return b != 0.0f ? a / b : 0.0f;
    }

    public static Vector4 NewVector4(Vector3 xyz, float w)
    {
        return new Vector4(xyz.x, xyz.y, xyz.z, w);
    }

    public static float Pow(float a, float p)
    {
        if (p >= 0.0f)
        {
            return Mathf.Pow(a, p);
        }
        else
        {
            return 1.0f - Mathf.Pow(1.0f - a, -p);
        }
    }

    public static float CubicInterpolate(float a, float b, float c, float d, float t)
    {
        var tt = t * t;

        d = (d - c) - (a - b);

        return d * (tt * t) + ((a - b) - d) * tt + (c - a) * t + b;
    }

    public static float HermiteInterpolate(float a, float b, float c, float d, float t)
    {
        var tt = t * t;
        var tt3 = tt * 3.0f;
        var ttt = t * tt;
        var ttt2 = ttt * 2.0f;
        float a0, a1, a2, a3;

        var m0 = (c - a) * 0.5f;
        var m1 = (d - b) * 0.5f;

        a0 = ttt2 - tt3 + 1.0f;
        a1 = ttt - tt * 2.0f + t;
        a2 = ttt - tt;
        a3 = -ttt2 + tt3;

        return a0 * b + a1 * m0 + a2 * m1 + a3 * c;
    }

    public static Color HermiteInterpolate(Color a, Color b, Color c, Color d, float t)
    {
        var tt = t * t;
        var tt3 = tt * 3.0f;
        var ttt = t * tt;
        var ttt2 = ttt * 2.0f;
        float a0, a1, a2, a3;

        var m0 = (c - a) * 0.5f;
        var m1 = (d - b) * 0.5f;

        a0 = ttt2 - tt3 + 1.0f;
        a1 = ttt - tt * 2.0f + t;
        a2 = ttt - tt;
        a3 = -ttt2 + tt3;

        return a0 * b + a1 * m0 + a2 * m1 + a3 * c;
    }

    public static Vector3 HermiteInterpolate3(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        var tt = t * t;
        var tt3 = tt * 3.0f;
        var ttt = t * tt;
        var ttt2 = ttt * 2.0f;
        float a0, a1, a2, a3;

        var m0 = (c - a) * 0.5f;
        var m1 = (d - b) * 0.5f;

        a0 = ttt2 - tt3 + 1.0f;
        a1 = ttt - tt * 2.0f + t;
        a2 = ttt - tt;
        a3 = -ttt2 + tt3;

        return a0 * b + a1 * m0 + a2 * m1 + a3 * c;
    }

    // This gives you the time-independent 't' value for lerp when used for dampening
    public static float DampenFactor(float dampening, float elapsed)
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            return 1.0f;
        }
#endif
        return 1.0f - Mathf.Exp(-dampening * elapsed);
    }

    public static float Dampen(float current, float target, float dampening, float elapsed)
    {
        var factor = DampenFactor(dampening, elapsed);

        return Mathf.LerpUnclamped(current, target, factor);
    }

    public static float Dampen(float current, float target, float dampening, float elapsed, float minStep)
    {
        current = Mathf.MoveTowards(current, target, minStep * elapsed);

        return Dampen(current, target, dampening, elapsed);
    }

    public static Quaternion Dampen(Quaternion current, Quaternion target, float dampening, float elapsed)
    {
        var factor = DampenFactor(dampening, elapsed);

        return Quaternion.Slerp(current, target, factor);
    }

    public static Quaternion Dampen(Quaternion current, Quaternion target, float dampening, float elapsed, float minStep)
    {
        var delta = Quaternion.Angle(current, target);
        var factor = DampenFactor(dampening, elapsed) + Divide(minStep * elapsed, delta);

        return Quaternion.Slerp(current, target, factor);
    }

    public static Vector3 Dampen3(Vector3 current, Vector3 target, float dampening, float elapsed)
    {
        var factor = DampenFactor(dampening, elapsed);

        return Vector3.LerpUnclamped(current, target, factor);
    }

    public static Vector3 Dampen3(Vector3 current, Vector3 target, float dampening, float elapsed, float minStep)
    {
        current = Vector3.MoveTowards(current, target, minStep * elapsed);

        return Dampen3(current, target, dampening, elapsed);
    }

    public static int Mod(int a, int b)
    {
        var m = a % b;

        if (m < 0)
        {
            return m + b;
        }

        return m;
    }

    public static Bounds NewBoundsFromMinMax(Vector3 min, Vector3 max)
    {
        var bounds = default(Bounds);

        bounds.SetMinMax(min, max);

        return bounds;
    }

    public static Bounds NewBoundsCenter(Bounds b, Vector3 c)
    {
        var x = Mathf.Max(Mathf.Abs(c.x - b.min.x), Mathf.Abs(c.x - b.max.x));
        var y = Mathf.Max(Mathf.Abs(c.y - b.min.z), Mathf.Abs(c.y - b.max.y));
        var z = Mathf.Max(Mathf.Abs(c.z - b.min.z), Mathf.Abs(c.z - b.max.z));

        return new Bounds(c, new Vector3(x, y, z) * 2.0f);
    }

    public static void ResizeArrayTo<T>(ref List<T> array, int size, System.Func<int, T> newT, System.Action<T> removeT)
    {
        if (array != null)
        {
            while (array.Count < size)
            {
                array.Add(newT != null ? newT(array.Count) : default(T));
            }

            while (array.Count > size)
            {
                if (removeT != null)
                {
                    removeT(array[array.Count - 1]);
                }

                array.RemoveAt(array.Count - 1);
            }
        }
    }

    public static bool RequireArraySize<T>(ref T[] array, int size)
    {
        if (array == null || array.Length != size)
        {
            array = new T[size];

            return true;
        }

        return false;
    }

    public static void BeginRandomSeed(int newSeed)
    {
        seedStates.Push(Random.state);

        Random.InitState(newSeed);
    }

    public static void EndRandomSeed()
    {
        Random.state = seedStates.Pop();
    }

    public static Material CreateTempMaterial(string materialName, string shaderName)
    {
        var shader = Shader.Find(shaderName);

        if (shader == null)
        {
            Debug.LogError("Failed to find shader: " + shaderName); return null;
        }

        var material = new Material(shader);

        material.name = materialName;

#if UNITY_EDITOR
        material.hideFlags = HideFlags.HideAndDontSave;
#endif

        return material;
    }

//    public static Mesh CreateTempMesh(string meshName)
//    {
//        var mesh = SgtObjectPool<Mesh>.Pop() ?? new Mesh();

//        mesh.name = meshName;

//#if UNITY_EDITOR
//        mesh.hideFlags = HideFlags.DontSave;
//#endif
//        return mesh;
//    }

    public static Texture2D CreateTempTexture2D(string name, int width, int height, TextureFormat format = TextureFormat.ARGB32, bool mips = false, bool linear = false)
    {
        var texture2D = new Texture2D(width, height, format, mips, linear);

        texture2D.name = name;

#if UNITY_EDITOR
        texture2D.hideFlags = HideFlags.DontSave;
#endif

        return texture2D;
    }

    public static Texture2D CreateTexture2D(string name, int width, int height, TextureFormat format = TextureFormat.ARGB32, bool mips = false, bool linear = false, bool recordUndo = true)
    {
        var texture2D = new Texture2D(width, height, format, mips, linear);

        texture2D.name = name;
//#if UNITY_EDITOR
//        if (recordUndo == true)
//        {
//            Undo.RegisterCreatedObjectUndo(texture2D, undoName);
//        }
//#endif

        return texture2D;
    }

    public static Material CreateMaterial(string shaderName, bool recordUndo = true)
    {
        var material = new Material(Shader.Find(shaderName));

//#if UNITY_EDITOR
//        if (recordUndo == true)
//        {
//            Undo.RegisterCreatedObjectUndo(material, undoName);
//        }
//#endif

        return material;
    }

    public static GameObject CloneGameObject(GameObject source, Transform parent, bool keepName = false)
    {
        return CloneGameObject(source, parent, source.transform.localPosition, source.transform.localRotation, keepName);
    }

    public static GameObject CloneGameObject(GameObject source, Transform parent, Vector3 localPosition, Quaternion localRotation, bool keepName = false)
    {
        if (source != null)
        {
            var clone = default(GameObject);

            if (parent != null)
            {
                clone = (GameObject)GameObject.Instantiate(source);

                clone.transform.parent = parent;
                clone.transform.localPosition = localPosition;
                clone.transform.localRotation = localRotation;
                clone.transform.localScale = source.transform.localScale;
            }
            else
            {
                clone = (GameObject)GameObject.Instantiate(source, localPosition, localRotation);
            }

            if (keepName == true) clone.name = source.name;

            return clone;
        }

        return source;
    }

    public static GameObject CreateGameObject(string name, int layer, Transform parent = null, bool recordUndo = true)
    {
        return CreateGameObject(name, layer, parent, Vector3.zero, Quaternion.identity, Vector3.one, recordUndo);
    }

    public static GameObject CreateGameObject(string name, int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, bool recordUndo = true)
    {
        var gameObject = new GameObject(name);

        gameObject.layer = layer;

        gameObject.transform.SetParent(parent, false);

        gameObject.transform.localPosition = localPosition;
        gameObject.transform.localRotation = localRotation;
        gameObject.transform.localScale = localScale;

//#if UNITY_EDITOR
//        if (recordUndo == true)
//        {
//            Undo.RegisterCreatedObjectUndo(gameObject, undoName);
//        }
//#endif

        return gameObject;
    }

    public static void SetSharedMaterial(Renderer r, Material m)
    {
        if (r != null && r.sharedMaterial != m)
        {
            r.sharedMaterial = m;
        }
    }

    public static void SetSharedMesh(MeshFilter mf, Mesh m)
    {
        if (mf != null && mf.sharedMesh != m)
        {
            mf.sharedMesh = m;
        }
    }

    public static void SetEnabled(Behaviour b, bool enabled)
    {
        if (b != null && b.enabled != enabled)
        {
            b.enabled = enabled;
        }
    }

    public static void SetEnabled(Collider c, bool enabled)
    {
        if (c != null && c.enabled != enabled)
        {
            c.enabled = enabled;
        }
    }

    public static void SetEnabled(Renderer r, bool enabled)
    {
        if (r != null && r.enabled != enabled)
        {
            r.enabled = enabled;
        }
    }

    public static void SetPosition(Transform t, float x, float y, float z)
    {
        SetPosition(t, new Vector3(x, y, z));
    }

    public static void SetPosition(Transform t, Vector3 v)
    {
        if (t != null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false && t.position == v) return;
#endif
            t.position = v;
        }
    }

    public static void SetLocalPosition(Transform t, float x, float y, float z)
    {
        SetLocalPosition(t, new Vector3(x, y, z));
    }

    public static void SetLocalPosition(Transform t, Vector3 v)
    {
        if (t != null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false && t.localPosition == v) return;
#endif
            t.localPosition = v;
        }
    }

    public static void SetLocalScale(Transform t, float v)
    {
        SetLocalScale(t, new Vector3(v, v, v));
    }

    public static void SetLocalScale(Transform t, float x, float y, float z)
    {
        SetLocalScale(t, new Vector3(x, y, z));
    }

    public static void SetLocalScale(Transform t, Vector3 v)
    {
        if (t != null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false && t.localScale == v) return;
#endif
            if (t.localScale == v) return;

            t.localScale = v;
        }
    }

    public static void SetLocalRotation(Transform t, Quaternion q)
    {
        if (t != null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false && t.localRotation == q) return;
#endif
            t.localRotation = q;
        }
    }

    public static void SetRotation(Transform t, Quaternion q)
    {
        if (t != null)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false && t.rotation == q) return;
#endif
            t.rotation = q;
        }
    }

    public static float DistanceToHorizon(float radius, float distanceToCenter)
    {
        if (distanceToCenter > radius)
        {
            return Mathf.Sqrt(distanceToCenter * distanceToCenter - radius * radius);
        }

        return 0.0f;
    }

    // return.x = -PI   .. +PI
    // return.y = -PI/2 .. +PI/2
    public static Vector2 CartesianToPolar(Vector3 xyz)
    {
        var longitude = Mathf.Atan2(xyz.x, xyz.z);
        var latitude = Mathf.Asin(xyz.y / xyz.magnitude);

        return new Vector2(longitude, latitude);
    }

    // return.x = 0 .. 1
    // return.y = 0 .. 1
    public static Vector2 CartesianToPolarUV(Vector3 xyz)
    {
        var uv = CartesianToPolar(xyz);

        uv.x = Mathf.Repeat(0.5f - uv.x / (Mathf.PI * 2.0f), 1.0f);
        uv.y = 0.5f + uv.y / Mathf.PI;

        return uv;
    }

    public static Vector4 CalculateSpriteUV(Sprite s)
    {
        var uv = default(Vector4);

        if (s != null)
        {
            var r = s.textureRect;
            var t = s.texture;

            uv.x = SgtHelper.Divide(r.xMin, t.width);
            uv.y = SgtHelper.Divide(r.yMin, t.height);
            uv.z = SgtHelper.Divide(r.xMax, t.width);
            uv.w = SgtHelper.Divide(r.yMax, t.height);
        }

        return uv;
    }

    public static void CalculateAtmosphereThicknessAtHorizon(float groundRadius, float skyRadius, float distance, out float groundThickness, out float skyThickness)
    {
        var horizonDistance = DistanceToHorizon(groundRadius, distance);
        var maxSkyThickness = Mathf.Sin(Mathf.Acos(SgtHelper.Divide(groundRadius, skyRadius))) * skyRadius;
        var maxGroundThickness = Mathf.Min(horizonDistance, maxSkyThickness);

        groundThickness = Mathf.Max(maxGroundThickness, skyRadius - groundRadius);
        skyThickness = maxSkyThickness + maxGroundThickness;
    }

    public static void EnableKeyword(string keyword, Material material)
    {
        if (material != null)
        {
            if (material.IsKeywordEnabled(keyword) == false)
            {
                material.EnableKeyword(keyword);
            }
        }
    }

    public static void DisableKeyword(string keyword, Material material)
    {
        if (material != null)
        {
            if (material.IsKeywordEnabled(keyword) == true)
            {
                material.DisableKeyword(keyword);
            }
        }
    }

    public static void SetKeywords(Material m, List<string> keywords)
    {
        if (m != null && ArraysEqual(m.shaderKeywords, keywords) == false)
        {
            m.shaderKeywords = keywords.ToArray();
        }
    }

    public static void SetKeywords(Material m, List<string> keywords, ref string[] lastKeywords)
    {
        if (m != null && ArraysEqual(lastKeywords, keywords) == false)
        {
            if (lastKeywords != null && lastKeywords.Length == keywords.Count)
            {
                for (var i = keywords.Count - 1; i >= 0; i--)
                {
                    lastKeywords[i] = keywords[i];
                }
            }
            else
            {
                lastKeywords = keywords.ToArray();
            }

            m.shaderKeywords = lastKeywords;
        }
    }

    public static bool ArraysEqual<T>(T[] a, List<T> b)
    {
        if (a != null && b == null) return false;
        if (a == null && b != null) return false;

        if (a != null && b != null)
        {
            if (a.Length != b.Count) return false;

            var comparer = System.Collections.Generic.EqualityComparer<T>.Default;

            for (var i = 0; i < a.Length; i++)
            {
                if (comparer.Equals(a[i], b[i]) == false)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static List<Material> materials = new List<Material>();

    public static void AddMaterial(Renderer r, Material m)
    {
        if (r != null && m != null)
        {
            var sms = r.sharedMaterials;

            foreach (var sm in sms)
            {
                if (sm == m)
                {
                    return;
                }
            }

            foreach (var sm in sms)
            {
                if (sm != null)
                {
                    materials.Add(sm);
                }
            }

            materials.Add(m);

            r.sharedMaterials = materials.ToArray(); materials.Clear();
        }
    }

    // Prevent applying the same shader material twice
    public static void ReplaceMaterial(Renderer r, Material m)
    {
        if (r != null && m != null)
        {
            var sms = r.sharedMaterials;

            foreach (var sm in sms)
            {
                if (sm == m)
                {
                    return;
                }
            }

            foreach (var sm in sms)
            {
                if (sm != null)
                {
                    if (sm.shader != m.shader)
                    {
                        materials.Add(sm);
                    }
                }
            }

            materials.Add(m);

            r.sharedMaterials = materials.ToArray(); materials.Clear();
        }
    }

    public static void RemoveMaterial(Renderer r, Material m)
    {
        if (r != null)
        {
            var sms = r.sharedMaterials;

            foreach (var sm in sms)
            {
                if (sm != null && sm != m)
                {
                    materials.Add(sm);
                }
            }

            r.sharedMaterials = materials.ToArray(); materials.Clear();
        }
    }

    public static float UniformScale(Vector3 scale)
    {
        scale.x = System.Math.Abs(scale.x);
        scale.y = System.Math.Abs(scale.y);
        scale.z = System.Math.Abs(scale.z);

        return (scale.x + scale.y + scale.z) / 3.0f;
    }

    public static Matrix4x4 Rotation(Quaternion q)
    {
        var matrix = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);

        return matrix;
    }

    public static Matrix4x4 Translation(Vector3 xyz)
    {
        var matrix = Matrix4x4.identity;

        matrix.m03 = xyz.x;
        matrix.m13 = xyz.y;
        matrix.m23 = xyz.z;

        return matrix;
    }

    public static Matrix4x4 Scaling(float xyz)
    {
        var matrix = Matrix4x4.identity;

        matrix.m00 = xyz;
        matrix.m11 = xyz;
        matrix.m22 = xyz;

        return matrix;
    }

    public static Matrix4x4 Scaling(Vector3 xyz)
    {
        var matrix = Matrix4x4.identity;

        matrix.m00 = xyz.x;
        matrix.m11 = xyz.y;
        matrix.m22 = xyz.z;

        return matrix;
    }

    public static Matrix4x4 Scaling(float x, float y, float z)
    {
        var matrix = Matrix4x4.identity;

        matrix.m00 = x;
        matrix.m11 = y;
        matrix.m22 = z;

        return matrix;
    }

    public static Matrix4x4 ShearingX(Vector2 yz) // X changes with y/z
    {
        var matrix = Matrix4x4.identity;

        matrix.m01 = yz.x;
        matrix.m02 = yz.y;

        return matrix;
    }

    public static Matrix4x4 ShearingY(Vector2 xz) // Y changes with x/z
    {
        var matrix = Matrix4x4.identity;

        matrix.m10 = xz.x;
        matrix.m12 = xz.y;

        return matrix;
    }

    public static Matrix4x4 ShearingZ(Vector2 xy) // Z changes with x/y
    {
        var matrix = Matrix4x4.identity;

        matrix.m20 = xy.x;
        matrix.m21 = xy.y;

        return matrix;
    }

    public static Color Brighten(Color color, float brightness)
    {
        color.r *= brightness;
        color.g *= brightness;
        color.b *= brightness;

        return color;
    }

    public static Color Premultiply(Color color)
    {
        color.r *= color.a;
        color.g *= color.a;
        color.b *= color.a;

        return color;
    }

    public static void CalculateLight(Light light, Vector3 center, Transform directionTransform, Transform positionTransform, ref Vector3 position, ref Vector3 direction, ref Color color)
    {
        if (light != null)
        {
            direction = -light.transform.forward;
            position = light.transform.position;
            color = SgtHelper.Brighten(light.color, light.intensity);

            switch (light.type)
            {
                case LightType.Point: direction = Vector3.Normalize(position - center); break;

                case LightType.Directional: position = center + direction * 10000.0f; break;
            }

            // Transform into local space?
            if (directionTransform != null)
            {
                direction = directionTransform.InverseTransformDirection(direction);
            }

            if (positionTransform != null)
            {
                position = positionTransform.InverseTransformPoint(position);
            }
        }
    }

    public static void SetTempMaterial(Material material)
    {
        tempMaterials.Clear();

        tempMaterials.Add(material);
    }

    public static void SetTempMaterial(Material material1, Material material2)
    {
        tempMaterials.Clear();

        tempMaterials.Add(material1);
        tempMaterials.Add(material2);
    }

    public static void SetTempMaterial(List<Material> materials)
    {
        tempMaterials.Clear();

        if (materials != null)
        {
            tempMaterials.AddRange(materials);
        }
    }

    public static void EnableKeyword(string keyword)
    {
        for (var i = tempMaterials.Count - 1; i >= 0; i--)
        {
            var tempMaterial = tempMaterials[i];

            if (tempMaterial != null)
            {
                if (tempMaterial.IsKeywordEnabled(keyword) == false)
                {
                    tempMaterial.EnableKeyword(keyword);
                }
            }
        }
    }

    public static void DisableKeyword(string keyword)
    {
        for (var i = tempMaterials.Count - 1; i >= 0; i--)
        {
            var tempMaterial = tempMaterials[i];

            if (tempMaterial != null)
            {
                if (tempMaterial.IsKeywordEnabled(keyword) == true)
                {
                    tempMaterial.DisableKeyword(keyword);
                }
            }
        }
    }

    public static void SetMatrix(string key, Matrix4x4 value)
    {
        for (var i = tempMaterials.Count - 1; i >= 0; i--)
        {
            var tempMaterial = tempMaterials[i];

            if (tempMaterial != null)
            {
                tempMaterial.SetMatrix(key, value);
            }
        }
    }

    // Writes light data to tempMaterials
    public static void WriteLights(bool lit, List<Light> lights, int maxLights, Vector3 center, Transform directionTransform, Transform positionTransform, Color tint, float scatterStrength)
    {
        var lightCount = 0;

        if (lit == true && lights != null)
        {
            for (var i = 0; i < lights.Count && lightCount < maxLights; i++)
            {
                var light = lights[i];

                if (SgtHelper.Enabled(light) == true && light.intensity > 0.0f)
                {
                    var prefix = "_Light" + (++lightCount);
                    var direction = default(Vector3);
                    var position = default(Vector3);
                    var color = default(Color);

                    CalculateLight(light, center, directionTransform, positionTransform, ref position, ref direction, ref color);

                    var position4 = SgtHelper.NewVector4(position, 1.0f);

                    for (var j = tempMaterials.Count - 1; j >= 0; j--)
                    {
                        var tempMaterial = tempMaterials[j];

                        if (tempMaterial != null)
                        {
                            tempMaterial.SetVector(prefix + "Direction", direction);
                            tempMaterial.SetVector(prefix + "Position", position4);
                            tempMaterial.SetColor(prefix + "Color", color * tint);
                            tempMaterial.SetColor(prefix + "Scatter", color * tint * scatterStrength);
                        }
                    }
                }
            }
        }

        for (var i = 0; i <= maxLights; i++)
        {
            if (lit == true && i == lightCount)
            {
                EnableKeyword("LIGHT_" + i);
            }
            else
            {
                DisableKeyword("LIGHT_" + i);
            }
        }
    }

    //public static void WriteShadows(List<SgtShadow> shadows, int maxShadows)
    //{
    //    var shadowCount = 0;

    //    if (shadows != null)
    //    {
    //        var shadowsCount = shadows.Count;

    //        for (var i = 0; i < shadowsCount && shadowCount < maxShadows; i++)
    //        {
    //            var shadow = shadows[i];
    //            var matrix = default(Matrix4x4);
    //            var ratio = default(float);

    //            if (SgtHelper.Enabled(shadow) == true && shadow.CalculateShadow(ref matrix, ref ratio) == true)
    //            {
    //                var prefix = "_Shadow" + (++shadowCount);

    //                for (var j = tempMaterials.Count - 1; j >= 0; j--)
    //                {
    //                    var tempMaterial = tempMaterials[j];

    //                    if (tempMaterial != null)
    //                    {
    //                        tempMaterial.SetTexture(prefix + "Texture", shadow.GetTexture());
    //                        tempMaterial.SetMatrix(prefix + "Matrix", matrix);
    //                        tempMaterial.SetFloat(prefix + "Ratio", ratio);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    for (var i = 1; i <= maxShadows; i++)
    //    {
    //        if (i == shadowCount)
    //        {
    //            EnableKeyword("SHADOW_" + i);
    //        }
    //        else
    //        {
    //            DisableKeyword("SHADOW_" + i);
    //        }
    //    }
    //}

    //public static void WriteShadowsNonSerialized(List<SgtShadow> shadows, int maxShadows)
    //{
    //    if (shadows != null)
    //    {
    //        var shadowCount = 0;

    //        for (var i = 0; i < shadows.Count && shadowCount < maxShadows; i++)
    //        {
    //            var shadow = shadows[i];
    //            var matrix = default(Matrix4x4);
    //            var ratio = default(float);

    //            if (SgtHelper.Enabled(shadow) == true && shadow.CalculateShadow(ref matrix, ref ratio) == true)
    //            {
    //                var prefix = "_Shadow" + (++shadowCount);

    //                SetMatrix(prefix + "Matrix", matrix);
    //            }
    //        }
    //    }
    //}

    public static void WriteMie(float sharpness, float strength)
    {
        sharpness = Mathf.Pow(10.0f, sharpness);
        strength *= (Mathf.Log10(sharpness) + 1) * 0.75f;

        //var mie  = -(1.0f - 1.0f / Mathf.Pow(10.0f, sharpness));
        //var mie4 = new Vector4(mie * 2.0f, 1.0f - mie * mie, 1.0f + mie * mie, mie / strength);

        var mie = -(1.0f - 1.0f / sharpness);
        var mie4 = new Vector4(mie * 2.0f, 1.0f - mie * mie, 1.0f + mie * mie, strength);

        for (var j = tempMaterials.Count - 1; j >= 0; j--)
        {
            var tempMaterial = tempMaterials[j];

            if (tempMaterial != null)
            {
                tempMaterial.SetVector("_Mie", mie4);
            }
        }
    }

    public static void WriteRayleigh(float strength)
    {
        for (var j = tempMaterials.Count - 1; j >= 0; j--)
        {
            var tempMaterial = tempMaterials[j];

            if (tempMaterial != null)
            {
                tempMaterial.SetFloat("_Rayleigh", strength);
            }
        }
    }

#if UNITY_EDITOR
    public static void DrawSphere(Vector3 center, Vector3 right, Vector3 up, Vector3 forward, int resolution = 32)
    {
        DrawCircle(center, right, up, resolution);
        DrawCircle(center, right, forward, resolution);
        DrawCircle(center, forward, up, resolution);
    }

    public static void DrawCircle(Vector3 center, Vector3 right, Vector3 up, int resolution = 32)
    {
        var step = Reciprocal(resolution);

        for (var i = 0; i < resolution; i++)
        {
            var a = i * step;
            var b = a + step;

            a = a * Mathf.PI * 2.0f;
            b = b * Mathf.PI * 2.0f;

            Gizmos.DrawLine(center + right * Mathf.Sin(a) + up * Mathf.Cos(a), center + right * Mathf.Sin(b) + up * Mathf.Cos(b));
        }
    }

    public static void DrawCircle(Vector3 center, Vector3 axis, float radius, int resolution = 32)
    {
        var rotation = Quaternion.FromToRotation(Vector3.up, axis);
        var right = rotation * Vector3.right * radius;
        var forward = rotation * Vector3.forward * radius;

        DrawCircle(center, right, forward, resolution);
    }
#endif
}
