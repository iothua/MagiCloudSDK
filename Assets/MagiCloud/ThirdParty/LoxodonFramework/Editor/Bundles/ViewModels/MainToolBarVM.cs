using UnityEditor;
using UnityEngine;

namespace Loxodon.Framework.Bundles.Editors
{
    [System.Serializable]
    public class MainToolBarVM : AbstractViewModel
    {
        private const string PREFIX = "Loxodon::Framework::Bundle::";
        private const string CURRENT_MENU_INDEX_KEY = "CURR_MENU_INDEX";

        [SerializeField]
        private int currentMenuIndex = -1;
        [SerializeField]
        private string[] menus;

        public MainToolBarVM()
        {
        }

        public string[] Menus
        {
            get { return this.menus; }
            set { this.menus = value; }
        }

        public int CurrentMenuIndex
        {
            get
            {
                if (this.currentMenuIndex == -1)
                    this.currentMenuIndex = EditorPrefs.GetInt(PREFIX + CURRENT_MENU_INDEX_KEY, 0);

                return this.currentMenuIndex;
            }
            set
            {
                if (this.currentMenuIndex == value)
                    return;

                this.currentMenuIndex = value;
                EditorPrefs.SetInt(PREFIX + CURRENT_MENU_INDEX_KEY, this.currentMenuIndex);
            }
        }
    }
}
