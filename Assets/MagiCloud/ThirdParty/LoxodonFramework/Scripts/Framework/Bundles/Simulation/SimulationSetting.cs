#if UNITY_EDITOR
using UnityEditor;

namespace Loxodon.Framework.Bundles
{
    public class SimulationSetting
    {
        private const string kSimulateAssetBundleInEditor = "Loxodon::Framework::Bundle::SimulateInEditor";

        public static bool IsSimulationMode
        {
            get { return EditorPrefs.GetBool(kSimulateAssetBundleInEditor, false); }
            set { EditorPrefs.SetBool(kSimulateAssetBundleInEditor, !EditorPrefs.GetBool(kSimulateAssetBundleInEditor)); }
        }
    }
}
#endif
