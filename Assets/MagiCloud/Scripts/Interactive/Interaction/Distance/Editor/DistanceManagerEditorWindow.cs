using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace MagiCloud.Interactive.Distance
{
    /// <summary>
    /// 距离管理器窗口
    /// </summary>
    public class DistanceManagerEditorWindow : EditorWindow
    {
        private static GUISkin magiCloud;
        Vector2 scrollPos = Vector2.zero;

        string distancePath;

        private string distanceAllPath;

        private GUIStyle boxStyle;

        DistanceManagerEditorWindow()
        {
            this.titleContent = new GUIContent("距离管理窗口");
        }

        [MenuItem("MagiCloud/距离管理窗口")]
        static void CreateDistanceManager()
        {
            EditorWindow.GetWindow(typeof(DistanceManagerEditorWindow));
        }

        private void OnGUI()
        {
            if (magiCloud == null)
                magiCloud = Resources.Load<GUISkin>("MagiCloud");

            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.alignment = TextAnchor.MiddleCenter;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            //读取路径文件
            //解析
            //然后将总的文件读取出来，并且嵌进入。保存

            EditorGUILayout.BeginVertical("box");

            InspectorDistance(DistanceStorage.dataManagers);

            //展示出所有的ID
            EditorGUILayout.EndVertical();

            GUILayout.EndScrollView();
        }


        /// <summary>
        /// 显示所有的距离信息
        /// </summary>
        void InspectorDistance(List<DistanceDataManager> dataManagers)
        {

            GUILayout.BeginHorizontal();

            GUILayout.Box("物体", boxStyle, GUILayout.Width(150), GUILayout.Height(22));
            GUILayout.Box("TagID", boxStyle, GUILayout.Width(70), GUILayout.Height(22));
            GUILayout.Box("类型", boxStyle, GUILayout.Width(70), GUILayout.Height(22));
            GUILayout.Box("删除", boxStyle, GUILayout.Width(70), GUILayout.Height(22));

            GUILayout.Box("物体", boxStyle, GUILayout.Width(150), GUILayout.Height(22));
            GUILayout.Box("TagID", boxStyle, GUILayout.Width(70), GUILayout.Height(22));
            GUILayout.Box("类型", boxStyle, GUILayout.Width(70), GUILayout.Height(22));
            GUILayout.Box("删除", boxStyle, GUILayout.Width(70), GUILayout.Height(22));

            GUILayout.EndHorizontal();

            //距离管理
            foreach (var data in dataManagers.ToList())
            {
                GUILayout.BeginHorizontal();

                float height = 22;

                height = data.Distances.Count == 0 ? 22 : (22 * data.Distances.Count) + 3 * (data.Distances.Count - 1);

                if (GUILayout.Button(data.sendData.Interaction.name, GUILayout.Width(150), GUILayout.Height(height)))
                {
                    Selection.activeGameObject = data.sendData.Interaction.gameObject;
                }
                //GUILayout.Box(data.sendData.Guid, boxStyle, GUILayout.Width(250), GUILayout.Height(height));
                GUILayout.Box(data.sendData.TagID, boxStyle, GUILayout.Width(70),GUILayout.Height(height));
                GUILayout.Box(data.sendData.interactionType.ToString(), boxStyle, GUILayout.Width(70), GUILayout.Height(height));

                if (GUILayout.Button("删除", GUILayout.Width(70), GUILayout.Height(height)))
                {
                    dataManagers.Remove(data);

                    DestroyImmediate(data.sendData.Interaction);
                }

                GUILayout.BeginVertical();

                foreach (var distance in data.Distances.ToList())
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(distance.Interaction.name, GUILayout.Width(150), GUILayout.Height(height)))
                    {
                        Selection.activeGameObject = distance.Interaction.gameObject;
                    }
                    //GUILayout.Box(distance.Guid, boxStyle, GUILayout.Width(250), GUILayout.Height(22));
                    GUILayout.Box(distance.TagID, boxStyle, GUILayout.Width(70), GUILayout.Height(22));
                    GUILayout.Box(distance.interactionType.ToString(), boxStyle, GUILayout.Width(70), GUILayout.Height(22));

                    if (GUILayout.Button("删除", GUILayout.Width(70)))
                    {
                        data.Distances.Remove(distance);

                        DestroyImmediate(distance.Interaction);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

        }
    }
}