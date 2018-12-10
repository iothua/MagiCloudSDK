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
        public string Namespaces; //命名空间

        [LabelText("脚本名称(scriptName)")]
        public string scriptName;

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

        [Button("刷新数据")]
        public void OnUpdateData()
        {
            var equipment = gameObject.GetComponent<EquipmentBase>();

            Type type = equipment.GetType();
            Namespaces = type.Namespace;
            scriptName = type.Name;

            colliderData = new ColliderData(equipment.FeaturesObject.Collider);
            transformData = new TransformData(transform);

            for (int i = 0; i < modelDatas.Count; i++)
            {
            }

            childs = gameObject.GetComponentsInChildren<EquipmentGenerateInfo>().Where(arg => !arg.Equals(this)).ToList();


        }

        [Button("创建")]
        public void OnCreate()
        {
            //添加脚本，修改名称
            gameObject.name = EquipmentName;
            transform.SetTransform(transformData);

            var equipment = transform.AddEquipmentScript<EquipmentBase>(Namespaces, scriptName);
            if(equipment!=null)
            {
                equipment.FeaturesObject.SetCollider(colliderData.Center.Vector, colliderData.Size.Vector);
            }

            Transform modelNode = equipment == null ? transform : equipment.ModelNode;

            for (int i = 0; i < modelDatas.Count; i++)
            {
                if (modelDatas[i].resourcesItem == null) continue;
                modelDatas[i].CreateModel(modelNode);
            }

            Transform effectNode = equipment == null ? transform : equipment.EffectNode;
            for (int i = 0; i < effectDatas.Count; i++)
            {
                if (effectDatas[i].resourcesItem == null) continue;
                effectDatas[i].CreateModel(effectNode);
            }
        }
    }
}
#endif


