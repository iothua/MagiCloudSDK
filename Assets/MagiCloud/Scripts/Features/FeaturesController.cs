using System;
using UnityEngine;
using HighlightingSystem;
using MagiCloud.KGUI;

namespace MagiCloud.Features
{
    /*
    思路解析
        1、首先获取到发射线的物体是否为指定物体层，也就是Layer和Ray。所有的物体都应该是这个层
        2、判断照射到的物体是否挂有高亮脚本、旋转脚本（处于抓取状态才可以）、以及限制移动脚本（处于抓取状态才可以）
        3、获取释放状态，如果释放了，则全部回归
    */


    /// <summary>
    /// 功能控制端（一些事件处理）
    /// </summary>
    public class FeaturesController :MonoBehaviour
    {
        public HandOperate LeftHand, RightHand;

        private HighlightingRenderer highlighting;
        private Camera _camera;

        public RotateRayHandle leftRotateRay, rightRotateRay;

        private static FeaturesController _instance;

        /// <summary>
        /// 静态对象
        /// </summary>
        public static FeaturesController Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<FeaturesController>();

                return _instance;
            }
        }

        /// <summary>
        /// 限制物体旋转
        /// </summary>
        public static bool IsLimitObjectRotate = true;

        private void Awake()
        {
            if (_camera == null)
                _camera = Camera.main;

            highlighting = _camera.gameObject.GetComponent<HighlightingRenderer>() ?? _camera.gameObject.AddComponent<HighlightingRenderer>();

            if (highlighting != null)
            {
                highlighting.blurIntensity = 1;
                highlighting.blurSpread = 0;
                highlighting.blurMinSpread = 1;
                highlighting.iterations = 2;
                highlighting.downsampleFactor = 1;
            }

            Application.targetFrameRate = 60;

            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            Destroy(highlighting);



            //KinectEventRay.RemoveListener(OnRay);
            //KinectEventHandGrip.RemoveListener(OnGrip);
            //KinectEventHandIdle.RemoveListener(OnIdle);
        }

        private void OnEnable()
        {

            rightRotateRay = new RotateRayHandle(0);
            rightRotateRay.OnEnable();

            leftRotateRay = new RotateRayHandle(1);
            leftRotateRay.OnEnable();

            LeftHand = new HandOperate(1,leftRotateRay.UIRayDetection);

            RightHand = new HandOperate(0,rightRotateRay.UIRayDetection);

            //KinectEventRayDetection.AddListener(1,leftRotateRay.UIRayDetection);
            //KinectEventRayDetection.AddListener(0,rightRotateRay.UIRayDetection);

            LeftHand.OtherOperate = RightHand;
            RightHand.OtherOperate = LeftHand;

            //LeftHand.OnStart();
            //RightHand.OnStart();

            //KinectEventRay.AddListener(EventLevel.B,OnRay);
            //KinectEventHandGrip.AddListener(EventLevel.B,OnGrip);
            //KinectEventHandIdle.AddListener(EventLevel.B,OnIdle);
        }

        private void FixedUpdate()
        {
            if (leftRotateRay.IsButtonPress || leftRotateRay.IsGrabObject && IsLimitObjectRotate
                || rightRotateRay.IsButtonPress || rightRotateRay.IsGrabObject && IsLimitObjectRotate)
            {
                RotateManager.IsActiveCameraZoom = false;
                RotateManager.IsActiveCameraRotate = false;
                RotateManager.IsActiveCameraAroundCenter = false;
            }
            else
            {
                RotateManager.IsActiveCameraZoom = true;
                RotateManager.IsActiveCameraAroundCenter = true;
                RotateManager.IsActiveCameraRotate = true;
            }
        }

        void OnGrip(int handIndex)
        {
            if (handIndex == 0)
                RightHand.OnGrip(handIndex);
            else
                LeftHand.OnGrip(handIndex);
        }

        void OnIdle(int handIndex)
        {
            if (handIndex == 0)
            {
                RightHand.OnIdle(handIndex);
            }
            else
            {
                LeftHand.OnIdle(handIndex);
            }
        }

        void OnRay(Ray ray,int handIndex)
        {
            if (handIndex == 0)
            {
                RightHand.OnRay(ray,handIndex);

                rightRotateRay.objectRay = ray;
            }
            else
            {
                LeftHand.OnRay(ray,handIndex);
                leftRotateRay.uiRay = ray;
            }
        }

        private void OnDisable()
        {
            //LeftHand.OnStop();
            //RightHand.OnStop();

            //KinectEventRay.RemoveListener(OnRay);
            //KinectEventHandGrip.RemoveListener(OnGrip);
            //KinectEventHandIdle.RemoveListener(OnIdle);

            //KinectEventRayDetection.RemoveListener(1,leftRotateRay.UIRayDetection);
            //KinectEventRayDetection.RemoveListener(0,rightRotateRay.UIRayDetection);

            rightRotateRay.OnDistable();
            leftRotateRay.OnDistable();

            rightRotateRay = null;
            leftRotateRay = null;

            LeftHand = null;
            RightHand = null;
        }

        /// <summary>
        /// 设置物体被抓取
        /// </summary>
        /// <param name="handIndex"></param>
        /// <param name="target"></param>
        public void SetGrabObject(int handIndex,GameObject target)
        {
            if (handIndex == 0)
            {
                RightHand.SetGrabObject(target);
            }
            else
            {
                LeftHand.SetGrabObject(target);
            }
        }
    }

    public enum HandOperaType
    {
        None,
        Idle,
        Grip,
        Grab
    }

    public class HandOperate
    {
        private int handIndex = -1; //手势序号
        public HandOperaType operateStatus = HandOperaType.Idle; //手势状态

        private HighlightObject highlightObject;

        private OperaObject operaObject;

        public HandOperate OtherOperate { get; set; }

        private GameObject operaTargetObject;//操作的物体

        public Func<bool> RayExternaLimit; //射线外部限制

        private bool isGrip = false;

        public HandOperate(int handIndex,Func<bool> func)
        {
            this.handIndex = handIndex;
            RayExternaLimit = func;
        }

        //public void OnStart()
        //{
        //    KinectEventRay.AddListener(EventLevel.B, OnRay);
        //    KinectEventHandGrip.AddListener(EventLevel.B, OnGrip);
        //    KinectEventHandIdle.AddListener(EventLevel.B, OnIdle);
        //}

        public void OnGrip(int handIndex)
        {
            if (this.handIndex != handIndex) return;

            if (isGrip) return;

            isGrip = true;

            //HideHighLight();

            operateStatus = HandOperaType.Grip;

            //Events.EventHandGrip.SendListener(handIndex);
        }

        public void OnRay(Ray ray,int handIndex)
        {
            if (this.handIndex != handIndex) return;

            RaycastHit hit;

            //如果照射到物体，就取消掉旋转和缩放
            //Events.EventRay.SendListener(ray,handIndex);

            ////外部限制，如果限制成功，则不进行任何处理
            //if (RayExternaLimit())
            //{
            //    //判断是否激活高亮
            //    HideHighLight();
            //    HideLabel();
            //    if (operateStatus == HandOperaType.Idle)
            //        operaObject = null;
            //}
            //else if (Physics.Raycast(ray,out hit,10000,1 << KinectConfig.layerRay | 1 << KinectConfig.layerObject))
            //{
            //    Events.EventRayTarget.SendListener(hit,handIndex);
            //    OnRayTarget(hit,handIndex);
            //}
            //else
            //{
            //    //判断是否激活高亮
            //    HideHighLight();
            //    HideLabel();
            //    if (operateStatus == HandOperaType.Idle)
            //        operaObject = null;
            //}
        }

        /// <summary>
        /// 显示标签
        /// </summary>
        private void ShowLabel()
        {
            if (operaObject!=null &&operaObject.FeaturesObject.ActiveLabel)
                operaObject.FeaturesObject.AddLabel().label.OnEnter();
        }

        /// <summary>
        /// 隐藏标签
        /// </summary>
        private void HideLabel()
        {
            if (operaObject!=null &&operaObject.FeaturesObject.ActiveLabel)
                operaObject.FeaturesObject.AddLabel().label.OnExit();
        }

        /// <summary>
        /// 隐藏高亮
        /// </summary>
        private void HideHighLight()
        {
            if (operaObject != null && operaObject.FeaturesObject.ActiveHighlight && !isGrip)
            {
                operaObject.GetComponent<HighlightObject>().HideHighLight();
            }
        }

        /// <summary>
        /// 显示高亮
        /// </summary>
        private void ShowHightLight(bool isGrab)
        {
            //判断是否激活高亮
            if (operaObject != null && operaObject.FeaturesObject.ActiveHighlight)
            {
                operaObject.GetComponent<HighlightObject>().ShowHighLight(isGrab);
            }
        }

        public void OnIdle(int handIndex)
        {
            if (this.handIndex != handIndex) return;

            if (!isGrip) return;

            isGrip = false;

            GameObject grabObject = null;

            if (operaObject != null)
            {
                grabObject = HandleOperate(operaObject.FeaturesObject.operaType,false);
            }

            HideHighLight();

            //Events.EventHandIdle.SendListener(handIndex);

            //if (grabObject != null)
            //{
            //    Events.EventHandReleaseObject.SendListener(grabObject,handIndex);
            //    Events.EventHandReleaseObjectKey.SendListener(grabObject,handIndex);
            //}

            operateStatus = HandOperaType.Idle;

            operaObject = null;
        }

        //public void OnStop()
        //{
        //    KinectEventRay.RemoveListener(OnRay);
        //    KinectEventHandGrip.RemoveListener(OnGrip);
        //    KinectEventHandIdle.RemoveListener(OnIdle);
        //}

        private void OnRayTarget(RaycastHit hit,int handIndex)
        {
            if (this.handIndex != handIndex) return;

            switch (operateStatus)
            {
                case HandOperaType.Idle:

                    if (hit.collider == null)
                    {
                        //判断是否激活高亮
                        HideHighLight();
                        HideLabel();
                        operaObject = null;
                        return;
                    }

                    //如果已经是重复的，在进行后续的处理，已经没有意义了
                    if (operaObject != null && operaObject.gameObject == hit.collider.gameObject) return;

                    if (operaObject != null && operaObject == OtherOperate.operaObject) return;//如果照射的物体射线相等

                    if (operaObject != null)
                    {
                        //处理高亮
                        HideLabel();
                        HideHighLight();
                        operaObject = null;
                    }

                    operaObject = hit.collider.gameObject.GetComponent<OperaObject>();

                    if (operaObject == null) return;
                    ShowLabel();
                    ShowHightLight(false);
                    break;
                case HandOperaType.Grip:

                    //握拳，如果物体跟初始的物体一样，则进入Grab，否则不进入
                    if (operaObject != null && operaObject.gameObject != hit.collider.gameObject)
                        return;
                    HideLabel();
                    operateStatus = HandOperaType.Grab;

                    break;
                case HandOperaType.Grab:
                    //如果抓取时，判断这个物体是否具有旋转脚本或者限制脚本

                    if (operaObject == null) return;

                    ShowHightLight(true);
                    HideLabel();
                    GameObject grabObject = HandleOperate(operaObject.FeaturesObject.operaType,true);

                    operateStatus = HandOperaType.None;

                    if (grabObject != null)
                    {
                        //Events.EventHandGrabObject.SendListener(grabObject,handIndex);
                        //Events.EventHandGrabObjectKey.SendListener(grabObject,handIndex);
                    }

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isGrip"></param>
        /// <returns></returns>
        private GameObject HandleOperate(ObjectOperaType type,bool isGrip)
        {
            GameObject grabObject = null;

            switch (type)
            {
                case ObjectOperaType.无:
                    grabObject = null;
                    break;
                case ObjectOperaType.能抓取:

                    //var canGrab = operaObject.GetComponent<MCCanGrab>();
                    //grabObject = canGrab.GrabObject;

                    break;
                case ObjectOperaType.物体自身旋转:
                case ObjectOperaType.摄像机围绕物体旋转:
                    var rotation = operaObject.GetComponent<MCObjectRatation>();

                    if (isGrip)
                        rotation.OnOpen();
                    else
                        rotation.OnClose();

                    grabObject = rotation.GrabObject;
                    break;
                case ObjectOperaType.自定义:

                    var customize = operaObject.GetComponent<MCustomize>();
                    grabObject = customize.GrabObject;

                    if (isGrip)
                    {
                        customize.OnOpen(handIndex);
                    }
                    else
                    {
                        customize.OnClose();
                    }

                    break;
            }

            return grabObject;
        }

        /// <summary>
        /// 设置物体被抓取
        /// </summary>
        /// <param name="target"></param>
        public void SetGrabObject(GameObject target)
        {
            var features = target.GetComponent<FeaturesObjectController>();

            if (features == null)
            {
                //KinectTransfer.SetObjectGrab(target,handIndex);
            }
            else
            {
                operaObject = features.Opera;
                GameObject grabObject = HandleOperate(operaObject.FeaturesObject.operaType,true);

                operateStatus = HandOperaType.None;

                if (grabObject != null)
                {
                    //Events.EventHandGrabObject.SendListener(grabObject,handIndex);
                    //Events.EventHandGrabObjectKey.SendListener(grabObject,handIndex);
                }
            }
        }
    }
}
