using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MagiCloud.Equipments
{
#if UNITY_EDITOR

    [ExecuteInEditMode]
    public class EquipmentGenerateInfo : MonoBehaviour
    {
        [Title("仪器基本信息")]
        public string EquipmentName; //仪器名称

        [LabelText("命名空间(Namespaces)：")]
        public string Namespaces; //命名空间

        [LabelText("脚本名称(scriptName)：")]
        public string scriptName;

        //碰撞体数据
        public ColliderData colliderData; //碰撞体数据
        public TransformData transformData;


        [Title("模型数据")]
        public List<EquipmentModelData> modelDatas;
        [Title("特效数据")]
        public List<EquipmentModelData> effectDatas;

        [Title("子仪器")]
        public List<EquipmentGenerateInfo> childs;

        private void OnGUI()
        {
            if (Application.isPlaying) return;

            Debug.Log("一直执行");
        }

        private void OnDestroy()
        {
            if (Application.isPlaying) return;
            Debug.Log("移除，呵呵");

            DestroyImmediate(gameObject);
        }

        [Button("创建")]
        public void OnCreate()
        {

        }
    }

#endif

}

