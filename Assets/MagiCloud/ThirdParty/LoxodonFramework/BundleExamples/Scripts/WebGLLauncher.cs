using System;
using System.Collections;
using System.Text;
using UnityEngine;

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Bundles;
using Loxodon.Framework.Contexts;

namespace Loxodon.Framework.Examples.Bundle
{
    public class WebGLLauncher : MonoBehaviour
    {
#if UNITY_WEBGL
        private Rect position = new Rect(5, 5, 150, 40);
        private GUIContent contentON = new GUIContent("Simulation Mode: ON");
        private GUIContent contentOFF = new GUIContent("Simulation Mode: OFF");
        private GUIStyle style;

        private string iv = "5Hh2390dQlVh0AqC";
        private string key = "E4YZgiGQ0aqe5LEJ";
        private IResources resources;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        IEnumerator Start()
        {
            ApplicationContext context = Context.GetApplicationContext();

            /* Create a BundleManifestLoader. */
            IBundleManifestLoader manifestLoader = new BundleManifestLoader();

            /* Loads BundleManifest. */
            IAsyncResult<BundleManifest> result = manifestLoader.LoadAsync(BundleUtil.GetReadOnlyDirectory() + BundleSetting.ManifestFilename);
            yield return result.WaitForDone();
            BundleManifest manifest = result.Result;

            //manifest.ActiveVariants = new string[] { "", "sd" };
            manifest.ActiveVariants = new string[] { "", "hd" };

            this.resources = CreateResources(manifest);           
            context.GetContainer().Register<IResources>(this.resources);
        }

        IResources CreateResources(BundleManifest manifest)
        {
            IResources resources = null;
#if UNITY_EDITOR
            if (SimulationSetting.IsSimulationMode)
            {
                Debug.Log("Use SimulationResources. Run In Editor");

                /* Create a PathInfoParser. */
                //IPathInfoParser pathInfoParser = new SimplePathInfoParser("@");
                IPathInfoParser pathInfoParser = new SimulationAutoMappingPathInfoParser();

                /* Create a BundleManager */
                IBundleManager manager = new SimulationBundleManager();

                /* Create a BundleResources */
                resources = new SimulationResources(pathInfoParser, manager);
            }
            else
#endif
            {
                /* Create a PathInfoParser. */
                //IPathInfoParser pathInfoParser = new SimplePathInfoParser("@");
                IPathInfoParser pathInfoParser = new AutoMappingPathInfoParser(manifest);

                /* Create a BundleLoaderBuilder */
                //ILoaderBuilder builder = new WWWBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), false);

                /* AES128_CBC_PKCS7 */
                RijndaelCryptograph rijndaelCryptograph = new RijndaelCryptograph(128, Encoding.ASCII.GetBytes(this.key), Encoding.ASCII.GetBytes(this.iv));

                /* Use a custom BundleLoaderBuilder */
                ILoaderBuilder builder = new CustomBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), false, rijndaelCryptograph);

                /* Create a BundleManager */
                IBundleManager manager = new BundleManager(manifest, builder);

                /* Create a BundleResources */
                resources = new BundleResources(pathInfoParser, manager);
            }
            return resources;
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            if (style == null)
                style = new GUIStyle("AssetLabel");

            if (SimulationSetting.IsSimulationMode)
                GUI.Label(position, contentON, style);
            else
                GUI.Label(position, contentOFF, style);
        }
#endif
#endif
    }
}
