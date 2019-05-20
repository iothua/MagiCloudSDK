using MagiCloud.Core;
using MagiCloud.Core.Events;
using MagiCloud.Core.UI;
using UnityEngine;

namespace MagiCloud.KGUI
{
    /*
     * 滚动思路：
     * 1、获取到按下时，当进行移动时。进行滚动。
     * 2、一种是移动本身，另一种是移动其他。
     * 3、当释放时，停止移动。
     * 
     * 
     * 
     * 滚动带动背景思路：
     * 1、获取到该滚动条基于背景的最大值、最小值
     * 2、默认显示百分比
     * 3、获取到可移动对象的最大高度/宽度
     * 4、
    */

    public enum SliderType
    {
        /// <summary>
        /// 只抓取物体进行滚动
        /// </summary>
        None,
        /// <summary>
        /// 只抓取滚轮进行滚动
        /// </summary>
        Bar,
        /// <summary>
        /// 抓取两者都可以进行滚动
        /// </summary>
        All
    }

    /// <summary>
    /// 滚动条
    /// </summary>
    [ExecuteInEditMode]
    public class KGUI_Slider : KGUI_ButtonBase
	{
        public Axis KguiAxis = Axis.X;

        public SliderType sliderType = SliderType.None;

        public Horizontal horizontal = Horizontal.LeftToRight;
        public Vertical vertical = Vertical.TopToBottom;

		[Header("范围值")]
		public float minValue, maxValue;

		public float moveSpeed = 10.0f;

        public GameObject sliderObject;

        private bool IsDown = false;

        private int handIndex = -1; //同一时间段，只能是一个一只手操作滚动

        public RectTransform rectMove;//需要进行滚动的对象

        //public bool IsFullBar = false;//是否填充Bar

        [Range(0,1)]
        public float Value = 0;

        private float tempValue = 0;

        private float sumValue;//最大值，最小值的值

        private float size = 10;//容器大小

        private float parentSize; //父容器大小

        //激活滚轮
        public bool IsActive = true;

        /// <summary>
        /// 是否设置滚轮完成
        /// </summary>
        public bool IsSetScroll { get; set; }

        public EventFloat OnValueChanged;

        /// <summary>
        /// 可移动的大小，如果是X轴，则是宽度。如果是Y轴，则是高度
        /// </summary>
        public float RectMoveSize {
            get {
                return size;
            }
            set {
                size = value;
            }
        }

        protected override void Start()
        {
            base.Start();

            EventHandIdle.AddListener(OnButtonRelease, ExecutionPriority.High);

            //KinectEventHandIdle.AddListener(EventLevel.A, OnButtonRelease);

            if (sliderType == SliderType.Bar && !IsSetScroll)
                SetRectData();

            sumValue = Mathf.Abs(minValue - maxValue); //计算总长度

            SetChangingValue(Value);
        }

        /// <summary>
        /// 设置RectTransform基本数据
        /// </summary>
        public void SetRectData()
        {
            //根据对象容器，进行填充，并且设置最左移动点和最右移动点

            //所有的边界值就要进行计算了

            if (KguiAxis == Axis.X)
            {
                //设置顶点
                rectMove.pivot = new Vector2(0, 0.5f);
                //设置
                rectMove.anchorMin = new Vector2(0, 0.5f);
                rectMove.anchorMax = new Vector2(0, 0.5f);

                //rectMove.transform.localPosition = new Vector3(0, rectMove.transform.localPosition.y, rectMove.transform.localPosition.z);

                size = rectMove.sizeDelta.x;
                parentSize = rectMove.parent.GetComponent<RectTransform>().sizeDelta.x;

                float scale = size > parentSize ? size / parentSize : 1;

                //计算出滚轮的大小
                RectTransform bar = sliderObject.GetComponent<RectTransform>();

                //设置滚轮的瞄点
                bar.pivot = new Vector2(0, 0.5f);
                bar.anchorMin = new Vector2(0, 0.5f);
                bar.anchorMax = new Vector2(0, 0.5f);

                RectTransform barParent = sliderObject.transform.parent.GetComponent<RectTransform>();

                float x = barParent.sizeDelta.x / scale;

                bar.sizeDelta = new Vector2(x, bar.sizeDelta.y);

                //计算可移动的最大最小值
                minValue = -barParent.sizeDelta.x / 2;
                maxValue = barParent.sizeDelta.x / 2 - bar.sizeDelta.x;

                bar.transform.localPosition = new Vector3(-barParent.sizeDelta.x / 2, bar.transform.localPosition.y, bar.transform.localPosition.z);

                //设置碰撞体
                var barBox = bar.GetComponent<BoxCollider>();
                barBox.size = bar.sizeDelta;
                barBox.center = new Vector3(bar.sizeDelta.x / 2, bar.transform.localPosition.y, bar.transform.localPosition.z);
            }

            if (KguiAxis == Axis.Y)
            {
                //设置顶点
                rectMove.pivot = new Vector2(0.5f, 1);
                //设置
                rectMove.anchorMin = new Vector2(0.5f, 1);
                rectMove.anchorMax = new Vector2(0.5f, 1);

                size = rectMove.sizeDelta.y;
                parentSize = rectMove.parent.GetComponent<RectTransform>().sizeDelta.y;


                //计算出需要滚动容器与该容器的父物体比例
                float scale = size > parentSize ? size / parentSize : 1;

                //计算出滚轮的大小
                RectTransform bar = sliderObject.GetComponent<RectTransform>();

                //设置滚轮的瞄点
                bar.pivot = new Vector2(0.5f, 1);
                bar.anchorMin = new Vector2(0.5f, 1);
                bar.anchorMax = new Vector2(0.5f, 1);

                RectTransform barParent = sliderObject.transform.parent.GetComponent<RectTransform>();

                //一定要有物体去填充，并且对应好
                float y = barParent.sizeDelta.y / scale;

                //重新计算高度
                bar.sizeDelta = new Vector2(bar.sizeDelta.x, y);

                //在设置顶边界和底边界
                minValue = barParent.sizeDelta.y / 2;
                maxValue = bar.sizeDelta.y - minValue;//根据滚轮的高度-最顶值，得到范围内的值

                //设置坐标
                bar.transform.localPosition = new Vector3(bar.transform.localPosition.x, barParent.sizeDelta.y / 2, bar.transform.localPosition.z);

                //设置碰撞体大小
                var barBox = bar.GetComponent<BoxCollider>();

                barBox.size = bar.sizeDelta;
                barBox.center = new Vector3(bar.transform.localPosition.x, -bar.sizeDelta.y / 2, bar.transform.localPosition.z);
            }


            //获取到容器对象后，进行设置。然后滚轮填充，设置完成。
            IsSetScroll = true;

            IsActive = size <= parentSize ? false : true;

            transform.parent.gameObject.SetActive(IsActive);
        }

        /// <summary>
        /// 设置变化值
        /// </summary>
        /// <param name="value"></param>
        public void SetChangingValue(float value)
        {
            if (sliderObject == null)
            {
                Debug.LogError("未指定当前滚动对象：sliderObject属性为Null");
                return;
            }

            if (sliderType == SliderType.Bar)
            {
                if (rectMove == null)
                {
                    Debug.LogError("未指定当前滚动对象：rectMove属性为Null");
                    return;
                }
            }

            Vector3 position = Vector3.zero;
            float moveValue = 0;

            switch (KguiAxis)
            {
                case Axis.X:

                    ////将方向调整
                    if (horizontal == Horizontal.RightToLeft)
                    {
                        value = 1 - value;
                    }

                    moveValue = minValue + value * sumValue;

                    Value = value;

                    if (horizontal == Horizontal.RightToLeft)
                        Value = 1 - value;

                    position = new Vector3(moveValue, sliderObject.transform.localPosition.y, sliderObject.transform.localPosition.z);

                    if (sliderType == SliderType.Bar)
                    {
                        float xValue = -(Value * size + parentSize / 2);

                        float x = Mathf.Clamp(xValue, -(size - parentSize / 2), minValue);
                        rectMove.transform.localPosition = new Vector3(x, rectMove.transform.localPosition.y, rectMove.transform.localPosition.z);

                        Debug.Log(rectMove.transform.localPosition);
                    }

                    break;
                case Axis.Y:

                    //将方向调整
                    if (vertical == Vertical.TopToBottom)
                        value = 1 - value;

                    //计算出当前值
                    moveValue = maxValue + value * sumValue;

                    Value = value;

                    //将方向调整
                    if (vertical == Vertical.TopToBottom)
                        Value = 1 - value;

                    position = new Vector3(sliderObject.transform.localPosition.x, moveValue, sliderObject.transform.localPosition.z);

                    //if (sliderType == SliderType.Bar)
                    //    rectMove.transform.localPosition = new Vector3(rectMove.transform.localPosition.x, Value * size + parentSize / 2, rectMove.transform.localPosition.z);

                    if (sliderType == SliderType.Bar)
                    {
                        var y = Mathf.Clamp(Value * size + parentSize / 2, minValue, size - parentSize + minValue);
                        rectMove.transform.localPosition = new Vector3(rectMove.transform.localPosition.x, y, rectMove.transform.localPosition.z);
                    }

                    break;

                case Axis.Z:

                    //To...Do

                    position = new Vector3(sliderObject.transform.localPosition.x, sliderObject.transform.localPosition.y, moveValue);

                    if (sliderType == SliderType.Bar)
                        rectMove.transform.localPosition = new Vector3(rectMove.transform.localPosition.x, rectMove.transform.localPosition.y, Value * size + parentSize / 2);

                    break;
            }

            sliderObject.transform.localPosition = position;

            if (OnValueChanged != null)
                OnValueChanged.Invoke(Value);

            tempValue = Value;
        }

        protected override void OnDestroy()
        {
            EventHandIdle.RemoveListener(OnButtonRelease);
        }

        void OnButtonRelease(int handIndex)
        {
            if (!enabled) return;

            //如果当前手的值不等于-1，并且释放时，跟当前手不符，则直接跳过
            if (this.handIndex != -1 && this.handIndex != handIndex) return;

            IsDown = false;
            handIndex = -1;
        }

        public override void OnDown(int handIndex)
        {
            if (!enabled) return;
            base.OnDown(handIndex);

            if (this.handIndex != -1 && this.handIndex != handIndex) return;

            this.handIndex = handIndex;

            IsDown = true;
        }

        private void Update()
        {

            if (IsDown && handIndex != -1 && enabled)
            {
                //屏幕坐标
                Vector3 screenPoint = MOperateManager.GetHandScreenPoint(handIndex);

                Vector3 screenDevice = MUtility.MarkWorldToScreenPoint(sliderObject.transform.position);
                Vector3 vPos = MUtility.MarkScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, screenDevice.z));

                OnExecute(vPos);
            }

            //if (tempValue != Value)
            //{
            //    SetChangingValue(Value);
            //}

        }

        public void OnExecute(Vector3 handPosition)
        {
            Vector3 position = handPosition;
            Vector3 vpos;
            switch (KguiAxis)
            {
                case Axis.X:

                    //防抖
                    if (Mathf.Abs(position.x - sliderObject.transform.position.x) < 0.005f) return;

                    position.y = sliderObject.transform.position.y;
                    position.z = sliderObject.transform.position.z;

                    //moveObject.transform.position = Vector3.Lerp(moveObject.transform.position, position, Time.deltaTime * moveSpeed);
                    sliderObject.transform.position = position;

                    //获取到局部坐标
                    vpos = sliderObject.transform.localPosition;

                    vpos.x = Mathf.Clamp(vpos.x, minValue, maxValue);

                    if (horizontal == Horizontal.RightToLeft)
                        vpos.x = -vpos.x;

                    if (sliderType == SliderType.None)
                        Value = vpos.x / sumValue + 0.5f;
                    else
                        Value = (vpos.x + sliderObject.GetComponent<RectTransform>().sizeDelta.x / 2) / sumValue + 0.5f;

                    Value = Mathf.Clamp(Value, 0, 1);

                    if (sliderType == SliderType.Bar)
                        rectMove.transform.localPosition = new Vector3(Value * size + parentSize / 2, rectMove.transform.localPosition.y, rectMove.transform.localPosition.z);

                    SetChangingValue(Value);

                    break;
                case Axis.Y:

                    //防抖
                    if (Mathf.Abs(position.y - sliderObject.transform.position.y) < 0.005f) return;

                    position.x = sliderObject.transform.position.x;
                    position.z = sliderObject.transform.position.z;

                    sliderObject.transform.position = position;

                    //获取到局部坐标
                    vpos = sliderObject.transform.localPosition;

                    //判断此时Y轴的值在范围内没
                    vpos.y = Mathf.Clamp(vpos.y, maxValue, minValue);

                    //将方向调整
                    if (vertical == Vertical.TopToBottom)
                        vpos.y = -vpos.y;

                    if (sliderType == SliderType.None)
                        Value = vpos.y / sumValue + 0.5f;
                    else
                        Value = (vpos.y + sliderObject.GetComponent<RectTransform>().sizeDelta.y / 2) / sumValue + 0.5f;

                    Value = Mathf.Clamp(Value, 0, 1);

                    SetChangingValue(Value);

                    break;
                case Axis.Z:

                    //防抖
                    if (Mathf.Abs(position.z - sliderObject.transform.position.z) < 0.005f) return;

                    position.x = sliderObject.transform.position.x;
                    position.y = sliderObject.transform.position.y;

                    sliderObject.transform.position = Vector3.Lerp(sliderObject.transform.position, position, Time.deltaTime * moveSpeed);

                    vpos = sliderObject.transform.localPosition;

                    vpos.z = Mathf.Clamp(vpos.z, minValue, maxValue);

                    sliderObject.transform.localPosition = vpos;

                    Value = vpos.z / sumValue + 0.5f;

                    if (sliderType == SliderType.Bar)
                        rectMove.transform.localPosition = new Vector3(rectMove.transform.localPosition.x, rectMove.transform.localPosition.y, Value * size + parentSize / 2);

                    break;
            }
        }
	}
}

