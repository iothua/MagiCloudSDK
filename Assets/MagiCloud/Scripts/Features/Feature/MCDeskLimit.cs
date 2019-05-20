using MagiCloud.Core.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MagiCloud.Equipments;

namespace MagiCloud.Features
{
    /// <summary>
    /// 桌面吸附及限制下陷
    /// </summary>
    public class MCDeskLimit :MonoBehaviour
    {
        public GameObject limitObj;
        public bool openAdsorption = true;  //开启桌面吸附
        public bool limitSink = true;       //限制下陷到桌面下

        public BoxCollider boxCollider;     //用于空间查询的BoxCollider
        [Header("桌面的高度")]
        public float deskHeight = -7f;
        [Header("距离桌面多高开始吸附")]
        public float deskDistance = 4;
        public bool autoExtremum = true;           //自动计算网格极值
        public Transform minPoint;          //手动赋值网格极小值
        public Transform maxPoint;          //手动赋值网格极大值

        private Vector3 meshMin;
        private bool isGrab;
        private LayerMask layerMask = 1 << 26;
        private Ray ray;
        private RaycastHit hitInfo;
        private float initDeskHeight;



        void Awake()
        {
            EventHandGrabObject.AddListener(OnGrab,Core.ExecutionPriority.High);
            EventHandReleaseObject.AddListener(OnIdle,Core.ExecutionPriority.High);
            initDeskHeight = deskHeight;
        }

        public void OnDestroy()
        {
            EventHandGrabObject.RemoveListener(OnGrab);
            EventHandReleaseObject.RemoveListener(OnIdle);
        }

        private void OnGrab(GameObject grabObj,int index)
        {
            if (grabObj != limitObj) return;
            isGrab = true;
        }

        private void OnIdle(GameObject grabObj,int index)
        {
            if (grabObj != limitObj) return;
            //if (!openAdsorption) return;
            if (transform.parent.parent != null && transform.parent.parent.gameObject.GetComponent<EquipmentBase>()) return;
            isGrab = false;
            StartCoroutine(SetDrop());
        }
        IEnumerator SetDrop()
        {
            yield return new WaitForEndOfFrame();
            if (openAdsorption)
            {
                CalculationDeskHeight_Update();

                if (autoExtremum)
                    meshMin = limitObj.BoundsMin(skinnedMesh: true);
                else
                    meshMin = minPoint.position;

                float distance = meshMin.y - deskHeight;
                if (distance >= 0)
                {
                    if (distance <= deskDistance)
                    {
                        limitObj.transform.DOMoveY(limitObj.transform.position.y - distance,distance * 0.2f).SetEase(Ease.InCubic);
                    }
                }
            }
            StopCoroutine(SetDrop());
        }

        private void LateUpdate()
        {
            if (!limitSink) return;
            if (!isGrab) return;
            if (transform.parent.parent != null && transform.parent.parent.gameObject.GetComponent<EquipmentBase>()) return;
            Vector3 curPos = limitObj.transform.position;

            if (autoExtremum)
                meshMin = limitObj.BoundsMin(skinnedMesh: true);
            else
                meshMin = minPoint.position;

            float tempY = deskHeight + limitObj.transform.position.y - meshMin.y;
            curPos.y = Mathf.Clamp(curPos.y,tempY,float.MaxValue);
            limitObj.transform.position = curPos;
            CalculationDeskHeight_Update();
        }
        /// <summary>
        /// 根据碰撞体计算桌面高度
        /// </summary>
        private void CalculationDeskHeight_Update()
        {

            if (boxCollider == null)
            {
                ray = new Ray(limitObj.transform.position,Vector3.down);
                if (Physics.Raycast(ray,out hitInfo,1000,layerMask.value))   //boxCollider不赋值，如果DeskLimiCheck碰撞体高于limitObj的Y值会失效
                {
                    deskHeight = hitInfo.point.y;
                }
                else
                {
                    deskHeight = initDeskHeight;
                }
                return;
            }
            Vector3 size = new Vector3(boxCollider.size.x * boxCollider.gameObject.transform.localScale.x,
                                        boxCollider.size.y * boxCollider.gameObject.transform.localScale.y,
                                        boxCollider.size.z * boxCollider.gameObject.transform.localScale.z) * 0.5f;
            Collider[] colliders = Physics.OverlapBox(limitObj.transform.position,size,Quaternion.identity,layerMask.value);
            if (colliders.Length == 0)
            {
                ray = new Ray(limitObj.transform.position,Vector3.down);
                if (Physics.Raycast(ray,out hitInfo,1000,layerMask.value))
                {
                    deskHeight = hitInfo.point.y;
                }
                else
                {
                    deskHeight = initDeskHeight;
                }
                return;
            }
            foreach (var item in colliders)
            {
                deskHeight = Mathf.Max(deskHeight,item.bounds.max.y);
            }
        }
    }
}
