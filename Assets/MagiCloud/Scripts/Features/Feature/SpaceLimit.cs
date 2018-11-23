using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.Core.Events;

namespace MagiCloud.Features
{
    public class SpaceLimit : MonoBehaviour
    {
        public GameObject limitObj;
        public bool topLimit = true;
        public bool bottomLimit = true;
        public bool leftLimit = true;
        public bool rightLimit = true;
        public float offset = 0.5f;
        
        private Vector3 meshMin;
        private Vector3 meshMax;
        Coroutine coroutine;
        void Start()
        {

            EventHandGrabObject.AddListener(OnGrab, Core.ExecutionPriority.High);
            EventHandReleaseObject.AddListener(OnIdle, Core.ExecutionPriority.High);
        }

        private void OnDestroy()
        {
            EventHandGrabObject.RemoveListener(OnGrab);
            EventHandReleaseObject.RemoveListener(OnIdle);
        }
        /// <summary>
        /// 全部关闭空间限制
        /// </summary>
        public void CloseLimit()
        {
            topLimit = false;
            bottomLimit = false;
            leftLimit = false;
            rightLimit = false;
        }
        /// <summary>
        /// 全部打开空间限制
        /// </summary>
        public void OpenLimit()
        {
            topLimit = true;
            bottomLimit = true;
            leftLimit = true;
            rightLimit = true;
        }
        private void OnGrab(GameObject grabObj, int index)
        {


        }

        private void OnIdle(GameObject grabObj, int index)
        {
            if (grabObj == limitObj)
                coroutine = StartCoroutine(OutLimitReset());
        }
        void Update()
        {
            
        }

        /// <summary>
        /// 越界重置
        /// </summary>
        /// <returns></returns>
        IEnumerator OutLimitReset()
        {
            yield return new WaitForEndOfFrame();
            Vector3 limitObjPos = transform.position;
            meshMin = BoundsMin(limitObj);
            meshMax = BoundsMax(limitObj);
            Vector3 limitObjPosToScreen = Camera.main.WorldToScreenPoint(limitObjPos);
            Vector3 screenMinPointToWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, limitObjPosToScreen.z));
            Vector3 screenMaxPointToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, limitObjPosToScreen.z));
            if (topLimit)
            {
                if (meshMax.y >= screenMaxPointToWorld.y)   //上边越界
                {
                    float temp = meshMax.y - screenMaxPointToWorld.y;
                    limitObjPos.y -= temp + offset;
                }
            }
            if (bottomLimit)
            {
                if (meshMin.y <= screenMinPointToWorld.y)   //下边越界
                {
                    float temp = meshMin.y - screenMinPointToWorld.y;
                    limitObjPos.y -= temp - offset;
                }
            }
            if (leftLimit)
            {
                if (meshMin.x <= screenMinPointToWorld.x)   //左边越界
                {
                    float temp = meshMin.x - screenMinPointToWorld.x;
                    limitObjPos.x -= temp - offset;
                }
            }
            if (rightLimit)
            {
                if (meshMax.x >= screenMaxPointToWorld.x)   //右边越界
                {
                    float temp = meshMax.x - screenMaxPointToWorld.x;
                    limitObjPos.x -= temp + offset;
                }
            }

            limitObj.transform.position = limitObjPos;
            StopCoroutine(coroutine);
        }

        private Vector3 ScreenToWorldPos(Vector2 screenPos)
        {
            return Vector3.zero;
        }

        /// <summary>
        /// 网格最大点
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Vector3 BoundsMin(GameObject target)
        {
            Renderer[] renderers;
            Vector3 sizeMin = Vector3.zero;
            if (target != null)
            {
                if (CheckTheConditions(target, out renderers))
                {
                    Bounds[] bounds = RenderersToBounds(renderers);
                    sizeMin = bounds[0].min;
                    foreach (Bounds bound in bounds)
                    {
                        Vector3 boundsMin = bound.min;
                        sizeMin.x = Mathf.Min(sizeMin.x, boundsMin.x);
                        sizeMin.y = Mathf.Min(sizeMin.y, boundsMin.y);
                        sizeMin.z = Mathf.Min(sizeMin.z, boundsMin.z);
                    }
                }
            }
            return sizeMin;
        }

        /// <summary>
        /// 网格最小点
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Vector3 BoundsMax(GameObject target)
        {
            Renderer[] renderers;
            Vector3 sizeMax = Vector3.zero;
            if (CheckTheConditions(target, out renderers))
            {
                Bounds[] bounds = RenderersToBounds(renderers);
                sizeMax = bounds[0].max;
                foreach (Bounds bound in bounds)
                {
                    Vector3 boundsMax = bound.max;
                    sizeMax.x = Mathf.Max(sizeMax.x, boundsMax.x);
                    sizeMax.y = Mathf.Max(sizeMax.y, boundsMax.y);
                    sizeMax.z = Mathf.Max(sizeMax.z, boundsMax.z);
                }
            }
            return sizeMax;
        }
        /// <summary>
        /// 检查自己及子物体是否包含Renderers相关组件
        /// </summary>
        private bool CheckTheConditions(GameObject obj, out Renderer[] renderers, bool inChildren = true)
        {
            List<Renderer> tmpRenderers = new List<Renderer>();

            //网格渲染器与蒙皮网格渲染器的获取
            MeshRenderer[] meshRenderers;
            SkinnedMeshRenderer[] skinnedMeshRenderers;
            if (obj != null)
            {
                if (inChildren)
                {
                    meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
                    skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                }
                else
                {
                    meshRenderers = obj.GetComponentInChildren<MeshRenderer>() == null ? null : new MeshRenderer[] { obj.GetComponentInChildren<MeshRenderer>() };
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
                    Debug.LogError(obj.name + "及子物体没有Renderers属性相关组件");
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
        private Bounds[] RenderersToBounds(Renderer[] renderers)
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
