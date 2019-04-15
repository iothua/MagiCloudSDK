using System;
using UnityEngine;
using UnityEditor;

namespace Loxodon.Framework.Bundles.Editors
{
    public class MainWindow : EditorWindow
    {
        [MenuItem("Tools/Loxodon/Build AssetBundle")]
        static void ShowWindow()
        {
            var window = GetWindow<MainWindow>(false, "AssetBundle");
            window.Show();
        }

        private MainToolBar mainToolBar;
        private BuildPanel buildPanel;
        private RedundancyAnalysisPanel redundancyAnalysisPanel;

        private MainToolBarVM mainToolBarVM;
        private BuildVM buildVM;

        void OnEnable()
        {
            if (mainToolBarVM == null)
            {
                mainToolBarVM = new MainToolBarVM();
                mainToolBarVM.Menus = new string[] { "Build", "Analysis" };
            }
            mainToolBarVM.OnEnable();

            if (buildVM == null)
                buildVM = new BuildVM();

            buildVM.OnEnable();

            mainToolBar = new MainToolBar(this, mainToolBarVM);
            mainToolBar.OnEnable();

            buildPanel = new BuildPanel(this, this.buildVM);
            buildPanel.OnEnable();

            redundancyAnalysisPanel = new RedundancyAnalysisPanel(this,this.buildVM);
            redundancyAnalysisPanel.OnEnable();
        }

        void OnDisable()
        {
            if (mainToolBar != null)
                mainToolBar.OnDisable();

            if (buildPanel != null)
                buildPanel.OnDisable();

            if (redundancyAnalysisPanel != null)
                redundancyAnalysisPanel.OnDisable();

            if (mainToolBarVM != null)
                mainToolBarVM.OnDisable();

            if (buildVM != null)
                buildVM.OnDisable();
        }

        void OnGUI()
        {
            Rect toolBarRect = new Rect(0, 0, this.position.width, 20);
            Rect panelRect = new Rect(0, toolBarRect.yMax, toolBarRect.width, this.position.height - toolBarRect.yMax);

            this.mainToolBar.OnGUI(toolBarRect);
            int index = this.mainToolBarVM.CurrentMenuIndex;
            switch (index)
            {
                case 0:
                    this.buildPanel.OnGUI(panelRect);
                    break;
                case 1:
                    this.redundancyAnalysisPanel.OnGUI(panelRect);
                    break;
                default:
                    this.buildPanel.OnGUI(panelRect);
                    break;
            }
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}
