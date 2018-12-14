using System.Collections.Generic;
using UnityEngine;
using MagiCloud.Equipments;
using System;

namespace MagiCloud
{
    public static class Utilitys
    {
        /// <summary>
        /// 从0-指定数中随机n个不重复数
        /// </summary>
        /// <param name="total">随机总数</param>
        /// <param name="n">几个数</param>
        /// <returns></returns>
        public static int[] GetRandomSequence(int total,int n)
        {
            //随机总数组  
            int[] sequence = new int[total];
            //取到的不重复数字的数组长度  
            int[] output = new int[n];

            for (int i = 0; i < total; i++)
            {
                sequence[i] = i;
            }

            int end = total - 1;

            for (int i = 0; i < n; i++)
            {
                //随机一个数，每随机一次，随机区间-1  
                int num = UnityEngine.Random.Range(0,end + 1);
                output[i] = sequence[num];
                //将区间最后一个数赋值到取到数上  
                sequence[num] = sequence[end];
                end--;
            }

            return output;
        }

        /// <summary>
        /// 重置transform的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="parent">父物体</param>
        /// <returns></returns>
        public static T ResetTransform<T>(this T t,Transform parent = null) where T : Component
        {
            Transform result = t.transform;
            if (parent!=null)
                result.SetParent(parent);
            result.localPosition=Vector3.zero;
            result.localRotation=Quaternion.identity;
            result.localScale=Vector3.one;
            return t;
        }


        /// <summary>
        /// 根据脚本信息，添加命名空间
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="namespaces">Namespaces.</param>
        /// <param name="scriptName">Script name.</param>
        public static T AddEquipmentScript<T>(this Transform transform,string namespaces,string scriptName)
            where T:EquipmentBase
        {
            if (string.IsNullOrEmpty(scriptName)) return default(T);

            string script = !string.IsNullOrEmpty(namespaces) ? namespaces + "." + scriptName : scriptName;
            var component = transform.AddEquipmentByName(script);

            return (T)component;
        }

        /// <summary>
        /// 根据名称添加脚本
        /// </summary>
        /// <returns>The equipment by name.</returns>
        /// <param name="t">T.</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Component AddEquipmentByName<T>(this T t, string name) where T : Component
        {
            Type type = null;
            type = Type.GetType(name);

            return t.gameObject.GetComponent(type) ?? t.gameObject.AddComponent(type);
        }

        /// <summary>
        /// 设置局部Tranform值
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="transformData">Transform data.</param>
        public static void SetTransform(this Transform transform,TransformData transformData,bool isLocal = true)
        {
            if(isLocal)
            {
                transform.localPosition = transformData.localPosition.Vector;
                transform.localRotation = Quaternion.Euler(transformData.localRotation.Vector);
                transform.localScale = transformData.localScale.Vector;
            }
            else
            {
                transform.position = transformData.localPosition.Vector;
                transform.rotation = Quaternion.Euler(transformData.localRotation.Vector);
                transform.localScale = transformData.localScale.Vector;
            }

        }

        /// <summary>
        /// 删除该物体下的所有子物体（编辑器下）
        /// </summary>
        /// <param name="transform"></param>
        public static void DestroyImmediateChildObject(this Transform transform)
        {
            foreach (Transform item in transform)
            {
                //删除下面所有的
                GameObject.DestroyImmediate(item.gameObject);
            }
        }

        /// <summary>
        /// 删除该物体下的所有子物体
        /// </summary>
        /// <param name="transform"></param>
        public static void DestroyChildObject(this Transform transform)
        {
            foreach (Transform item in transform)
            {
                //删除下面所有的
                GameObject.Destroy(item.gameObject);
            }
        }
    }
}
