using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using MagiCloud;

namespace Chemistry.Data
{
    /// <summary>
    /// 容器类信息
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class DI_EquipmentInfo
    {
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string equipmentName;

        /// <summary>
        /// 资源名称
        /// </summary>
        public string resourcesName;

        /// <summary>
        /// 碰撞体大小
        /// </summary>
        public ColliderData colliderData;

        /// <summary>
        /// Transform数据
        /// </summary>
        public TransformData transformData;

        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespaces = "Chemistry.Equipments";

        /// <summary>
        /// 脚本名称
        /// </summary>
        public string scriptName;

        //子仪器
        public List<string> childEquipments = new List<string>();

        public DI_EquipmentInfo()
        {
            colliderData = new ColliderData();
            colliderData.Center = new MVector3();
            colliderData.Size = new MVector3();

            transformData = new TransformData();
            transformData.localPosition = new MVector3();
            transformData.localRotation = new MVector3();
            transformData.localScale = new MVector3();
        }
    }

    /// <summary>
    /// 药品名称
    /// </summary>
    [Serializable]
    public class DI_DrugInfo
    {
        public string drugName;
        public float drugVolume;

        public MVector3 drugPosition;

        public string drugModelName;
    }

    [Serializable]
    public class DI_EquipmentDrugInfo
    {
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string equipmentName;

        /// <summary>
        /// 容器容积
        /// </summary>
        public float sumVolume = 100;

        public List<DI_DrugInfo> drugInfos = new List<DI_DrugInfo>();
    }
}
