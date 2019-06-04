using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MagiCloud.CameraManager
{
    /// <summary>
    /// 摄像机管理端
    /// </summary>
    public class CameraManagerEditorWindow : EditorWindow
    {
        private GUISkin magiCloud;

        private List<CameraInfo> cameraInfos;

        Vector2 scrollPos = Vector2.zero;

        private GUIStyle labelStyle;

        CameraManagerEditorWindow()
        {
            this.titleContent = new GUIContent("框架摄像机管理");
        }

        [MenuItem("MagiCloud/场景摄像机管理")]
        static void CreateCameraManager()
        {
            EditorWindow.GetWindow(typeof(CameraManagerEditorWindow));
        }

        private void OnEnable()
        {
            if (magiCloud == null)
                magiCloud = Resources.Load<GUISkin>("MagiCloud");

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(magiCloud.label);
                labelStyle.fontSize = 13;
            }

            if (cameraInfos == null || cameraInfos.Count == 0)
            {
                var cameras = FindObjectsOfType<Camera>();
                cameraInfos = new List<CameraInfo>(cameras.Length);

                foreach (var camera in cameras)
                {
                    var info = camera.GetComponent<CameraInfo>() ?? camera.gameObject.AddComponent<CameraInfo>();
                    info.hideFlags = HideFlags.HideInInspector;
                    info.Camera = camera;

                    cameraInfos.Add(info);
                }
            }
        }

        private void OnDestroy()
        {
            cameraInfos.Clear();
        }

        private void OnGUI()
        {

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            GUILayout.Space(10);

            GUILayout.BeginVertical("box", GUILayout.Width(500));

            GUILayout.BeginHorizontal();

            GUILayout.Box("摄像机名称", GUILayout.Width(100), GUILayout.Height(20));
            GUILayout.Box("渲染类型", GUILayout.Width(100), GUILayout.Height(20));
            GUILayout.Box("深度", GUILayout.Width(80), GUILayout.Height(20));
            GUILayout.Box("描述", GUILayout.Width(200), GUILayout.Height(20));

            GUILayout.EndHorizontal();

            for (int i = 0; i < cameraInfos.Count; i++)
            {
                var info = cameraInfos[i];

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(info.name, GUILayout.Width(100), GUILayout.Height(20)))
                {
                    Selection.activeGameObject = info.gameObject;
                }

                GUILayout.Box(info.ClearFlags, GUILayout.Width(100), GUILayout.Height(20));
                GUILayout.Box(info.Depth, GUILayout.Width(80), GUILayout.Height(20));

                info.Depict = EditorGUILayout.TextField("", info.Depict, GUILayout.Width(200), GUILayout.Height(20));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        private LayerMask test;
    }
}
