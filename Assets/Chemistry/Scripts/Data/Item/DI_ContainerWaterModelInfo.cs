using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using MagiCloud;

namespace Chemistry.Data
{
    /// <summary>
    /// 容器水体模型信息
    /// 开发者：阮榆皓
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    [System.Serializable]
    public class DI_ContainerWaterModelInfo : DataItemBase
    {
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string equipmentName;

        /// <summary>
        /// 仪器类型
        /// </summary>
        public EContainerType containerType;

        /// <summary>
        /// 水体模型名称
        /// </summary>
        public string modelName;

        /// <summary>
        /// 相对于容器的位置（仅Y值生效）
        /// </summary>
        public MVector3 pos;
        


        public DI_ContainerWaterModelInfo()
        {
            pos = new MVector3(0, 1.0f, 0);
        }
    }
}
