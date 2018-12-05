using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DrawLine
{
    public class RectTransformZoomTool : MonoBehaviour
    {
        [Header("最大放大倍数"), Range(1, 10)]
        public float maxMultiple = 2.5f;
        private RectTransform _RectTransform;
        private RectTransform _ParentMask;

        private float _StartWidth;
        private float _StartHeight;
        private Vector3 _StartWorldPos;
        private float _StartWidthWorld;                     //世界坐标下的宽度
        private float _StartHeightWorld;                    //世界坐标下的高度

        private bool _IsInto = false;                        //鼠标是否在RawImage内  

        private Vector3 _First;                                  //  记录鼠标点击的初始位置  
        private Vector3 _Second;                                 //  记录鼠标移动时的位置

        public void OnPointerEnter(int handIndex)
        {
            _IsInto = true;
        }

        public void OnPointerExit(int handIndex)
        {
            _IsInto = false;
        }

        void Start()
        {
            _RectTransform = GetComponent<RectTransform>();
            _ParentMask = transform.parent.GetComponent<RectTransform>();
            if (!_ParentMask.GetComponent<RectMask2D>()) _ParentMask.gameObject.AddComponent<RectMask2D>();

            _StartWidth = _RectTransform.rect.width;
            _StartHeight = _RectTransform.rect.height;
            _StartWorldPos = _RectTransform.position;

            Vector3[] temp = new Vector3[4];
            _RectTransform.GetWorldCorners(temp);
            _StartWidthWorld = _RectTransform.position.x;// - temp[0].x;
            _StartHeightWorld = _RectTransform.position.y;// - temp[0].y;
        }

        void Update()
        {
            if (_IsInto)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    SetPivot(Input.mousePosition);
                    SetZoom(Input.mousePosition, "+");
                }
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    SetPivot(Input.mousePosition);
                    SetZoom(Input.mousePosition, "-");
                }
                if (Input.GetMouseButtonDown(0))
                {
                    _First = Input.mousePosition;
                }
                if (Input.GetMouseButton(0))
                {
                    OnDrag(Input.mousePosition);                    //图片跟随鼠标移动
                }
                if (Input.GetMouseButtonUp(0))                      //鼠标拖拽时还在图片内松开
                {
                    LimitPosition(0.2f);                            //在当前pivot下限制物体的拖拽范围
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    LimitPosition(0.2f);                            //鼠标拖拽时还在图片内松开
                }
            }
        }
        /// <summary>
        /// 根据screenPos坐标得到新pivot
        /// </summary>
        public void SetPivot(Vector3 screenPos)
        {
            Vector3[] temp = new Vector3[4];
            _RectTransform.GetLocalCorners(temp);
            Vector2 screenPointToLocalPointInRectangle;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_RectTransform, screenPos, MagiCloud.MUtility.UICamera, out screenPointToLocalPointInRectangle);
            screenPointToLocalPointInRectangle.x = Mathf.Clamp(screenPointToLocalPointInRectangle.x, temp[0].x, temp[2].x);
            screenPointToLocalPointInRectangle.y = Mathf.Clamp(screenPointToLocalPointInRectangle.y, temp[0].y, temp[2].y);

            Vector2 nextPivot;
            if (screenPointToLocalPointInRectangle.x >= 0)
                nextPivot.x = (screenPointToLocalPointInRectangle.x + Mathf.Abs(temp[0].x)) / (temp[2].x + Mathf.Abs(temp[0].x));
            else
                nextPivot.x = (Mathf.Abs(temp[0].x) - Mathf.Abs(screenPointToLocalPointInRectangle.x)) / (temp[2].x + Mathf.Abs(temp[0].x));

            if (screenPointToLocalPointInRectangle.y >= 0)
                nextPivot.y = (screenPointToLocalPointInRectangle.y + Mathf.Abs(temp[0].y)) / (temp[2].y + Mathf.Abs(temp[0].y));
            else
                nextPivot.y = (Mathf.Abs(temp[0].y) - Mathf.Abs(screenPointToLocalPointInRectangle.y)) / (temp[2].y + Mathf.Abs(temp[0].y));

            _RectTransform.pivot = nextPivot;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="param"></param>
        public void SetZoom(Vector3 screenPos, string param)
        {
            float currentWidth = _RectTransform.rect.width;
            float currentHeight = _RectTransform.rect.height;
            if (param == "+")
            {
                currentWidth *= 1.1f;
                currentHeight *= 1.1f;
            }
            if (param == "-")
            {
                currentWidth *= 0.9f;
                currentHeight *= 0.9f;
            }
            currentWidth = Mathf.Clamp(currentWidth, _StartWidth, maxMultiple * _StartWidth);                       //限制在1到maxMultiple倍之间
            currentHeight = Mathf.Clamp(currentHeight, _StartHeight, maxMultiple * _StartHeight);
            SetRectTransformSize(_RectTransform, new Vector2(currentWidth, currentHeight));                         //修改后赋值给rawImageRectTransform
                                                                                                                    //_RectTransform.position = MagiCloud.KGUI.KGUI_Utility.kguiCamera.WorldToScreenPoint(screenPos);         //将位置设置为screenPos   当改变一个物体的pivot时最重要的就是这句话
            LimitPosition(0);                                                                                       //防止缩放改变的位置超出指定范围
        }

        /// <summary>
        /// 抓取移动
        /// </summary>
        public void OnDrag(Vector3 screenPos)
        {
            _Second = screenPos - _First;                                                   //得到移动的方向向量
            _First = screenPos;
            _RectTransform.Translate(_Second * 0.01f);                                      //平移
        }

        /// <summary>
        /// 限制图片移动
        /// </summary>
        /// <param name="time">DoTween动画时间</param>
        public void LimitPosition(float time)
        {
            Vector3[] temp = new Vector3[4];                                                                                //temp 与temp2用于在屏幕坐标系下计算差值
            _RectTransform.GetWorldCorners(temp);
            temp[0] = MagiCloud.MUtility.UICamera.WorldToScreenPoint(temp[0]);
            temp[2] = MagiCloud.MUtility.UICamera.WorldToScreenPoint(temp[2]);

            Vector3[] temp2 = new Vector3[4];
            _ParentMask.GetWorldCorners(temp2);
            temp2[0] = MagiCloud.MUtility.UICamera.WorldToScreenPoint(temp2[0]);
            temp2[2] = MagiCloud.MUtility.UICamera.WorldToScreenPoint(temp2[2]);

            if (temp[0].x >= temp2[0].x)
            {
                _RectTransform.DOLocalMoveX(_RectTransform.localPosition.x - (temp[0].x - temp2[0].x), time);               //动画往左移动
            }
            if (temp[2].x <= temp2[2].x)
            {
                _RectTransform.DOLocalMoveX(_RectTransform.localPosition.x + (temp2[2].x - temp[2].x), time);               //动画往右移动
            }

            if (temp[0].y >= temp2[0].y)
            {
                _RectTransform.DOLocalMoveY(_RectTransform.localPosition.y - (temp[0].y - temp2[0].y), time);               //动画往下移动
            }
            if (temp[2].y <= temp2[2].y)
            {
                _RectTransform.DOLocalMoveY(_RectTransform.localPosition.y + (temp2[2].y - temp[2].y), time);               //动画往上移动
            }
        }

        /// <summary>
        /// 根据newSize设置新的trans大小
        /// </summary>
        /// <param name="rectTrans"></param>
        /// <param name="newSize"></param>
        public void SetRectTransformSize(RectTransform rectTrans, Vector2 newSize)
        {
            Vector2 oldSize = rectTrans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            rectTrans.offsetMin = rectTrans.offsetMin - new Vector2(deltaSize.x * rectTrans.pivot.x, deltaSize.y * rectTrans.pivot.y);
            rectTrans.offsetMax = rectTrans.offsetMax + new Vector2(deltaSize.x * (1f - rectTrans.pivot.x), deltaSize.y * (1f - rectTrans.pivot.y));
        }
    } 
}
