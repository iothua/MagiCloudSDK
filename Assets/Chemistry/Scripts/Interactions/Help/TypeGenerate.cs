using System;
using UnityEngine;
namespace Chemistry.Help
{
    /// <summary>
    /// 类型生成
    /// </summary>
    public static class TypeGenerate
    {
        /// <summary>
        /// 根据名字添加脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="name">全名</param>
        /// <returns></returns>
        public static Component AddComponentByName<T>(this T t,string name) where T : Component
        {
            Type type = null;
            type=Type.GetType(name);
            Component component = t.gameObject.AddComponent(type);
            return component;
        }
    }

}


