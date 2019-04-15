using UnityEditor;

namespace Loxodon.Framework.Bundles.Editors
{
    public class SimulationMenu
    {
        private const string kSimulationMode = "Tools/Loxodon/Simulation Mode";

        [MenuItem(kSimulationMode)]
        public static void ToggleSimulationMode()
        {
            SimulationSetting.IsSimulationMode = !SimulationSetting.IsSimulationMode;
        }

        [MenuItem(kSimulationMode, true)]
        public static bool ToggleSimulationModeValidate()
        {
            Menu.SetChecked(kSimulationMode, SimulationSetting.IsSimulationMode);
            return true;
        }

    }
}
