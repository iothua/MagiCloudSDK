using UnityEngine;
using UnityEditor;
using MagiCloud.Json;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 背包配置
    /// </summary>
    public class BackpackConfigWindows : EditorWindow
    {
        private string fileName; //文件名称

        private string normalItempath;
        private string enterItempath;
        private string distableItempath;

        private static GUISkin backpack;

        private GUIStyle boxTitleStyle;
        private GUIStyle boxValueStyle;

        public KGUI_ItemDataConfig config = new KGUI_ItemDataConfig();

        private KGUI_Backpack_ItemData itemData = new KGUI_Backpack_ItemData();

        private string jsonPath = string.Empty;

        private string deleteName;

        Vector2 scrollPos = Vector2.zero;

        BackpackConfigWindows()
        {
            this.titleContent = new GUIContent("KGUI背包配置窗口");
        }

        [MenuItem("MagiCloud/KGUI/Windows/BackpackConfig(背包配置窗口)")] //在菜单栏显示
        static void CreateBackpackConfig()
        {
            EditorWindow.GetWindow(typeof(BackpackConfigWindows));

        }

        private void OnGUI()
        {

            if (backpack == null)
                backpack = Resources.Load<GUISkin>("KGUI");

            if (jsonPath == string.Empty)
            {
                jsonPath = Application.streamingAssetsPath + "/Backpack/JsonData/";
            }

            if (boxTitleStyle == null)
            {
                boxTitleStyle = new GUIStyle(backpack.box);
                boxTitleStyle.normal.textColor = Color.white;
                boxTitleStyle.fontStyle = FontStyle.Bold;
                boxTitleStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (boxValueStyle == null)
            {
                boxValueStyle = new GUIStyle(backpack.box);
                boxValueStyle.normal.textColor = Color.white;

                boxValueStyle.alignment = TextAnchor.MiddleCenter;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));


            GUILayout.BeginVertical();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();

            fileName = EditorGUILayout.TextField(new GUIContent("文件名称："), fileName, GUILayout.Width(500), GUILayout.Height(18));

            GUILayout.Space(10);

            if (GUILayout.Button(new GUIContent("导入Json数据", "从路径StreamingAssets/Backpack/JsonData下根据名称，导入背包数据"), GUILayout.Width(150), GUILayout.Height(18)))
            {
                string jsonData = Json.JsonHelper.ReadJsonString(jsonPath + fileName + ".json");
                config = Json.JsonHelper.JsonToObject<KGUI_ItemDataConfig>(jsonData);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            //GUILayout.Space(10);
            GUILayout.Label("背包子项数据配置", backpack.label);

            if (itemData == null)
                itemData = new KGUI_Backpack_ItemData();

            itemData.ID = EditorGUILayout.IntField("仪器ID：", itemData.ID, GUILayout.Width(200), GUILayout.Height(18));
            itemData.number = EditorGUILayout.IntField("仪器数量：", itemData.number, GUILayout.Width(200), GUILayout.Height(18));
            itemData.Name = EditorGUILayout.TextField("仪器名称：", itemData.Name, GUILayout.Width(350), GUILayout.Height(18));

            itemData.normalSpritePath = EditorGUILayout.TextField("仪器默认图片：", itemData.normalSpritePath, GUILayout.Width(500), GUILayout.Height(18));
            itemData.disableSpritePath = EditorGUILayout.TextField("仪器禁用图片：", itemData.disableSpritePath, GUILayout.Width(500), GUILayout.Height(18));

            itemData.ItemPath = EditorGUILayout.TextField("仪器预制物体路径(Resources路径下)：", itemData.ItemPath, GUILayout.Width(500), GUILayout.Height(18));

            itemData.zValue = EditorGUILayout.FloatField("生成物体相对摄像机Z轴值：", itemData.zValue, GUILayout.Width(200), GUILayout.Height(18));

            itemData.isGenerate = EditorGUILayout.Toggle("是否初始化生成仪器：", itemData.isGenerate,GUILayout.Width(200),GUILayout.Height(18));

            if(itemData.isGenerate)
            {
                itemData.generateCount = EditorGUILayout.IntSlider("初始数量：", itemData.generateCount, 0, itemData.number, GUILayout.Width(200), GUILayout.Height(18));
                itemData.Position = EditorGUILayout.Vector3Field("初始仪器坐标：", itemData.Position, GUILayout.Width(500), GUILayout.Height(18));
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("添加", GUILayout.Width(150), GUILayout.Height(18)))
            {
                config.ItemDatas.Add(itemData);
                var newItem = new KGUI_Backpack_ItemData();

                newItem.number = itemData.number;
                newItem.normalSpritePath = itemData.normalSpritePath;
                newItem.disableSpritePath = itemData.disableSpritePath;
                newItem.ItemPath = itemData.ItemPath;
                newItem.zValue = itemData.zValue;

                itemData = newItem;
            }

            GUILayout.Space(10);

            if (GUILayout.Button(new GUIContent("导出Json数据", "导出成Json,生成路径会位于KGUI/Resources/Backpack/JsonData下"), GUILayout.Width(150), GUILayout.Height(18)))
            {
                var jsonData = Json.JsonHelper.ObjectToJsonString(config);

                var path = jsonPath + fileName + ".json";

                JsonHelper.SaveJson(jsonData, path);

                Debug.Log("创建成功，路径如下：" + path);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            deleteName = EditorGUILayout.TextField("从配置中删除指定仪器：", deleteName, GUILayout.Width(350), GUILayout.Height(18));

            GUILayout.Space(10);

            if (GUILayout.Button(new GUIContent("删除"), GUILayout.Width(150), GUILayout.Height(18)))
            {
                config.RemoveItem(deleteName);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Box("仪器ID", boxTitleStyle, GUILayout.Width(70), GUILayout.Height(22));
            GUILayout.Box("仪器名称", boxTitleStyle, GUILayout.Width(70), GUILayout.Height(22));
            GUILayout.Box("仪器数量", boxTitleStyle, GUILayout.Width(70), GUILayout.Height(22));
            GUILayout.Box("仪器默认图片", boxTitleStyle, GUILayout.Width(200), GUILayout.Height(22));
            GUILayout.Box("仪器禁用图片", boxTitleStyle, GUILayout.Width(200), GUILayout.Height(22));
            GUILayout.Box("仪器预制物体路径", boxTitleStyle, GUILayout.Width(200), GUILayout.Height(22));
            GUILayout.Box("物体相对摄像机Z轴值", boxTitleStyle, GUILayout.Width(130), GUILayout.Height(22));
            GUILayout.Box("初始生成仪器数量", boxTitleStyle, GUILayout.Width(130), GUILayout.Height(22));
            GUILayout.Box("初始仪器坐标", boxTitleStyle, GUILayout.Width(100), GUILayout.Height(22));
            GUILayout.EndHorizontal();

            if (config.ItemDatas != null)
            {
                //将已经有的，布局展示
                foreach (var item in config.ItemDatas)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Box(item.ID.ToString(), boxValueStyle, GUILayout.Width(70), GUILayout.Height(20));
                    if (GUILayout.Button(item.Name, GUILayout.Width(70), GUILayout.Height(20)))
                    {
                        itemData = item;
                    }
                    GUILayout.Box(item.number.ToString(), boxValueStyle, GUILayout.Width(70), GUILayout.Height(20));
                    GUILayout.Box(item.normalSpritePath, boxValueStyle, GUILayout.Width(200), GUILayout.Height(20));
                    GUILayout.Box(item.disableSpritePath, boxValueStyle, GUILayout.Width(200), GUILayout.Height(20));
                    GUILayout.Box(item.ItemPath, boxValueStyle, GUILayout.Width(200), GUILayout.Height(20));
                    GUILayout.Box(item.zValue.ToString(), boxValueStyle, GUILayout.Width(130), GUILayout.Height(20));

                    GUILayout.Box(item.generateCount.ToString(), boxTitleStyle, GUILayout.Width(130), GUILayout.Height(20));
                    GUILayout.Box(item.Position.ToString(), boxTitleStyle, GUILayout.Width(100), GUILayout.Height(20));

                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }
    }
}

