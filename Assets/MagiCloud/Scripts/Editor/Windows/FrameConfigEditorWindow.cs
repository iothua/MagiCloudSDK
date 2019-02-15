using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MagiCloud.Core;

namespace MagiCloud
{


    public class FrameConfigEditorWindow :EditorWindow
    {
        private static GUISkin magiCloud;

        private static MInitialize initialize;

        private static List<MagiCloudTagInfo> tagInfos; //Tag信息
        private static List<MagiCloudLayerInfo> layerInfos; //Layer信息

        private GUIStyle labelStyle;

        Vector2 scrollPos = Vector2.zero;

        FrameConfigEditorWindow()
        {
            this.titleContent = new GUIContent("框架统一配置");
        }

        [MenuItem("MagiCloud/框架初始化[统一配置]")]
        static void CreateFrameConfig()
        {
            EditorWindow.GetWindow(typeof(FrameConfigEditorWindow));

            initialize = FindObjectOfType<MInitialize>();
        }

        private void OnInitTags()
        {
            if (tagInfos == null || tagInfos.Count == 0)
            {
                tagInfos = new List<MagiCloudTagInfo>();

                tagInfos.Add(new MagiCloudTagInfo() { TagID = "button",Use = "KGUI按钮Tag" });
                tagInfos.Add(new MagiCloudTagInfo() { TagID = "MainCamera",Use = "Kienct主摄像机，照射物体所用" });
                tagInfos.Add(new MagiCloudTagInfo() { TagID = "GameController",Use = "Kinect定位，用于手势发射线" });
            }
        }

        private void OnInitLayers()
        {
            if (layerInfos == null || layerInfos.Count == 0)
            {

                layerInfos = new List<MagiCloudLayerInfo>();

                layerInfos.Add(new MagiCloudLayerInfo() { LayerID = "Layer5 UI",Use = "KGUI插件识别UI所用" });
                layerInfos.Add(new MagiCloudLayerInfo() { LayerID = "Layer8 kinectObject",Use = "Kinect识别物体所用(直接识别物体)" });
                layerInfos.Add(new MagiCloudLayerInfo() { LayerID = "Layer9 kinectRay",Use = "Kinect识别“射线物体”(通过识别碰撞体，间接识别到物体)" });
                layerInfos.Add(new MagiCloudLayerInfo() { LayerID = "Layer10 UI2",Use = "KGUI插件识别UI备用" });

                layerInfos.Add(new MagiCloudLayerInfo() { LayerID = "Layer11 ",Use = "框架备用" });
                layerInfos.Add(new MagiCloudLayerInfo() { LayerID = "Layer12 ",Use = "框架备用" });
                layerInfos.Add(new MagiCloudLayerInfo() { LayerID = "Layer13 ",Use = "框架备用" });
                layerInfos.Add(new MagiCloudLayerInfo() { LayerID = "Layer14 ",Use = "框架备用" });
                layerInfos.Add(new MagiCloudLayerInfo() { LayerID = "Layer15 ",Use = "框架备用" });
            }

        }

        private void OnGUI()
        {
            if (magiCloud == null)
                magiCloud = Resources.Load<GUISkin>("MagiCloud");

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(magiCloud.label);
                labelStyle.fontSize = 13;
            }

            OnInitTags();

            OnInitLayers();

            scrollPos = GUILayout.BeginScrollView(scrollPos,GUILayout.Width(position.width),GUILayout.Height(position.height));


            GUILayout.BeginVertical();
            GUILayout.Space(10);

            GUILayout.Label("创建/查找框架预制物体",magiCloud.label);

            GUILayout.BeginHorizontal();
            initialize = (MInitialize)EditorGUILayout.ObjectField(new GUIContent("场景框架预知物体：","查询场景框架预制物体,如果场景不存在,可点击创建可自动创建,也可通过创建按钮,自动查找"),
                initialize,typeof(MInitialize),false,GUILayout.Width(400),GUILayout.Height(20));

            GUILayout.Space(10);

            if (GUILayout.Button(new GUIContent("创建/查找","如果场景框架预制物体为Null，自动会在场景中创建，如果场景框架预制物体不为Null，但是编辑器窗口为Null，则点击该按钮自动赋值"),
                GUILayout.Width(120),GUILayout.Height(20)))
            {
                initialize = FindObjectOfType<MInitialize>();

                if (initialize == null)
                {
                    GameObject featuresObject = Resources.Load<GameObject>("MagiCloud");
                    var newFeatures = Instantiate<GameObject>(featuresObject);
                    newFeatures.name = featuresObject.name;
                    initialize = newFeatures.GetComponent<MInitialize>();
                }

                Selection.activeGameObject = initialize.gameObject;
            }

            GUILayout.EndHorizontal();

            if (FrameConfig.Config != null)
            {

                #region 高亮公用参数配置

                GUILayout.Space(20);

                GUILayout.Label("高亮公用参数配置",magiCloud.label);

                //FrameConfig.Config.highlightType = (HighlightType)EditorGUILayout.EnumPopup(new GUIContent("高亮类型：","可通过代码FrameConfig.highlightType返回以及设置相关参数"),FrameConfig.Config.highlightType
                //    ,GUILayout.Width(250),GUILayout.Height(20));

                GUILayout.Space(5);

                FrameConfig.Config.highlightColor = EditorGUILayout.ColorField(new GUIContent("高亮颜色：", "可通过代码FrameConfig.highlightColor获取高亮颜色"), FrameConfig.Config.highlightColor
                            , GUILayout.Width(350), GUILayout.Height(20));

                FrameConfig.Config.grabColor = EditorGUILayout.ColorField(new GUIContent("高亮颜色：", "可通过代码FrameConfig.grabColor获取高亮颜色"), FrameConfig.Config.grabColor
                    , GUILayout.Width(350), GUILayout.Height(20));


                //switch (FrameConfig.Config.highlightType)
                //{
                //    case HighlightType.Shader:

                //        FrameConfig.Config.highlightColor = EditorGUILayout.ColorField(new GUIContent("高亮颜色：","可通过代码FrameConfig.highlightColor获取高亮颜色"),FrameConfig.Config.highlightColor
                //            ,GUILayout.Width(350),GUILayout.Height(20));

                //        FrameConfig.Config.grabColor = EditorGUILayout.ColorField(new GUIContent("高亮颜色：","可通过代码FrameConfig.grabColor获取高亮颜色"),FrameConfig.Config.grabColor
                //            ,GUILayout.Width(350),GUILayout.Height(20));

                //        break;
                //    case HighlightType.Model:
                //        break;
                //}

                GUILayout.Space(20);
                #endregion

                #region 标签公用参数配置
                GUILayout.Space(20);
                GUILayout.Label("标签公用参数配置",magiCloud.label);
                FrameConfig.Config.labelFont = (Font)EditorGUILayout.ObjectField(new GUIContent("标签字体："),FrameConfig.Config.labelFont,typeof(Font),true,GUILayout.Width(350),GUILayout.Height(20));
                GUILayout.Space(5);

                //标签颜色
                EditorGUI.BeginChangeCheck();
                FrameConfig.Config.initLabelColor = EditorGUILayout.ColorField(new GUIContent("标签颜色：","可通过代码FrameConfig.labelColor获取高亮颜色"),FrameConfig.Config.initLabelColor
                 ,GUILayout.Width(350),GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck())
                    KGUI.KGUI_LabelController.Instance.SetAllFontColor(FrameConfig.Config.initLabelColor);

                //标签字体大小
                EditorGUI.BeginChangeCheck();
                FrameConfig.Config.initLabelFontSize = EditorGUILayout.IntField(new GUIContent("标签字体大小：","可通过代码FrameConfig.labelFontSize获取字体大小"),FrameConfig.Config.initLabelFontSize,GUILayout.Width(350),GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck())
                    KGUI.KGUI_LabelController.Instance.SetAllFontSize(FrameConfig.Config.initLabelFontSize);
                GUILayout.Space(5);
                //标签背景图片
                EditorGUI.BeginChangeCheck();
                FrameConfig.Config.labelBg = (Sprite)EditorGUILayout.ObjectField(new GUIContent("标签背景图片：","可通过代码FrameConfig.labelBg获取背景图片"),FrameConfig.Config.labelBg,typeof(Sprite),true,GUILayout.Width(350),GUILayout.Height(60));
                if (EditorGUI.EndChangeCheck())
                    KGUI.KGUI_LabelController.Instance.SetAllLabelBG(FrameConfig.Config.labelBg);


                #endregion

            }

            #region 生成框架所需Tags以及Layer层

            GUILayout.Label("生成框架所需Tags以及Layer层",magiCloud.label);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginVertical("box");

            GUILayout.Label("框架Tag信息",labelStyle);
            foreach (var item in tagInfos)
            {
                GUILayout.Label(item.TagID + "：    " + item.Use);
            }

            //if (GUILayout.Button(new GUIContent("生成框架Tag", "如果此时Editor Tags不足框架所需Tag，点击此按钮自动生成"), GUILayout.Width(100), GUILayout.Height(20)))
            //{
            //    foreach (var item in tagInfos)
            //    {
            //        AddTag(item.TagID);
            //    }
            //}

            GUILayout.Space(20);

            GUILayout.Label("框架Layer信息",labelStyle);
            foreach (var item in layerInfos)
            {
                GUILayout.Label(item.LayerID + "：    " + item.Use);
            }

            //if (GUILayout.Button(new GUIContent("生成框架Layer", "如果此时Editor Layer不足框架所需Layer，点击此按钮自动生成"), GUILayout.Width(100), GUILayout.Height(20)))
            //{
            //    foreach (var item in layerInfos)
            //    {
            //        AddLayer(item.LayerID);
            //    }
            //}

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            #endregion

            GUILayout.EndVertical();

            GUILayout.Space(20);

            GUILayout.EndScrollView();
        }

        static void AddTag(string tag)
        {
            if (isHasTag(tag)) return;

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "tags")
                {
                    Debug.Log("添加Tag");
                    it.stringValue = tag;
                    tagManager.ApplyModifiedProperties();

                    //for (int i = 0; i < it.arraySize; i++)
                    //{
                    //    SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                    //    if (string.IsNullOrEmpty(dataPoint.stringValue))
                    //    {
                    //        dataPoint.stringValue = tag;
                    //        tagManager.ApplyModifiedProperties();
                    //        return;
                    //    }
                    //}
                }
            }
        }

        static void AddLayer(string layer)
        {
            if (!isHasLayer(layer))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name.StartsWith("User Layer"))
                    {
                        if (it.type == "string")
                        {
                            if (string.IsNullOrEmpty(it.stringValue))
                            {
                                it.stringValue = layer;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                }
            }
        }

        static bool isHasTag(string tag)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
                    return true;
            }
            return false;
        }

        static bool isHasLayer(string layer)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
            {
                if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
                    return true;
            }
            return false;
        }
    }
}

