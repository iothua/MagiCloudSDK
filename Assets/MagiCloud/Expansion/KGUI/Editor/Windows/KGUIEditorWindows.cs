using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace MagiCloud.KGUI
{
    public class KGUIEditorWindows : EditorWindow
    {

        [MenuItem("MagiCloud/KGUI/Control/KGUI_Canvas(画布)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_Canvas(画布)", validate = false, priority = 10)] //在GameObject菜单栏显示，又在Hierarchy菜单栏显示
        private static void CreateKGUI_Canvas()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var canvas = Resources.Load<GameObject>("UI/KGUI_Canvas");

            CreateObject(parent, null, canvas);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_SpriteRenderer(精灵画布)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_SpriteRenderer(精灵画布)", validate = false, priority = 10)] //在GameObject菜单栏显示，又在Hierarchy菜单栏显示
        private static void CreateKGUI_SpriteRenderer()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var spriteRenderer = Resources.Load<GameObject>("UI/KGUI_SpriteRenderer");

            CreateObject(parent, null, spriteRenderer);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_Clock(时钟)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_Clock(时钟)", validate = false, priority = 10)] //在GameObject菜单栏显示，又在Hierarchy菜单栏显示
        private static void CreateKGUI_Clock()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Clock");

            CreateController(parent, ui);
        }
        [MenuItem("MagiCloud/KGUI/Control/KGUI_Text(文本)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_Text(文本)",validate = false,priority = 10)] //在GameObject菜单栏显示，又在Hierarchy菜单栏显示
        private static void CreateKGUI_Text()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Text");

            CreateController(parent,ui);
        }
        [MenuItem("MagiCloud/KGUI/Control/KGUI_Panel(容器)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_Panel(容器)", validate = false, priority = 10)]
        private static void CreateKGUI_Panel()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Panel");

            CreateController(parent, ui);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_Button(按钮)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_Button(按钮)", validate = false, priority = 10)]
        private static void CreateKGUI_Button()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Button");

            CreateController(parent, ui);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_Toggle(开关)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_Toggle(开关)", validate = false, priority = 10)]
        private static void CreateKGUI_Toggle()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Toggle");

            CreateController(parent, ui);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_ScrollbarVer(垂直滚动)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_ScrollbarVer(垂直滚动)", validate = false, priority = 10)]
        private static void CreateKGUI_ScrollbarVer()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Scrollbar_ver");

            CreateController(parent, ui);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_ScrollbarHor(水平滚动)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_ScrollbarHor(水平滚动)", validate = false, priority = 10)]
        private static void CreateKGUI_ScrollbarHor()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Scrollbar_hor");

            CreateController(parent, ui);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_ScrollView(滚动视图)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_ScrollView(滚动视图)", validate = false, priority = 10)]
        private static void CreateKGUI_ScrollView()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Scroll View");

            CreateController(parent, ui);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_Dropdown(下拉框)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_Dropdown(下拉框)", validate = false, priority = 10)]
        private static void CreateKGUI_Dropdown()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Dropdown");

            CreateController(parent, ui);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_TableManager(表格管理器)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_TableManager(表格管理器)", validate = false, priority = 10)]
        private static void CreateKGUI_TableManager()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/TableManager");

            CreateController(parent, ui);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_Table(表格)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_Table(表格)", validate = false, priority = 10)]
        private static void CreateKGUI_Table()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Table");

            CreateController(parent, ui);
        }

        [MenuItem("MagiCloud/KGUI/Control/KGUI_Backpack(背包)")] //在菜单栏显示
        [MenuItem("GameObject/MagiCloud/KGUI/KGUI_Backpack(背包)", validate = false, priority = 10)]
        private static void CreateKGUI_Backpack()
        {
            //Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            Transform parent = GetSeleteTransform();

            var ui = Resources.Load<GameObject>("UI/Backpack");

            CreateController(parent, ui);
        }

        /// <summary>
        /// 创建控件
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="target"></param>
        private static void CreateController(Transform parent, GameObject target)
        {
            KGUI_Canvas canvas = GetKGUI_Canvas();

            if (canvas == null)
                CreateKGUI_Canvas();//创建canvas

            canvas = GetKGUI_Canvas();

            if (parent != null)
                parent = parent.GetComponent<RectTransform>() == null ? null : parent; //如果父对象不是RectTransform类型的，则为Null.

            CreateObject(parent, canvas.transform, target);
        }

        /// <summary>
        /// 创建物体
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="canvas"></param>
        /// <param name="target"></param>
        private static void CreateObject(Transform parent, Transform canvas, GameObject target)
        {
            GameObject newObject = parent == null ? GameObject.Instantiate(target, canvas) : GameObject.Instantiate(target, parent);

            newObject.name = target.name;

            ChangeSelected(newObject);
        }

        /// <summary>
        /// 获取到场景的KGUI_Canvas对象
        /// </summary>
        /// <returns></returns>
        static KGUI_Canvas GetKGUI_Canvas()
        {
            KGUI_Canvas[] canvas = FindObjectsOfType<KGUI_Canvas>();

            if (canvas.Length == 0) return null;
            else return canvas[0];
        }

        /// <summary>
        /// 获取到选中的Trnsform
        /// </summary>
        /// <returns></returns>
        static Transform GetSeleteTransform()
        {
            Transform[] selectedObject = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.ExcludePrefab);

            return selectedObject.Length == 0 ? null : selectedObject[0];
        }

        /// <summary>
        /// 展开并且选中
        /// </summary>
        /// <param name="target"></param>
        static void ChangeSelected(GameObject target)
        {
            Selection.activeGameObject = target;
        }
    }
}


