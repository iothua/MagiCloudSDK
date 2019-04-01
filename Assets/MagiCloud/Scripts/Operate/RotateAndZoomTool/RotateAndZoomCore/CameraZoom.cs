using DG.Tweening;
using MagiCloud.Core.Events;
using UnityEngine;

namespace MagiCloud.RotateAndZoomTool
{

    public class CameraZoom
    {
        private static CameraZoom instance = null;

        private CameraZoom()
        {

        }

        public static CameraZoom Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CameraZoom();
                }
                return instance;
            }
        }

        /// <summary>
        /// 缩放暂停和重启控制
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return isEnable;
            }

            set
            {
                PauseOrRestart(value);
                isEnable = value;
            }
        }

        public float Rotatedistance
        {
            get
            {
                return rotatedistance;
            }

            set
            {
                rotatedistance = value;
            }
        }

        /// <summary>
        /// 供外界读取 数据
        /// </summary>
        float rotatedistance = 5;

        /// <summary>
        /// 初始与物体距离
        /// </summary>
        float distance = 5;

        /// <summary>
        /// 相机缩放功能是否初始化
        /// </summary>
        bool isZoomInitialization = false;

        /// <summary>
        /// 相机缩放功能是暂停或者重启
        /// </summary>
        private bool isEnable = false;

        /// <summary>
        /// 缩放 相机对准的物体
        /// </summary>
        Transform zoomTarget;

        /// <summary>
        /// 主相机
        /// </summary>
        Camera mainCamera;

        /// <summary>
        /// 缩放的最远最近
        /// </summary>
        float zoomMinDis = 0;
        float zoomMaxDis = 0;

        private Vector3 velocity = Vector3.zero;
        /// <summary>
        ///  手势缩放系数
        /// </summary>
        float kinectHandZoomSpeed = 0.1f;

        /// <summary>
        /// 相机缩放平滑系数
        /// </summary>
        float mouseZoomSpeed = 100f;

        /// <summary>
        /// 相机缩放移动系数
        /// </summary>
        float mouseMoveSpeed = 10f;

        /// <summary>
        /// 双手缩放时候  双手距离阀值
        /// </summary>
        float mouseZoomThresholdValue = 10;

        ///// <summary>
        ///// 缩放快慢数值处理
        ///// </summary>
        //public OtherZoomHelp otherZoomHelp;
        /// <summary>
        /// 每变化一次缩放程度变化区间
        /// </summary>
        Vector2 zoomSpeedInterval = new Vector2(2f,5f);

        #region 开启和关闭缩放
        /// <summary>
        /// 开启相机的无限缩放
        /// </summary>
        void StartCameraZoom()
        {
            mainCamera = MUtility.MainCamera;

            //EventCameraZoom.AddListener(HandZoomFun);
            EventCameraZoom.AddListener(MouseZoomFun);
        }

        /// <summary>
        /// 关闭相机的无限缩放
        /// </summary>
        public void StopCameraZoom()
        {
            //EventCameraZoom.RemoveListener(HandZoomFun);
            EventCameraZoom.RemoveListener(MouseZoomFun);

            isEnable = false;

            isZoomInitialization = false;
        }

        /// <summary>
        /// 开启相机对中心点缩放
        /// </summary>
        /// <param name="zoomcenter">缩放中心</param>
        /// <param name="mindistance">最近距离</param>
        /// <param name="maxdistance">最大距离</param>
        public void StartCameraZoomWithCenter(Transform zoomcenter,float mindistance,float maxdistance)
        {
            mainCamera = MUtility.MainCamera; ;
            zoomTarget = zoomcenter;

            zoomMinDis = mindistance;
            zoomMaxDis = maxdistance;

            //EventCameraZoom.AddTwoHandDisEvent(HandZoomFun);
            EventCameraZoom.AddListener(MouseZoomFun);

            isEnable = true;

            isZoomInitialization = true;
        }

        /// <summary>
        /// 暂停或者重启动
        /// </summary>
        /// <param name="isenable"></param>
        public void PauseOrRestart(bool isenable)
        {
            if (!isZoomInitialization) return;
            if (isenable)
            {
                //EventCameraZoom.AddTwoHandDisEvent(HandZoomFun);
                EventCameraZoom.AddListener(MouseZoomFun);
            }
            else
            {
                //EventCameraZoom.RemoveTwoHandDisEvent(HandZoomFun);
                EventCameraZoom.RemoveListener(MouseZoomFun);
            }
        }


        /// <summary>
        /// 关闭缩放
        /// </summary>
        public void StopCameraZoomWithCenter()
        {
            StopCameraZoom();
        }

        #endregion     

        #region 缩放方法
        /// <summary>
        /// 双手缩放
        /// </summary>
        /// <param name="dis"></param>
        /// <param name="speedwithtimegrow"></param>
        void HandZoomFun(float zoomdis)
        {
            //距离阀值控制
            if (Mathf.Abs(zoomdis) < mouseZoomThresholdValue) return;

            //缩放双手距离 弱化处理
            if (zoomdis < 0) zoomdis = -0.01f;
            else if (zoomdis > 0) zoomdis = 0.01f;
            else zoomdis = 0;

            ZoomAndLimit(zoomdis);
        }
        #endregion

        #region 鼠标控制缩放
        /// <summary>
        /// 鼠标缩放
        /// </summary>
        /// <param name="zoomdis"></param>
        /// <param name="speedwithtimegrow"></param>
        void MouseZoomFun(float zoomdis)
        {
            ZoomAndLimit(zoomdis);
        }

        /// <summary>
        /// 缩放核心方法
        /// </summary>
        /// <param name="zoomdis">缩放控制数变量</param>
        /// <param name="speedwithtimegrow">速率变化(亦无用)</param>
        void ZoomAndLimit(float zoomdis)
        {

            distance = DistanceCameraToTarget(zoomTarget,mainCamera.transform);

            mouseZoomSpeed = RotateAndZoomManager.Speed_CameraZoom;
         
            distance = Mathf.Clamp(distance - zoomdis * mouseZoomSpeed,zoomMinDis,zoomMaxDis);
            Vector3 curCameraPosition = mainCamera.transform.position;
            RaycastHit hit;
            if (Physics.Linecast(zoomTarget.position,curCameraPosition,out hit,1 << (LayerMask.NameToLayer("None"))))
            {
                distance -= hit.distance * mouseMoveSpeed;
            }
            Rotatedistance = distance;

            Vector3 negDistance = new Vector3(0.0f,0.0f,-distance);
            Vector3 tempposition = mainCamera.transform.rotation * negDistance + zoomTarget.position;
            //阻尼计算
            Vector3.SmoothDamp(curCameraPosition,tempposition,ref velocity,0.5f,2);
            mainCamera.transform.position = tempposition;
        }


        #endregion

        #region 相机移动调整 和 物体间距离计算

        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        float DistanceCameraToTarget(Transform camera,Transform target)
        {
            return Vector3.Distance(camera.position,target.position);
        }

        #endregion

    }

}
