using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MagiCloud.Interactive.Distance
{
    [CustomEditor(typeof(DistanceInteraction))]
    [CanEditMultipleObjects]
    public class DistanceInteractionEditor : Editor
    {
        protected DistanceInteraction interaction;

        public SerializedProperty OnEnter, OnExit, OnStay, OnRelease;
        public SerializedProperty OnNotRelease;
        //private bool IsExitDistanceObject;
        //private bool IsExitDistance;

        protected virtual void OnEnable()
        {
            interaction = serializedObject.targetObject as DistanceInteraction;

            OnEnter = serializedObject.FindProperty("OnEnter");
            OnExit = serializedObject.FindProperty("OnExit");
            OnStay = serializedObject.FindProperty("OnStay");
            OnRelease = serializedObject.FindProperty("OnRelease");
            OnNotRelease = serializedObject.FindProperty("OnNotRelease");
            //GetDistanceEquals();
        }

        protected void AddMenuItemForValue(GenericMenu menu, string value)
        {
            menu.AddItem(new GUIContent(value), interaction.distanceData.TagID.Equals(value),
                OnValueSelected, value);
        }

        protected void OnValueSelected(object value)
        {
            interaction.distanceData.TagID = value.ToString();
        }

        protected void OnDestroyInteraction()
        {
            DistanceStorage.DeleteDistanceData(interaction.distanceData);
            DestroyImmediate(interaction);
        }

        protected void GetDistanceInfo()
        {
            List<DistanceData> distances = new List<DistanceData>();

            switch (interaction.distanceData.interactionType)
            {
                case InteractionType.Send:

                    //根据这个发送端，获取到所有的接收点信息
                    distances = DistanceStorage.GetReceiveDistanceDatas(interaction.distanceData);
                    break;
                case InteractionType.Receive:

                    var managers = DistanceStorage.GetSendDistaceDataAll(interaction.distanceData, InteractionType.Send);

                    foreach (var item in managers)
                    {
                        distances.Add(item.sendData);
                    }

                    break;
                case InteractionType.All:

                    var managerAlls = DistanceStorage.GetSendDistaceDataAll(interaction.distanceData, InteractionType.All);
                        /*&& !obj.sendData.EqualsObject(interaction.distanceData)*/

                    foreach (var item in managerAlls)
                    {
                        distances.Add(item.sendData);
                    }

                    break;
                case InteractionType.Pour:
                    var managerPours = DistanceStorage.GetSendDistaceDataAll(interaction.distanceData, InteractionType.Pour);
                    /*&& !obj.sendData.EqualsObject(interaction.distanceData)*/

                    foreach (var item in managerPours)
                    {
                        distances.Add(item.sendData);
                    }

                    break;
                default:
                    break;
            }

            if (distances.Count == 0) return;

            GUILayout.Space(20);

            GUILayout.BeginVertical("box");

            GUILayout.Label("在场景中与这个距离有绑定的【距离物体】");

            //距离信息遍历
            foreach (var distance in distances)
            {
                if (distance.Interaction == null) continue;

                if (GUILayout.Button(distance.interactionType.ToString() + "：" + (distance.Interaction != null ? distance.Interaction.name : "获取绑定物体失败"), GUILayout.Width(250)))
                {
                    if (distance.Interaction != null)
                        Selection.activeGameObject = distance.Interaction.gameObject;
                }
            }

            GUILayout.EndVertical();
        }

        protected void DrawDistanceInfo()
        {
            interaction.distanceData.IsOnly = EditorGUILayout.Toggle("唯一交互：", interaction.distanceData.IsOnly);
            if (!interaction.distanceData.IsOnly)
            {
                interaction.distanceData.maxCount = EditorGUILayout.IntField(new GUIContent("最大交互数：", "-1为无限，0则表示不能交互"),
                    interaction.distanceData.maxCount);
            }

            interaction.AutoDetection = false;

            interaction.distanceData.distanceShape = (DistanceShape)EditorGUILayout.EnumPopup("距离外形：", interaction.distanceData.distanceShape);

            if (interaction.distanceData.distanceShape == DistanceShape.Cube)
            {
                interaction.distanceData.Size = EditorGUILayout.Vector3Field("大小值：", interaction.distanceData.Size);
            }
            else
            {
                interaction.distanceData.distanceValue = EditorGUILayout.FloatField("距离值", interaction.distanceData.distanceValue);
            }

            interaction.distanceData.distanceType = (DistanceType)EditorGUILayout.EnumPopup("距离类型：", interaction.distanceData.distanceType);
        }

        public override void OnInspectorGUI()
        {
            InspectorDistanceInfoGUI();

            InspectorEventGUI();

            serializedObject.ApplyModifiedProperties();
        }

        protected void InspectorDistanceInfoGUI()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(500));

            GUILayout.Space(10);

            NormalEditorGUI();
            //列出所有的信息
            GetDistanceInfo();

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        protected void InspectorEventGUI()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(500));

            EditorGUILayout.PropertyField(OnEnter, new GUIContent("靠近(OnEnter)：", "当两个物体靠近指定距离时，会触发此事件"), true, null);
            EditorGUILayout.PropertyField(OnExit, new GUIContent("离开(OnExit)：", "当两个物体靠近后，离开指定距离时，会触发此事件"), true, null);
            EditorGUILayout.PropertyField(OnStay, new GUIContent("停留(OnStay)：", "当两个物体靠近指定距离时，会一直触发此事件"), true, null);
            EditorGUILayout.PropertyField(OnRelease, new GUIContent("有交互释放(OnRelease)：", "当两个物体靠近指定距离时，释放会触发此事件"), true, null);
            EditorGUILayout.PropertyField(OnNotRelease, new GUIContent("无交互释放(OnNotRelease)：", "当没有物体跟它进行交互时，释放时会出发此事件"), true, null);

            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// 未加入时的GUI布局
        /// </summary>
        protected void NormalEditorGUI()
        {
            interaction.distanceData.interactionType = (InteractionType)EditorGUILayout.EnumPopup("交互类型：", interaction.distanceData.interactionType);

            switch (interaction.distanceData.interactionType)
            {
                case InteractionType.Receive:

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("交互唯一ID：");

                    if (EditorGUILayout.DropdownButton(new GUIContent(interaction.distanceData.TagID), FocusType.Passive))
                    {
                        var sends = DistanceStorage.GetSendDistanceData(InteractionType.Send);

                        GenericMenu menu = new GenericMenu();

                        foreach (var send in sends)
                        {
                            if (string.IsNullOrEmpty(send.sendData.TagID)) continue;

                            AddMenuItemForValue(menu, send.sendData.TagID);
                        }

                        menu.ShowAsContext();
                    }
                    GUILayout.EndHorizontal();

                    //绘制距离信息
                    DrawDistanceInfo();
                    break;
                case InteractionType.Send:

                    interaction.distanceData.TagID = EditorGUILayout.TextField("交互唯一ID：", interaction.distanceData.TagID);

                    interaction.distanceData.detectType = (InteractionDetectType)EditorGUILayout.EnumPopup("距离检测优先级：", interaction.distanceData.detectType);

                    interaction.distanceData.IsGrabOwn = EditorGUILayout.Toggle("抓取本身触发：", interaction.distanceData.IsGrabOwn);
                    interaction.AutoDetection = EditorGUILayout.Toggle(new GUIContent("初始交互：", "执行Start函数时，会自动检索在距离内的接收点，并且进行交互"), interaction.AutoDetection);

                    break;
                case InteractionType.All:
                case InteractionType.Pour:

                    interaction.distanceData.TagID = EditorGUILayout.TextField("交互唯一ID：", interaction.distanceData.TagID);

                    GUILayout.BeginHorizontal();

                    GUILayout.Space(230);

                    if (EditorGUILayout.DropdownButton(new GUIContent(interaction.distanceData.TagID), FocusType.Keyboard))
                    {
                        var alls = DistanceStorage.GetSendDistanceData(InteractionType.All);

                        GenericMenu menu = new GenericMenu();

                        foreach (var all in alls)
                        {
                            if (string.IsNullOrEmpty(all.sendData.TagID)) continue;
                            AddMenuItemForValue(menu, all.sendData.TagID);
                        }
                        menu.ShowAsContext();
                    }

                    GUILayout.EndHorizontal();

                    interaction.distanceData.IsGrabOwn = EditorGUILayout.Toggle("抓取本身触发：", interaction.distanceData.IsGrabOwn);
                    interaction.AutoDetection = EditorGUILayout.Toggle(new GUIContent("初始交互：", "执行Start函数时，会自动检索在距离内的接收点，并且进行交互"), interaction.AutoDetection);

                    GUILayout.Space(10);
                    interaction.distanceData.detectType = (InteractionDetectType)EditorGUILayout.EnumPopup("距离检测优先级：", interaction.distanceData.detectType);

                    //绘制距离信息
                    DrawDistanceInfo();
                    break;
                default:
                    break;
            }
        }
    }
}

