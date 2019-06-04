using System.Collections.Generic;
using UnityEngine;
using System;


namespace MagiCloud
{
  
    public static partial class Utilitys
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
            if (parent != null)
                result.SetParent(parent);
            result.localPosition = Vector3.zero;
            result.localRotation = Quaternion.identity;
            result.localScale = Vector3.one;
            return t;
        }




        /// <summary>
        /// 根据名称添加脚本
        /// </summary>
        /// <returns>The equipment by name.</returns>
        /// <param name="t">T.</param>
        /// <param name="name">Name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Component AddEquipmentByName<T>(this T t,string name) where T : Component
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
            if (isLocal)
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

        /// <summary>
        /// 获得网格最小点，默认支持蒙皮网格
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector3 BoundsMin(this GameObject target,bool inChildren = true,bool skinnedMesh = true)
        {
            Renderer[] renderers;
            Vector3 sizeMin = Vector3.zero;
            if (target != null)
            {
                if (CheckTheConditions(target,out renderers,inChildren,skinnedMesh))
                {
                    Bounds[] bounds = RenderersToBounds(renderers);
                    sizeMin = bounds[0].min;
                    foreach (Bounds bound in bounds)
                    {
                        Vector3 boundsMin = bound.min;
                        sizeMin.x = Mathf.Min(sizeMin.x,boundsMin.x);
                        sizeMin.y = Mathf.Min(sizeMin.y,boundsMin.y);
                        sizeMin.z = Mathf.Min(sizeMin.z,boundsMin.z);
                    }
                }
            }
            return sizeMin;
        }

        /// <summary>
        /// 获得网格最大点，默认支持蒙皮网格
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector3 BoundsMax(this GameObject target,bool inChildren = true,bool skinnedMesh = true)
        {
            Renderer[] renderers;
            Vector3 sizeMax = Vector3.zero;
            if (CheckTheConditions(target,out renderers,inChildren,skinnedMesh))
            {
                Bounds[] bounds = RenderersToBounds(renderers);
                sizeMax = bounds[0].max;
                foreach (Bounds bound in bounds)
                {
                    Vector3 boundsMax = bound.max;
                    sizeMax.x = Mathf.Max(sizeMax.x,boundsMax.x);
                    sizeMax.y = Mathf.Max(sizeMax.y,boundsMax.y);
                    sizeMax.z = Mathf.Max(sizeMax.z,boundsMax.z);
                }
            }
            return sizeMax;
        }

        /// <summary>
        /// 检查自己及子物体是否包含Renderers相关组件
        /// </summary>
        private static bool CheckTheConditions(GameObject obj,out Renderer[] renderers,bool inChildren = true,bool skinnedMesh = true)
        {
            List<Renderer> tmpRenderers = new List<Renderer>();

            //网格渲染器与蒙皮网格渲染器的获取
            MeshRenderer[] meshRenderers = null;
            SkinnedMeshRenderer[] skinnedMeshRenderers = null;
            if (obj != null)
            {
                if (inChildren)
                {
                    meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
                    if (skinnedMesh)
                        skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                }
                else
                {
                    meshRenderers = obj.GetComponentInChildren<MeshRenderer>() == null ? null : new MeshRenderer[] { obj.GetComponentInChildren<MeshRenderer>() };
                    if (skinnedMesh)
                        skinnedMeshRenderers = obj.GetComponentInChildren<SkinnedMeshRenderer>() == null ? null : new SkinnedMeshRenderer[] { obj.GetComponentInChildren<SkinnedMeshRenderer>() };
                }

                //集合不同网格的Renderers
                if (meshRenderers != null)
                {
                    foreach (var meshRenderer in meshRenderers)
                    {
                        //检测 获取的Bounds是否符合条件
                        if (meshRenderer.bounds.size != Vector3.zero)
                            tmpRenderers.Add(meshRenderer);
                    }
                }

                if (skinnedMeshRenderers != null)
                {
                    foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
                    {
                        if (skinnedMeshRenderer.bounds.size != Vector3.zero)
                            tmpRenderers.Add(skinnedMeshRenderer);
                    }
                }
            }

            //赋值OUT关键字参数
            renderers = tmpRenderers.ToArray();
            if (renderers.Length == 0)
            {
                if (obj != null)
                {
                    Debug.LogError(obj.name + "及子物体没有Renderer相关组件");
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// 将Renderer转换为Bound
        /// </summary>
        /// <param name="renderers"></param>
        /// <returns></returns>
        private static Bounds[] RenderersToBounds(Renderer[] renderers)
        {
            List<Bounds> bounds = new List<Bounds>();
            foreach (var item in renderers)
            {
                bounds.Add(item.bounds);
            }
            return bounds.ToArray();
        }
    }
}
