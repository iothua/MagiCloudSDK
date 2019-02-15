using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{
    public class ScrollViewInfo
    {
        public float minValue;
        public float maxValue;

        public float currentValue;//当前值
    }


    /// <summary>
    /// KGUI滚动视图
    /// </summary>
    public class KGUI_ScrollView :MonoBehaviour
    {
        public KGUI_ScrollBar vertical; //垂直滚动
        public KGUI_ScrollBar horizontal; //水平滚动
        public KGUI_Panel panel; //容器

        public RectTransform content;//进行滚轮

        private ScrollViewInfo viewInfoX;
        private ScrollViewInfo viewInfoY;
        public bool isFollowHand = false;
        //y
        public int initNum = 4;
        private float unitSize;
        private float sumNum;
        private int curHand = -1;
        private Vector3 recordHandPos;
        private Vector3 recordPos;

        //x
        public int initXNum = 4;
        private float unitXSize;
        private float sumXNum;
        private int curXHand = -1;
        private Vector3 recordXHandPos;
        private Vector3 recordXPos;
        private void Start()
        {
            SetRectData();
            if (panel != null)
            {
                if (vertical != null)
                {
                    panel.onDown.AddListener(OnDown);
                    panel.onDirectionY.AddListener(PanelScrollY);
                    panel.onUp.AddListener(OnUp);
                }

                if (horizontal != null)
                {
                    panel.onUp.AddListener(OnXUp);
                    panel.onDown.AddListener(OnXDown);
                    panel.onDirectionX.AddListener(PanelScrollX);
                }
            }
        }


        private void OnUp(int arg0,bool arg1)
        {
            var pos = content.localPosition;
            if ((pos.y-viewInfoY.minValue)%unitSize>0.5f)
            {
                pos.y=viewInfoY.minValue+((int)((pos.y-viewInfoY.minValue)/unitSize)+1)*unitSize;
            }
            else if ((pos.y-viewInfoY.minValue)%unitSize<=0.5f)
            {
                pos.y=viewInfoY.minValue+((int)((pos.y-viewInfoY.minValue)/unitSize))*unitSize;
            }
            pos.y=Mathf.Clamp(pos.y,viewInfoY.minValue,viewInfoY.maxValue);
            content.localPosition=pos;
        }
        private void OnXUp(int arg0,bool arg1)
        {
            var pos = content.localPosition;
            if ((pos.x-viewInfoX.minValue)%unitSize>0.5f)
            {
                pos.x=viewInfoX.minValue+((int)((pos.x-viewInfoX.minValue)/unitSize)+1)*unitSize;
            }
            else if ((pos.x-viewInfoX.minValue)%unitSize<=0.5f)
            {
                pos.x=viewInfoX.minValue+((int)((pos.x-viewInfoX.minValue)/unitSize))*unitSize;
            }
            pos.x=Mathf.Clamp(pos.x,viewInfoX.minValue,viewInfoX.maxValue);
            content.localPosition=pos;
        }
        private void OnDown(int arg0,bool arg1)
        {
            curHand=arg0;
            recordHandPos= MOperateManager.GetHandScreenPoint(curHand);
            recordPos=content.localPosition;
        }
        private void OnXDown(int arg0,bool arg1)
        {
            curXHand=arg0;
            recordXHandPos= MOperateManager.GetHandScreenPoint(curXHand);
            recordXPos=content.localPosition;
        }
        /// <summary>
        /// 设置RectData
        /// </summary>
        public void SetRectData()
        {
            //根据视口与滚轮

            if (vertical != null)
            {
                //根据
                viewInfoY = new ScrollViewInfo();

                float size = content.sizeDelta.y;
                float parentSize = content.parent.GetComponent<RectTransform>().sizeDelta.y;
                unitSize = parentSize/initNum;
                sumNum = (int)(size/unitSize);

                //计算出父容器与当前容器的比例
                float scale = size > parentSize ? size / parentSize : 1;
                //根据比例，设置其值，同时当进行滑动时，也要同时设置滚轮的相应值

                //同时要计算出最小范围和最大范围
                if (size > parentSize)
                {
                    viewInfoY.minValue = -(size - parentSize) / 2;
                    viewInfoY.maxValue = (size - parentSize) / 2;
                }
                else
                {
                    viewInfoY.minValue = 0;
                    viewInfoY.maxValue = 0;
                }

                vertical.IsFullHandle = true; //是否填充滚轮
                vertical.Size = 1 / scale; //设置容器

                if (!isFollowHand)
                    //添加事件
                    vertical.OnValueChanged.AddListener(BindingVerticalValue);

                //如果高度一样
                if (size <= parentSize)
                {
                    vertical.IsEnable = false;//禁止滚动条

                    RectTransform rectParent = content.parent.GetComponent<RectTransform>();
                    rectParent.sizeDelta = new Vector2(rectParent.parent.GetComponent<RectTransform>().sizeDelta.x,rectParent.sizeDelta.y);
                    rectParent.localPosition = new Vector3(0,rectParent.localPosition.y,rectParent.localPosition.z);

                    var parentBox = rectParent.GetComponent<BoxCollider>();
                    parentBox.size = new Vector3(rectParent.sizeDelta.x,parentBox.size.y,parentBox.size.z);
                    parentBox.center = Vector3.zero;

                    content.sizeDelta = new Vector2(rectParent.sizeDelta.x,content.sizeDelta.y);

                    //var contentBox = rectParent.GetComponent<BoxCollider>();
                    //contentBox.size = new Vector3(content.sizeDelta.x, contentBox.size.y, contentBox.size.z);
                    //contentBox.center = Vector3.zero;
                }
                else
                {
                    vertical.IsEnable = true;

                    RectTransform rectParent = content.parent.GetComponent<RectTransform>();
                    //根据滚动条的X轴宽度和RectParent的父对象，去计算出此时该容器的大小
                    rectParent.sizeDelta = new Vector2(rectParent.parent.GetComponent<RectTransform>().sizeDelta.x -
                        vertical.handleRect.sizeDelta.x,rectParent.sizeDelta.y);

                    //计算出匹配的坐标
                    rectParent.localPosition = new Vector3(-vertical.handleRect.sizeDelta.x / 2,rectParent.localPosition.y,rectParent.localPosition.z);


                    var parentBox = rectParent.GetComponent<BoxCollider>();
                    parentBox.size = new Vector3(rectParent.sizeDelta.x,rectParent.sizeDelta.y,parentBox.size.z);
                    parentBox.center = Vector3.zero;

                    content.sizeDelta = new Vector2(rectParent.sizeDelta.x,content.sizeDelta.y);
                    var pos = content.localPosition;
                    pos.y =(rectParent.sizeDelta.y-content.sizeDelta.y)*0.5f;
                    content.localPosition=pos;
                    //var contentBox = rectParent.GetComponent<BoxCollider>();
                    //contentBox.size = new Vector3(content.sizeDelta.x, contentBox.size.y, contentBox.size.z);
                    //contentBox.center = Vector3.zero;
                }
            }

            if (horizontal != null)
            {
                viewInfoX = new ScrollViewInfo();

                float size = content.sizeDelta.x;
                float parentSize = content.parent.GetComponent<RectTransform>().sizeDelta.x;
                unitXSize = parentSize/initXNum;
                sumXNum = (int)(size/unitXSize);
                //计算出父容器与当前容器的比例
                float scale = size > parentSize ? size / parentSize : 1;
                //根据比例，设置其值，同时当进行滑动时，也要同时设置滚轮的相应值

                //同时要计算出最小范围和最大范围
                if (size > parentSize)
                {
                    viewInfoX.minValue = -(size - parentSize) / 2;
                    viewInfoX.maxValue = (size - parentSize) / 2;
                }
                else
                {
                    viewInfoX.minValue = 0;
                    viewInfoX.maxValue = 0;
                }

                horizontal.IsFullHandle = true;
                horizontal.Size = 1 / scale;
                if (!isFollowHand)
                    horizontal.OnValueChanged.AddListener(BindingHorizontalValue);

                if (size <= parentSize)
                {
                    horizontal.IsEnable = false;
                    //To……Do
                }
                else
                {
                    horizontal.IsEnable = true;
                    //To……Do
                }
            }


        }



        /// <summary>
        ///  绑定垂直滚动数据
        /// </summary>
        /// <param name="value"></param>
        public void BindingVerticalValue(float value)
        {
            //当值更新时，进行滚动
            float y = viewInfoY.minValue + Mathf.Abs(viewInfoY.maxValue - viewInfoY.minValue) * value;
            viewInfoY.currentValue = y;

            content.localPosition = new Vector3(content.localPosition.x,y,content.localPosition.z);
        }

        /// <summary>
        /// 绑定水平滚动数据
        /// </summary>
        /// <param name="value"></param>
        public void BindingHorizontalValue(float value)
        {
            float x = viewInfoX.minValue + Mathf.Abs(viewInfoX.maxValue - viewInfoX.minValue) * value;
            viewInfoX.currentValue = x;

            content.localPosition = new Vector3(x,content.localPosition.y,content.localPosition.z);
        }


        /// <summary>
        /// 容器垂直滚动
        /// </summary>
        /// <param name="direction"></param>
        public void PanelScrollY(int direction)
        {
            if (isFollowHand)
            {
                var handPos = MOperateManager.GetHandScreenPoint(curHand);
                var y = recordPos.y+  handPos.y-recordHandPos.y;
                y=Mathf.Clamp(y,viewInfoY.minValue,viewInfoY.maxValue);
                var pos = content.localPosition;
                pos.y=y;
                content.localPosition=pos;
            }
            else
            {
                vertical.Value -= direction * 1.5f * Time.deltaTime;
            }
        }

        /// <summary>
        /// 容器水平滚动
        /// </summary>
        /// <param name="direction"></param>
        public void PanelScrollX(int direction)
        {
            if (isFollowHand)
            {
                var handXPos = MOperateManager.GetHandScreenPoint(curXHand);
                var x = recordXPos.x+  handXPos.x-recordXHandPos.x;
                x=Mathf.Clamp(x,viewInfoX.minValue,viewInfoX.maxValue);
                var pos = content.localPosition;
                pos.x=x;
                content.localPosition=pos;
            }
            else
            {
                horizontal.Value -= direction * 1.5f * Time.deltaTime;
            }
        }
    }
}
