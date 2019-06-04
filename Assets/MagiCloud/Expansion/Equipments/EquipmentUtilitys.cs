using System.Collections.Generic;
using UnityEngine;
using System;
using MagiCloud.Equipments;

namespace MagiCloud
{
    public static partial class Utilitys
    {
        /// <summary>
        /// 根据脚本信息，添加命名空间
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="namespaces">Namespaces.</param>
        /// <param name="scriptName">Script name.</param>
        public static T AddEquipmentScript<T>(this Transform transform,string namespaces,string scriptName)
            where T : EquipmentBase
        {
            if (string.IsNullOrEmpty(scriptName)) return default(T);

            string script = !string.IsNullOrEmpty(namespaces) ? namespaces + "." + scriptName : scriptName;
            var component = transform.AddEquipmentByName(script);

            return (T)component;
        }
    }
}
