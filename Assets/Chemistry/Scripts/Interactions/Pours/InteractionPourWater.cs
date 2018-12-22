using MagiCloud.Interactive.Distance;
using MagiCloud.Interactive.Actions;
using MagiCloud.Interactive;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using MagiCloud;
using MagiCloud.Core.Events;
using Chemistry.Equipments;
using Chemistry.Data;
using Substance.Water;

namespace Chemistry.Interactions
{

    //待解决问题
    //1.抓取时根据左右手，只启用一个倒水点，关闭另一个

    public enum PourPointSide
    {
        Left,
        Right
    }

    [Serializable]
    public class EventPour : UnityEvent<InteractionPourWater> { }

    /// <summary>
    /// 倒水距离检测
    /// 开发者：阮榆皓
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(PourInterationHelper))]
    public class InteractionPourWater : InteractionEquipment
    {
        //需要手动去挂载此脚本
        public PourInterationHelper pourHelper;
        
        private Image ReadingImg;

        //上一帧光标的位置
        Vector2 lastMousePos;

        float curTime = 0.0f;

        ////[Header("倒水点")]--自己
        ////public Transform PourPt;
        //[Header("倒水仪器局部坐标")]
        //public Vector3 localPos = Vector3.zero;
        //[SerializeField,Header("倒水仪器局部旋转值")]
        //public Vector3 localRot = Vector3.zero;
        /// <summary>
        /// 倒水点控制器
        /// </summary>
        public PourContainer pourContainer;

        public EC_Container CurContainer;


        /// <summary>
        /// 倒水点在左边还是右边
        /// </summary>
        public PourPointSide pointSide;

        //[Header("进入倒水操作的读条时间")]
        //public float LimitTime = 2.0f;
        //private float curTimer = 0.0f;

        //是否吸附
        bool isAdsorbed = false;

        bool startPour = true;
        bool endPour = false;

        ////是否吸附完毕
        //bool adsorbOver = false;

        //进入倒水范围，开始倒水，倒水中，结束倒水，离开倒水范围
        public EventPour OnEnterPour, OnBeginPour, OnStayPour, OnEndPour, OnExitPour;

        protected override void Awake()
        {
            base.Awake();

            if (OnEnterPour == null)
            {
                OnEnterPour = new EventPour();
            }
            if (OnBeginPour == null)
            {
                OnBeginPour = new EventPour();
            }
            if (OnStayPour == null)
            {
                OnStayPour = new EventPour();
            }
            if (OnEndPour == null)
            {
                OnEndPour = new EventPour();
            }
            if (OnExitPour == null)
            {
                OnExitPour = new EventPour();
            }
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            pourHelper = this.GetComponent<PourInterationHelper>();

            if (pourHelper==null)
            {
                Debug.Log("PourInterationHelper脚本为空");
            }

            //if (pourHelper.ReadingImg == null)
            //{
            //    //Debug.Log("读条图片为空！！！");
            //}
        }

        protected override void Start()
        {
            base.Start();
            
            CurContainer = Equipment as EC_Container;
        }


        //void adsorbTo

        ///计算光标与空间中某点的屏幕距离
        float CaculateScreenPtDistance(Vector3 pt)
        {
            Vector3 tmpPt = new Vector3(MUtility.MainWorldToScreenPoint(pt).x, MUtility.MainWorldToScreenPoint(pt).y, 0);
            //Debug.Log("光标坐标"+MOperateManager.GetHandScreenPoint(0));
            //Debug.Log("倒水点坐标" + tmpPt);
            float tmpDistance = Vector3.Distance(MOperateManager.GetHandScreenPoint(0), tmpPt);
            //Debug.Log(tmpDistance);
            return tmpDistance;
        }


        #region 距离检测相关
        /// <summary>
        /// 交互第一个执行点 触发一次
        /// </summary>
        /// <param name="distanceInteraction">传过来的物体</param>
        /// <returns></returns>
        public override bool IsCanInteraction(DistanceInteraction distanceInteraction)
        {
            //倒水量允许
            //两个杯子可以相互交互条件允许
            //没有自锁

            var interaction = distanceInteraction as InteractionPourWater;
            if (interaction==null||interaction.pointSide==this.pointSide)
            {
                Debug.Log("不允许倒水交互");
                return false;
            }

            //if (true)
            //{

            //}


            return base.IsCanInteraction(distanceInteraction);
        }

        
        /// <summary>
        /// 在之前的执行完毕之后 进入点 触发一次
        /// </summary>
        /// <param name="distanceInteraction"></param>
        public override void OnDistanceEnter(DistanceInteraction distanceInteraction)
        {
            if (!IsCanInteraction(distanceInteraction)) return;

            //发出接近的通知
            var interaction = distanceInteraction as InteractionPourWater;

            OnEnterPourDistance(interaction);

            //被抓取的物体上显示读条UI
            if (IsGrab)
            {
                if (pourHelper.ReadingImgObj == null)
                {
                    Debug.Log(this.name);
                    GameObject tmpPrefabs = Resources.Load(pourHelper.PourImgPrefabsPath) as GameObject;
                    pourHelper.ReadingImgObj = Instantiate(tmpPrefabs);
                    pourHelper.ReadingImgObj.transform.SetParent(this.transform);
                    pourHelper.ReadingImgObj.transform.eulerAngles = Vector3.zero;
                    pourHelper.ReadingImgObj.transform.localPosition = Vector3.zero;
                }
                else
                {
                    pourHelper.ReadingImgObj.SetActive(true);
                }

                ReadingImg = pourHelper.ReadingImgObj.GetComponent<Image>();
                //Debug.Log(distanceInteraction.transform.name + "进入可倒水操作范围");
                //Debug.Log(distanceInteraction.transform.parent.parent.name + "进入可倒水操作范围");
            }

            base.OnDistanceEnter(distanceInteraction);
        }

        /// <summary>
        /// 此时交互的点
        /// </summary>
        /// <param name="distanceInteraction"></param>
        public override void OnDistanceStay(DistanceInteraction distanceInteraction)
        {
            if (!IsCanInteraction(distanceInteraction)) return;

            var interaction = distanceInteraction as InteractionPourWater;

            if (interaction==null)
            {
                Debug.LogError("倒水交互脚本为空!!!");
            }

            ////吸附完毕，
            //if (adsorbOver)
            //{
            //    adsorbOver = false;
            //    OnBeginPourOperate(interaction);
            //}

            if (!isAdsorbed)
            {
                if (IsGrab)
                {
                    //Debug.Log(this.transform.parent.parent.name + "开始吸附读条");

                    //进入倒水操作读条
                    curTime += Time.deltaTime;

                    if (ReadingImg != null)
                    {
                        ReadingImg.fillAmount = Mathf.Clamp((curTime / pourHelper.LimitTime), 0.0f, 1.0f);
                    }
                    else
                    {
                        Debug.Log("ReadingImg 为空");
                    }

                    if (curTime >= pourHelper.LimitTime)
                    {
                        //吸附--不设置为父子物体，改为获取旋转中心点

                        isAdsorbed = true;
                        interaction.isAdsorbed = true;
                        curTime = 0.0f;

                        //pourHelper.RotPt=interaction
                        this.FeaturesObjectController.SetParent(interaction.transform, interaction.pourHelper.localPos, interaction.pourHelper.localRot);

                        interaction.transform.localRotation = Quaternion.identity;
                        //distanceInteraction.FeaturesObjectController.SetParent(this.transform, pourHelper.localPos, pourHelper.localRot);


                        
                        ////吸附结束,发送开始倒水通知
                        //OnBeginPourOperate(interaction);
                        //interaction.OnBeginPourOperate(this);

                        Debug.Log(this.transform.parent.parent.name + "吸附成功");

                        startPour = true;
                        //关闭功能控制端
                        //distanceInteraction.FeaturesObjectController.IsEnable = false;
                        //Debug.Log(distanceInteraction.FeaturesObjectController.name + "抓取功能关闭");

                        //关闭物体跟随光标移动的功能
                        lastMousePos = MOperateManager.GetHandScreenPoint(0);

                        ReadingImg.fillAmount = 0.0f;
                        pourHelper.ReadingImgObj.SetActive(false);

                        FeaturesObjectController.gameObject.AddUpdateObject(OnUpdateObject);


                        Debug.Log(this.FeaturesObjectController.name + "自定义抓取后物体运动状态成功");
                    }
                }
                
            }
            else
            {

                //如果已经吸附，则进入能否倒出水的判断操作

                if (IsGrab)
                {
                    if (pourContainer.ISCanPourOut(this))
                    {
                        //发送开始倒水通知
                        if (startPour)
                        {
                            startPour = false;
                            endPour = false;
                            OnBeginPourOperate(interaction);
                            interaction.OnBeginPourOperate(this);
                            Debug.Log(this.transform.parent.parent.name + "开始往" + distanceInteraction.transform.parent.parent.name + "里倒水");
                        }

                        

                        //接水倒水
                        float pourOutV = pourContainer.PourOut(this);
                        float receiveV = interaction.pourContainer.ReceiveIn(pourContainer);

                        if (pourOutV>0.001f)
                        {
                            //发送倒水中的通知
                            OnPouring(interaction);
                            interaction.OnPouring(this);
                        }

                        if (pourOutV <= 0.001f && !endPour)
                        {
                            //发送结束倒水操作通知
                            endPour = true;
                            OnEndPourOperate(interaction);
                            interaction.OnEndPourOperate(this);
                        }

                        //液面和药品升降
                        CurContainer.ChangeLiquid(-1.0f * pourOutV, 0);
                        //CurContainer.DrugSystemInsdrugSystem.FirstName;
                        interaction.CurContainer.ChangeLiquid(receiveV, 0, CurContainer.DrugSystemIns.FirstName);

                        //if (CurContainer.containerType == EContainerType.烧杯)
                        //{
                        //    (CurContainer as EC_Beaker).ChangeLiquid(-1.0f * pourOutV, 0);
                        //    (interaction.CurContainer as EC_Beaker).ChangeLiquid(receiveV, 0);
                        //}
                        //else if (CurContainer.containerType == EContainerType.量筒)
                        //{
                        //    (CurContainer as EC_M_MeasuringCylinder).ChangeLiquid(-1.0f * pourOutV, 0);
                        //    (interaction.CurContainer as EC_M_MeasuringCylinder).ChangeLiquid(receiveV, 0);
                        //}


                        //else if (CurContainer.containerType == EContainerType.普通瓶子一)
                        //{
                        //    (CurContainer as PlasticBottleOne).ChangeLiquid(-1.0f * pourOutV, 0);
                        //    (interaction.CurContainer as EC_M_MeasuringCylinder).ChangeLiquid(receiveV, 0);
                        //}
                        //else if (CurContainer.containerType == EContainerType.普通瓶子二)
                        //{
                        //    (CurContainer as PlasticBottleTwo).ChangeLiquid(-1.0f * pourOutV, 0);
                        //    (interaction.CurContainer as PlasticBottleTwo).ChangeLiquid(receiveV, 0);
                        //}
                        //else if (CurContainer.containerType == EContainerType.普通瓶子三)
                        //{
                        //    (CurContainer as PlasticBottleThree).ChangeLiquid(-1.0f * pourOutV, 0);
                        //    (interaction.CurContainer as PlasticBottleThree).ChangeLiquid(receiveV, 0);
                        //}

                    }
                }

                


                //if (interaciton.pointSide == PourPointSide.Left)
                //{
                //    this.transform.eulerAngles = new Vector3(0, Mathf.Clamp(Input.mousePosition.y - lastMousePos.y, pourContainer.leftRotateLimits.x, pourContainer.leftRotateLimits.y), 0);
                //}
                //else if (interaciton.pointSide == PourPointSide.Right)
                //{
                //    this.transform.eulerAngles = new Vector3(0, Mathf.Clamp(Input.mousePosition.y - lastMousePos.y, pourContainer.rightRotateLimits.x, pourContainer.rightRotateLimits.y), 0);
                //}

                //this.transform.eulerAngles = new Vector3(0, Input.mousePosition.y - lastMousePos.y, 0);

                lastMousePos = MOperateManager.GetHandScreenPoint(0);
            }

            //抓取状态下
            if (IsGrab)
            {
                //判断光标和倒水点的距离
                if (CaculateScreenPtDistance(interaction.transform.position) > interaction.pourHelper.LeaveDistance)
                {
                    Debug.Log("光标超出范围");

                    ReadingImg.fillAmount = 0.0f;
                    pourHelper.ReadingImgObj.SetActive(false);

                    Debug.Log("移除自定义抓取事件成功");
                    FeaturesObjectController.gameObject.RemoveUpdateObject(OnUpdateObject);
                    curTime = 0.0f;
                    isAdsorbed = false;
                    interaction.isAdsorbed = false;
                    interaction.curTime = 0.0f;
                    FeaturesObjectController.SetParent(null);


                    this.FeaturesObjectController.transform.eulerAngles = Vector3.zero;

                    Debug.Log("this ------");
                    //发送倒水结束事件
                    //OnEndPourOperate(distanceInteraction as InteractionPourWater);

                    //发送离开倒水范围事件
                    OnExitPourDistance(distanceInteraction as InteractionPourWater);
                    

                }
            }

            

            //else if (interaction.isAdsorbed)
            //{
            //    var interaciton = distanceInteraction as InteractionPourWater;
            //    //
            //    //pourContainer
            //    //distanceInteraction.FeaturesObjectController.transform.eulerAngles=


            //    //var curInteraction = distanceInteraction as InteractionPourWater;
            //    //if (curInteraction != null)
            //    //{
            //    //    //curInteraction
            //    //}
            //}

            //进行交互 和动作允许判断 若离开重置判断
            //允许开始 倒水给量 和接水接受

            //if (Vector3.Distance(this.transform.position, distanceInteraction.transform.position) < 0.2f)
            //{
            //    //计时并吸附
            //    if (curTimer >= 2.0f)
            //    {
            //        curTimer = 0.0f;
            //    }

            //    Debug.Log(distanceInteraction.transform.parent.parent.name + "进入了0.2之内，开启倒水旋转");//

            //}

            //基类的距离交互不再生效
            //base.OnDistanceStay(distanceInteraction);
        }
        
        /// <summary>
        /// 切换光标抓取物体后的操作模式为控制旋转
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="quaternion"></param>
        /// <param name="handIndex"></param>
        void OnUpdateObject(GameObject target, Vector3 position, Quaternion quaternion, int handIndex)
        {

            //Debug.Log("旋转中心为：" + target.transform.parent.name);

            float tmpAngle = 0.0f;

            //依据当前操作对象来判断，即当前操作倒水点的左右，来限制旋转角度范围
            //根据左右限制
            if (pointSide == PourPointSide.Left)
            {
                tmpAngle = target.transform.parent.eulerAngles.z + (MOperateManager.GetHandScreenPoint(handIndex).y - lastMousePos.y) * pourHelper.RotSpeed;
                //Debug.Log("旋转角度1----" + tmpAngle);
                tmpAngle = Mathf.Clamp(tmpAngle, pourContainer.leftRotateLimits.x, pourContainer.leftRotateLimits.y);
            }
            else if (pointSide == PourPointSide.Right)
            {
                tmpAngle = target.transform.parent.eulerAngles.z;
                //Debug.Log("初始角度" + tmpAngle);
                if (tmpAngle>0.0f)
                {
                    tmpAngle = (tmpAngle - 360.0f) - (MOperateManager.GetHandScreenPoint(handIndex).y - lastMousePos.y) * pourHelper.RotSpeed;
                }
                else
                {
                    tmpAngle -= (MOperateManager.GetHandScreenPoint(handIndex).y - lastMousePos.y) * pourHelper.RotSpeed;
                }
                //Debug.Log("旋转角度2--***" + tmpAngle);
                tmpAngle = Mathf.Clamp(tmpAngle, pourContainer.rightRotateLimits.x, pourContainer.rightRotateLimits.y);
            }


            //使用其父物体自身旋转来完成倒水旋转
            //Debug.Log("--*******---" + target.transform.parent.name);
            target.transform.parent.eulerAngles = new Vector3(0, 0, tmpAngle);

            //Debug.Log("上一帧" + lastMousePos);
            lastMousePos = MOperateManager.GetHandScreenPoint(handIndex);
            //Debug.Log("当前帧" + lastMousePos);
        }

        /// <summary>
        /// 手握住时离开时 触发一次
        /// </summary>
        /// <param name="distanceInteraction"></param>
        public override void OnDistanceExit(DistanceInteraction distanceInteraction)
        {
            if (!IsCanInteraction(distanceInteraction)) return;
            //进行离开操作 容器解开移动限制


            //Debug.Log("移除自定义抓取事件成功");
            //FeaturesObjectController.gameObject.RemoveUpdateObject(OnUpdateObject);

            if (ReadingImg)
            {
                ReadingImg.fillAmount = 0.0f;
            }
            if (pourHelper.ReadingImgObj)
            {
                pourHelper.ReadingImgObj.SetActive(false);
            }

            if (!isAdsorbed)
            {
                Debug.Log(distanceInteraction.transform.parent.parent.name + "离开");

                base.OnDistanceExit(distanceInteraction);
            }
        }

        /// <summary>
        /// 在距离范围内放手 触发一次
        /// </summary>
        /// <param name="distanceInteraction"></param>
        public override void OnDistanceRelesae(DistanceInteraction distanceInteraction)
        {
            if (!IsCanInteraction(distanceInteraction)) return;
            //进行放手操作 容器解开移动限制

            var interaction = distanceInteraction as InteractionPourWater;

            if (IsGrab)
            {
                if (ReadingImg)
                {
                    ReadingImg.fillAmount = 0.0f;
                }
                if (pourHelper.ReadingImgObj)
                {
                    pourHelper.ReadingImgObj.SetActive(false);
                }
                if (isAdsorbed)
                {
                    FeaturesObjectController.transform.parent.eulerAngles = Vector3.zero;
                    isAdsorbed = false;
                }
                FeaturesObjectController.SetParent(null);
                Debug.Log("移除自定义抓取事件成功");
                FeaturesObjectController.gameObject.RemoveUpdateObject(OnUpdateObject);
                //旋转状态恢复为无旋转
                this.FeaturesObjectController.transform.eulerAngles = Vector3.zero;
                Debug.Log(distanceInteraction.transform.parent.parent.name + "距离范围内直接放手11111");
            }

            //取消吸附
            isAdsorbed = false;
            curTime = 0.0f;

            Debug.Log(distanceInteraction.transform.parent.parent.name + "距离范围内直接放手2222222");

            //发送倒水结束事件
            //if (endPour)
            //{

            //}
            OnEndPourOperate(interaction);

            base.OnDistanceRelesae(distanceInteraction);
        }

        public override void OnDistanceNotInteractionRelease()
        {
            base.OnDistanceNotInteractionRelease();
        }

        #endregion

        #region 倒水距离检测相关
        /// <summary>
        /// 进入倒水距离
        /// </summary>
        /// <param name="interactionPourWater"></param>
        public void OnEnterPourDistance(InteractionPourWater interactionPourWater)
        {
            if (interactionPourWater==null)
            {
                return;
            }
            ////计时读条UI逻辑
            ////startTimer = true;

            //if (true)
            //{

            //}
            ////
            Debug.Log(this.name+"diaoyong进入可倒水范围");
            OnEnterPour.Invoke(interactionPourWater);
        }

        /// <summary>
        /// 开始倒水操作
        /// </summary>
        /// <param name="interactionPourWater"></param>
        public void OnBeginPourOperate(InteractionPourWater interactionPourWater)
        {
            if (interactionPourWater == null)
            {
                return;
            }
            Debug.Log("开始倒水操作");
            OnBeginPour.Invoke(interactionPourWater);
        }

        /// <summary>
        /// 倒水中
        /// </summary>
        /// <param name="interactionPourWater"></param>
        public void OnPouring(InteractionPourWater interactionPourWater)
        {
            if (interactionPourWater == null)
            {
                return;
            }
            Debug.Log("倒水中");
            OnStayPour.Invoke(interactionPourWater);
        }

        /// <summary>
        /// 结束倒水操作
        /// </summary>
        /// <param name="interactionPourWater"></param>
        public void OnEndPourOperate(InteractionPourWater interactionPourWater)
        {
            if (interactionPourWater == null)
            {
                return;
            }
            Debug.Log("结束倒水操作");
            OnEndPour.Invoke(interactionPourWater);
        }

        /// <summary>
        /// 离开倒水范围
        /// </summary>
        /// <param name="interactionPourWater"></param>
        public void OnExitPourDistance(InteractionPourWater interactionPourWater)
        {
            if (interactionPourWater == null)
            {
                return;
            }
            Debug.Log("离开倒水范围");
            OnExitPour.Invoke(interactionPourWater);
        }

        #endregion
    }
}
