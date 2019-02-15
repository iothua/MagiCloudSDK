using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using MagiCloud.Features;

namespace MagiCloud.Interactive.Distance
{
    [CustomEditor(typeof(DistanceInteraction))]
    [CanEditMultipleObjects]
    public class DistanceInteractionEditor :Editor
    {
        protected DistanceInteraction interaction;

        public SerializedProperty OnEnter, OnExit, OnStay, OnRelease;
        public SerializedProperty OnNotRelease;

        protected virtual void OnEnable()
        {
            interaction = serializedObject.targetObject as DistanceInteraction;

            OnEnter = serializedObject.FindProperty("OnEnter");
            OnExit = serializedObject.FindProperty("OnExit");
            OnStay = serializedObject.FindProperty("OnStay");
            OnRelease = serializedObject.FindProperty("OnRelease");
            OnNotRelease = serializedObject.FindProperty("OnNotRelease");
        }

        protected void AddMenuItemForValue(GenericMenu menu,string value)
        {
            menu.AddItem(new GUIContent(value),interaction.distanceData.TagID.Equals(value),
                OnValueSelected,value);
        }

        protected void OnValueSelected(object value)
        {
            interaction.distanceData.TagID = value.ToString();
        }

        protected void OnDestroyInteraction()
        {
            DistanceStorage.DeleteDistanceData(interaction);
            DestroyImmediate(interaction);
        }

        protected void GetDistanceInfo()
        {
            List<DistanceInteraction> distances = new List<DistanceInteraction>();

            switch (interaction.distanceData.interactionType)
            {
                case InteractionType.Send:

                    //根据这个发送端，获取到所有的接收点信息
                    distances = DistanceStorage.GetReceiveDistanceDatas(interaction);
                    break;
                case InteractionType.Receive:

                    var managers = DistanceStorage.GetSendDistaceDataAll(interaction,InteractionType.Send);

                    foreach (var item in managers)
                    {
                        distances.Add(item.sendData);
                    }

                    break;
                case InteractionType.All:

                    var managerAlls = DistanceStorage.GetSendDistaceDataAll(interaction,InteractionType.All);

                    foreach (var item in managerAlls)
                    {
                        distances.Add(item.sendData);
                    }

                    break;
                case InteractionType.Pour:
                    var managerPours = DistanceStorage.GetSendDistaceDataAll(interaction,InteractionType.Pour);

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
                if (distance == null) continue;

                if (GUILayout.Button(distance.distanceData.interactionType.ToString() + "：" + (distance != null ? distance.name : "获取绑定物体失败"),GUILayout.Width(250)))
                {
                    if (distance != null)
                        Selection.activeGameObject = distance.gameObject;
                }
            }

            GUILayout.EndVertical();
        }

        protected void DrawDistanceInfo()
        {
            interaction.distanceData.IsOnly = EditorGUILayout.Toggle("唯一交互：",interaction.distanceData.IsOnly);
            if (!interaction.distanceData.IsOnly)
            {
                interaction.distanceData.maxCount = EditorGUILayout.IntField(new GUIContent("最大交互数：","-1为无限，0则表示不能交互"),
                    interaction.distanceData.maxCount);
            }

            interaction.distanceData.distanceShape = (DistanceShape)EditorGUILayout.EnumPopup("距离外形：",interaction.distanceData.distanceShape);

            if (interaction.distanceData.distanceShape == DistanceShape.Cube)
            {
                interaction.distanceData.Size = EditorGUILayout.Vector3Field("大小值：",interaction.distanceData.Size);
            }
            else
            {
                interaction.distanceData.distanceValue = EditorGUILayout.FloatField("距离值",interaction.distanceData.distanceValue);
            }

            interaction.distanceData.distanceType = (DistanceType)EditorGUILayout.EnumPopup("距离类型：",interaction.distanceData.distanceType);
        }

        public override void OnInspectorGUI()
        {
            InspectorDistanceInfoGUI();

            InspectorEventGUI();

            serializedObject.ApplyModifiedProperties();
        }

        protected void InspectorDistanceInfoGUI()
        {
            EditorGUILayout.BeginVertical("box",GUILayout.Width(500));

            GUILayout.Space(10);

            NormalEditorGUI();
            //列出所有的信息
            GetDistanceInfo();

            GUILayout.Space(10);

            //设置
            InspectorInteractionAction();

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        public void InspectorInteractionAction()
        {
            EditorGUILayout.BeginVertical("box");

            interaction.ActiveParent = GUILayout.Toggle(interaction.ActiveParent,"激活【加入子父物体】功能");
            if (interaction.ActiveParent)
            {
                interaction.AddParent();

                interaction.interactionParent.IsSelf = EditorGUILayout.Toggle(new GUIContent("      本身：","当进行交互时，交互是否这些本身，True为执行本身，对方不执行。反之同理!"),interaction.interactionParent.IsSelf);

                interaction.interactionParent.Parent = EditorGUILayout.ObjectField(new GUIContent("      父对象：","需要加入子父物体的父对象"),interaction.interactionParent.Parent,typeof(Transform),true) as Transform;
                interaction.interactionParent.localPosition = EditorGUILayout.Vector3Field("      局部坐标：",interaction.interactionParent.localPosition);
                interaction.interactionParent.localRotation = EditorGUILayout.Vector3Field("      局部旋转值：",interaction.interactionParent.localRotation);
            }
            else
            {
                interaction.RemoveParent();
            }

            GUILayout.Space(10);

            interaction.ActiveShadow = GUILayout.Toggle(interaction.ActiveShadow,"激活【虚影】功能");

            if (interaction.ActiveShadow)
            {
                interaction.AddShadow();
                interaction.interactionShadow.isLocal= EditorGUILayout.Toggle("    ·是否使用局部坐标：",interaction.interactionShadow.isLocal);
                interaction.interactionShadow.IsSelf = EditorGUILayout.Toggle(new GUIContent("      本身：","当进行交互时，交互是否执行本身，True为执行本身，对方不执行。反之同理!"),interaction.interactionShadow.IsSelf);

                interaction.interactionShadow.localPosition = EditorGUILayout.Vector3Field("      局部坐标：",interaction.interactionShadow.localPosition);
                interaction.interactionShadow.localRotation = EditorGUILayout.Vector3Field("      局部旋转值：",interaction.interactionShadow.localRotation);

                interaction.interactionShadow.type = (ShadowType)EditorGUILayout.EnumPopup("    ·虚影类型：",interaction.interactionShadow.type);
                if (interaction.interactionShadow.type == ShadowType.Manual)
                {
                    interaction.interactionShadow.traModelNode = EditorGUILayout.ObjectField("    ·虚影模型：",interaction.interactionShadow.traModelNode,typeof(Transform),true) as Transform;
                }
                //interaction.interactionShadow.localScale = EditorGUILayout.Vector3Field("局部大小：", shadow.localScale);

                interaction.interactionShadow.intension=EditorGUILayout.Slider("    ·虚影透明度：",interaction.interactionShadow.intension,0.1f,0.9f);
                interaction.interactionShadow.renderQueue = EditorGUILayout.IntField("    ·Shader渲染层级：",interaction.interactionShadow.renderQueue);
                //interaction.interactionShadow.color=EditorGUILayout.ColorField("    ·虚影透明度：",interaction.interactionShadow.color);
                interaction.interactionShadow.shaderName=EditorGUILayout.TextField("    ·虚影shader名：",interaction.interactionShadow.shaderName);
                interaction.interactionShadow.isReUpdate=EditorGUILayout.Toggle("    ·每次交互更新虚影：",interaction.interactionShadow.isReUpdate);
            }
            else
            {
                interaction.RemoveShadow();
            }

            EditorGUILayout.EndVertical();
        }

        protected void InspectorEventGUI()
        {
            EditorGUILayout.BeginVertical("box",GUILayout.Width(500));

            EditorGUILayout.PropertyField(OnEnter,new GUIContent("靠近(OnEnter)：","当两个物体靠近指定距离时，会触发此事件"),true,null);
            EditorGUILayout.PropertyField(OnExit,new GUIContent("离开(OnExit)：","当两个物体靠近后，离开指定距离时，会触发此事件"),true,null);
            EditorGUILayout.PropertyField(OnStay,new GUIContent("停留(OnStay)：","当两个物体靠近指定距离时，会一直触发此事件"),true,null);
            EditorGUILayout.PropertyField(OnRelease,new GUIContent("有交互释放(OnRelease)：","当两个物体靠近指定距离时，释放会触发此事件"),true,null);
            EditorGUILayout.PropertyField(OnNotRelease,new GUIContent("无交互释放(OnNotRelease)：","当没有物体跟它进行交互时，释放时会出发此事件"),true,null);

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 未加入时的GUI布局
        /// </summary>
        protected void NormalEditorGUI()
        {
            interaction.distanceData.interactionType = (InteractionType)EditorGUILayout.EnumPopup("交互类型：",interaction.distanceData.interactionType);

            switch (interaction.distanceData.interactionType)
            {
                case InteractionType.Receive:

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("交互唯一ID：");

                    if (EditorGUILayout.DropdownButton(new GUIContent(interaction.distanceData.TagID),FocusType.Passive))
                    {
                        var sends = DistanceStorage.GetSendDistanceData(InteractionType.Send);

                        GenericMenu menu = new GenericMenu();

                        foreach (var send in sends)
                        {
                            if (string.IsNullOrEmpty(send.sendData.distanceData.TagID)) continue;

                            AddMenuItemForValue(menu,send.sendData.distanceData.TagID);
                        }

                        menu.ShowAsContext();
                    }
                    GUILayout.EndHorizontal();

                    interaction.AutoDetection = EditorGUILayout.Toggle(new GUIContent("初始交互：","执行Start函数时，会自动检索在距离内的接收点，并且进行交互"),interaction.AutoDetection);

                    //绘制距离信息
                    DrawDistanceInfo();
                    break;
                case InteractionType.Send:

                    interaction.distanceData.TagID = EditorGUILayout.TextField("交互唯一ID：",interaction.distanceData.TagID);

                    interaction.distanceData.IsGrabOwn = EditorGUILayout.Toggle("抓取本身触发：",interaction.distanceData.IsGrabOwn);
                    interaction.distanceData.IsShort = EditorGUILayout.Toggle("最短距离内进行交互", interaction.distanceData.IsShort);

                    interaction.distanceData.detectType = (InteractionDetectType)EditorGUILayout.EnumPopup("距离检测优先级：",interaction.distanceData.detectType);

                    interaction.AutoDetection = false;
                    break;
                case InteractionType.All:
                case InteractionType.Pour:

                    interaction.distanceData.TagID = EditorGUILayout.TextField("交互唯一ID：",interaction.distanceData.TagID);

                    GUILayout.BeginHorizontal();

                    GUILayout.Space(230);

                    if (EditorGUILayout.DropdownButton(new GUIContent(interaction.distanceData.TagID),FocusType.Keyboard))
                    {
                        var alls = DistanceStorage.GetSendDistanceData(InteractionType.All);

                        GenericMenu menu = new GenericMenu();

                        foreach (var all in alls)
                        {
                            if (string.IsNullOrEmpty(all.sendData.distanceData.TagID)) continue;
                            AddMenuItemForValue(menu,all.sendData.distanceData.TagID);
                        }
                        menu.ShowAsContext();
                    }

                    GUILayout.EndHorizontal();

                    interaction.distanceData.IsGrabOwn = EditorGUILayout.Toggle("抓取本身触发：",interaction.distanceData.IsGrabOwn);
                    //interaction.AutoDetection = EditorGUILayout.Toggle(new GUIContent("初始交互：", "执行Start函数时，会自动检索在距离内的接收点，并且进行交互"), interaction.AutoDetection);
                    interaction.distanceData.IsShort = EditorGUILayout.Toggle("最短距离内进行交互", interaction.distanceData.IsShort);

                    interaction.AutoDetection = false;

                    GUILayout.Space(10);
                    interaction.distanceData.detectType = (InteractionDetectType)EditorGUILayout.EnumPopup("距离检测优先级：",interaction.distanceData.detectType);

                    //绘制距离信息
                    DrawDistanceInfo();
                    break;
                default:
                    break;
            }

            GUILayout.Width(5);
            if (GUILayout.Button("刷新数据",GUILayout.Width(100)))
            {
                DistanceStorage.CheckDistanceData();
            }
        }
    }
}

