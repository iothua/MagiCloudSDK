using MagiCloud.Utility.Lighting;
using UnityEngine;

namespace MagiCloud
{
    public class SystemParameters
    {
        /// <summary>
        /// 设置Lighting相关参数
        /// </summary>
        public static void SetLighting(LightingData lighting)
        {
            RenderSettings.skybox = lighting.skyboxMaterial;
            RenderSettings.sun = lighting.sunSource;

            switch (lighting.Source)
            {
                case LightingType.Skybox:
                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
                    //Skybox类型的参数设置
                    RenderSettings.ambientIntensity = lighting.intensityMultiplier;
                    break;
                case LightingType.Gradient:

                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
                    //Gradient类型设置
                    RenderSettings.ambientSkyColor = lighting.skyColor;
                    RenderSettings.ambientEquatorColor = lighting.equatorColor;
                    RenderSettings.ambientGroundColor = lighting.groundColor;

                    break;
                case LightingType.Color:

                    RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                    //Color类型
                    RenderSettings.ambientSkyColor = lighting.ambientColor;
                    break;
            }

        }
    }
}
