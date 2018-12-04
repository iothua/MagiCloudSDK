
using UnityEngine;

namespace MagiCloud.Utility.Lighting
{
    public enum LightingType
    {
        Skybox,
        Gradient,
        Color
    }

    /// <summary>
    /// Lighting数据参数设置
    /// </summary>
    [System.Serializable]
    public class LightingData
    {
        [Header("天空材质")]
        public Material skyboxMaterial;

        [Header("太阳光源")]
        public Light sunSource;

        [Header("灯类型")]
        public LightingType Source = LightingType.Skybox;

        [Header("当Source为Skybox时，该参数有效")]
        public float intensityMultiplier = 1;

        [Header("当Source为Gradient时，以下参数有效")]
        public Color skyColor = new Color(0.212f, 0.227f, 0.259f);
        public Color equatorColor = new Color(0.114f, 0.125f, 0.133f);
        public Color groundColor = new Color(0.047f, 0.043f, 0.035f);

        [Header("当Source为Color时，以下参数有效")]
        public Color ambientColor = new Color(0.212f, 0.227f, 0.259f);
    }

}
