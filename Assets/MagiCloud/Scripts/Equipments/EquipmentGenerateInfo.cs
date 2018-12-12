#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System;
using System.Linq;

namespace MagiCloud.Equipments
{


    [ExecuteInEditMode]
    public class EquipmentGenerateInfo : MonoBehaviour
    {
        [Title("仪器基本信息")]
        [LabelText("仪器名称(EquipmentName)")]
        public string EquipmentName = "仪器名称"; //仪器名称

        [LabelText("命名空间(Namespaces)")]
        public string Namespaces = "MagiCloud.Equipments"; //命名空间

        [LabelText("脚本名称(scriptName)")]
        public string scriptName = "EquipmentBase";

        [PropertySpace(10)]
        [LabelText("碰撞体数据(ColliderDat)")]
        //碰撞体数据
        public ColliderData colliderData; //碰撞体数据
        [LabelText("坐标信息(TransformData)")]
        public TransformData transformData;


        [Title("模型数据")]
        public List<EquipmentModelData> modelDatas;
        [Title("特效数据")]
        public List<EquipmentModelData> effectDatas;

        [Title("子仪器")]
        [ReadOnly]
        public List<EquipmentGenerateInfo> childs;

        [ButtonGroup]
        [Button("创建")]
        public void OnCreate()
        {
            //添加脚本，修改名称
            gameObject.name = EquipmentName;
            transform.SetTransform(transformData);

            var equipment = transform.AddEquipmentScript<EquipmentBase>(Namespaces, scriptName);
            if (equipment != null)
            {
                equipment.FeaturesObject.SetCollider(colliderData.Center.Vector, colliderData.Size.Vector);
            }

            Transform modelNode = equipment == null ? transform : equipment.ModelNode;
            modelNode.DestroyImmediateChildObject();

            for (int i = 0; i < modelDatas.Count; i++)
            {
                if (modelDatas[i].resourcesItem == null) continue;
                modelDatas[i].CreateModel(modelNode);
            }

            Transform effectNode = equipment == null ? transform : equipment.EffectNode;

            effectNode.DestroyImmediateChildObject();

            for (int i = 0; i < effectDatas.Count; i++)
            {
                if (effectDatas[i].resourcesItem == null) continue;
                effectDatas[i].CreateModel(effectNode);
            }

            equipment.OnInitializeEquipment_Editor(EquipmentName);
        }

        [ButtonGroup]
        [Button("获取物体数据")]
        public void GetObjectData()
        {
            var equipment = gameObject.GetComponent<EquipmentBase>();
            if (equipment == null) return;

            EquipmentName = name;

            Type type = equipment.GetType();
            Namespaces = type.Namespace;
            scriptName = type.Name;

            colliderData = new ColliderData(equipment.FeaturesObject.Collider);
            transformData = new TransformData(transform);

            for (int i = 0; i < modelDatas.Count; i++)
            {
                modelDatas[i].geneterItem.Assignment();
                modelDatas[i].resourcesItem.Assignment(modelDatas[i].geneterItem.modelObject.transform);
            }

            for (int i = 0; i < effectDatas.Count; i++)
            {
                effectDatas[i].geneterItem.Assignment();
                effectDatas[i].resourcesItem.Assignment(effectDatas[i].geneterItem.modelObject.transform);
            }

            childs = gameObject.GetComponentsInChildren<EquipmentGenerateInfo>().Where(arg => !arg.Equals(this)).ToList();
        }

        [ButtonGroup]
        [Button("设置物体数据")]
        public void SetObjectData()
        {
            var equipment = gameObject.GetComponent<EquipmentBase>();
            if (equipment == null) return;

            equipment.FeaturesObject.SetCollider(colliderData.Center.Vector, colliderData.Size.Vector);
            transform.SetTransform(transformData);

            for (int i = 0; i < modelDatas.Count; i++)
            {
                modelDatas[i].geneterItem.SetTransform();
            }

            for (int i = 0; i < effectDatas.Count; i++)
            {
                effectDatas[i].geneterItem.SetTransform();
            }
        }
    }
}
#endif


