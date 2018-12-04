using UnityEngine;
using DG.Tweening;
using MagiCloud.Core.Events;
using MagiCloud.Core;

namespace MagiCloud.RotateAndZoomTool
{

    public class CameraRotate
    {
        private static CameraRotate instance = null;

        Vector3 inputPointPos = Vector3.zero;

        private CameraRotate()
        {

        }

        public static CameraRotate Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CameraRotate();
                }
                return instance;
            }
        }

        /// <summary>
        /// 传参数的相机绕点转暂停和开启方式
        /// </summary>
        public bool IsRotateCameraWithCenterEnable
        {
            get
            {
                return isRotateCameraWithCenter;
            }

            set
            {
                PauseOrRestart(value);
                isRotateCameraWithCenter = value;
            }
        }

        /// <summary>
        /// 重启或者暂停 相机按照自身转
        /// </summary>
        public bool IsSelfRotateCameraEnable
        {
            get
            {
                return isSelfRotateCameraEnable;
            }

            set
            {
                PauseOrRestart_CameraRotateSelf(value);
                isSelfRotateCameraEnable = value;
            }
        }



        /// <summary>
        /// 主相机
        /// </summary>
        Camera mainCamera;

        /// <summary>
        /// 相机从其他状态恢复到绕点转过渡的速度
        /// </summary>
        float cameraResetspeed = 0.5f;

        /// <summary>
        /// 相机绕自身轴旋转处理对象
        /// </summary>
        public RotateCore rotateCore;

        //相机绕点转的旋转对象
        public DragMouseOrbit dragMouseOrbit;

        #region 相机旋转参数

        /// <summary>
        /// 是否相机绕点转时候的初始化
        /// </summary>
        bool isRotateCameraWithCenterInitialization = false;
        /// <summary>
        /// 暂停和重启bool
        /// </summary>
        bool isRotateCameraWithCenter = false;

        /// <summary>
        /// 相机对的中心位置物体
        /// </summary>
        GameObject cameraLookToCenter;

        /// <summary>
        /// 相机绕点转的速率
        /// </summary>
        float rotateCameraWithGoSpeed = 0.5f;

        /// <summary>
        /// 抖动处理
        /// </summary>
        float cameraShake = 0.01f;
        /// <summary>
        /// 相机在绕点旋转的角度限制 //弃用
        /// </summary>
        //Vector2 cameraCenterRotateXlimits = new Vector2(-80, 80);
        //Vector2 cameraCenterRotateYlimits = new Vector2(-40, 40);

        //Vector2 cameraCenterRotateFirstEulerAngleX = Vector2.zero;

        /// <summary>
        /// 相机绕点转的处理对象 //阻尼旋转的处理对象
        /// </summary>
        //OtherRotateHelp otherHelpCameraRotateWithCenter;


        /// <summary>
        /// 是否相机绕自身转时候的初始化
        /// </summary>
        bool isCameraRotateSelfInitialization = false;

        /// <summary>
        /// 相机绕自身轴旋转重启与
        /// </summary>
        bool isSelfRotateCameraEnable = false;

        /// <summary>
        /// 相机在绕自身旋转旋转速率
        /// </summary>
        float rotateCameraSelfSpeed = 0.5f;

        /// <summary>
        /// 相机在绕自身旋转的角度限制
        /// </summary>
        Vector2 cameraSelfRotateXlimits = new Vector2(-10, 10);
        Vector2 cameraSelfRotateYlimits = new Vector2(-10, 10);

        /// <summary>
        /// 相机按自身轴旋转对象 插件
        /// </summary>
        OtherRotateHelp otherHelpCameraSelfRotate;

        /// <summary>
        /// 相机开启绕点转的时候的tween动画对象
        /// </summary>
        Tween tween;

        /// <summary>
        /// update执行的代码
        /// </summary>
        MBehaviour mBehaviour;

        #endregion


        #region 旋转相机绕点旋转的方法
        /// <summary>
        /// 开启主相机绕指定点旋转 注意:不要设置在有自转的Transform下
        /// </summary>
        /// <param name="cameraRotateCenter">相机对准物体</param>
        /// <param name="speed"></param>
        public void StartCameraRotateWithCenter(Transform cameraCenter, float duration = 0.5f)
        {
            StopCameraRotateWithCenter();

            if (mainCamera == null) mainCamera = MUtility.MainCamera;

            cameraLookToCenter = cameraCenter.gameObject;

            tween =

            //朝向开启再旋转
            mainCamera.transform.DOLookAt(cameraLookToCenter.transform.position, duration).OnComplete(() =>
            {
                dragMouseOrbit = new DragMouseOrbit(cameraCenter, mainCamera.transform);

                EventCameraRotate.AddListener(RotateCameraToCenter);

                mBehaviour = new MBehaviour(ExecutionPriority.High);

                MBehaviourController.AddBehaviour(mBehaviour);

                mBehaviour.OnUpdate_MBehaviour(() =>
                {
                    RotateCameraToCenter(inputPointPos);
                });


                isRotateCameraWithCenter = true;

                isRotateCameraWithCenterInitialization = true;

                RotateAndZoomManager.IsDone_StartCameraAroundCenter_Initialization = true;
            });

        }



        public void StartCameraRotateWithCenter(Transform cameraCenter, Vector3 camerainitialpos, Quaternion camerainitialqua, float duration = 0.5f)
        {
            StopCameraRotateWithCenter();

            if (mainCamera == null) mainCamera = MUtility.MainCamera;

            cameraLookToCenter = cameraCenter.gameObject;

            //开启初始化相机位置和角度

            tween =

            mainCamera.transform.DOMove(camerainitialpos, duration / 3);

            mainCamera.transform.DORotate(camerainitialqua.eulerAngles, duration / 3).OnComplete(() =>
              {
                  mainCamera.transform.DOLookAt(cameraCenter.transform.position, duration / 3).OnComplete(() =>
                    {
                        dragMouseOrbit = new DragMouseOrbit(cameraCenter, mainCamera.transform);

                        EventCameraRotate.AddListener(RotateCameraToCenter);

                        mBehaviour = new MBehaviour(ExecutionPriority.High);

                        MBehaviourController.AddBehaviour(mBehaviour);

                        mBehaviour.OnUpdate_MBehaviour(() =>
                        {
                            RotateCameraToCenter(inputPointPos);
                        });

                        isRotateCameraWithCenter = true;

                        isRotateCameraWithCenterInitialization = true;

                        RotateAndZoomManager.IsDone_StartCameraAroundCenter_Initialization = true;
                    });

              });

        }


        /// <summary>
        /// 相机绕点转的暂停和重启方法
        /// </summary>
        /// <param name="isenable"></param>
        public void PauseOrRestart(bool isenable)
        {
            if (!isRotateCameraWithCenterInitialization) return;
            if (isenable)
            {
                EventCameraRotate.AddListener(RotateCameraToCenter);
                mBehaviour.IsEnable = true;

            }
            else
            {
                EventCameraRotate.RemoveListener(RotateCameraToCenter);
                mBehaviour.IsEnable = false;
            }
        }

        /// <summary>
        /// 停止主相机 绕点旋转 相机会 设置回到 MCKinectObject下
        /// </summary>
        public void StopCameraRotateWithCenter()
        {
            tween.Kill();

            if (mBehaviour != null) mBehaviour.OnExcuteDestroy();

            if (dragMouseOrbit != null) dragMouseOrbit.ClearData();

            EventCameraRotate.RemoveListener(RotateCameraToCenter);

            isRotateCameraWithCenter = false;

            isRotateCameraWithCenterInitialization = false;

            RotateAndZoomManager.IsDone_StartCameraAroundCenter_Initialization = false;
        }

        #endregion


        #region 相机绕点旋转处理
        /// <summary>
        /// 绕点旋转相机
        /// </summary>
        /// <param name="handindex">手编号</param>
        /// <param name="vector3">手帧差向量</param>
        void RotateCameraToCenter(Vector3 vector3)
        {
            if (!isRotateCameraWithCenter) return;
            inputPointPos = vector3;
            InUpdateCameraRotate(mainCamera.transform, cameraLookToCenter, -vector3);
        }

        /// <summary>
        /// 相机绕点旋转处理
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="target"></param>
        /// <param name="pos"></param>
        void InUpdateCameraRotate(Transform maincamera, GameObject target, Vector3 pos)
        {
            if (maincamera == null) return;

            dragMouseOrbit.yMinLimit = RotateAndZoomManager.Limit_CameraRotateAroundCenter_VerticalAxis.x;
            dragMouseOrbit.yMaxLimit = RotateAndZoomManager.Limit_CameraRotateAroundCenter_VerticalAxis.y;
            dragMouseOrbit.xMinLimit = RotateAndZoomManager.Limit_CameraRotateAroundCenter_HorizontalAxis.x;
            dragMouseOrbit.xMaxLimit = RotateAndZoomManager.Limit_CameraRotateAroundCenter_HorizontalAxis.y;

            CameraZoom.Instance.Rotatedistance = DistanceCameraToTarget(target.transform, maincamera);

            dragMouseOrbit.LateUpdateCameraRotate(maincamera, pos, CameraZoom.Instance.Rotatedistance);
        }


        #endregion


        #region 开启相机绕自身轴旋转的方法
        /// <summary>
        /// 开启主相机 绕自身轴转
        /// </summary>
        public void StartCameraSelfRotate()
        {
            rotateCore = new RotateCore();

            if (mainCamera == null) mainCamera = MUtility.MainCamera;

            if (otherHelpCameraSelfRotate == null) otherHelpCameraSelfRotate = new OtherRotateHelp(cameraSelfRotateXlimits, cameraSelfRotateYlimits, rotateCameraSelfSpeed);

            EventCameraRotate.AddListener(RotateCameraSelf);

            isSelfRotateCameraEnable = true;

            isCameraRotateSelfInitialization = true;
        }

        /// <summary>
        /// 关闭主相机 绕自身轴转
        /// </summary>
        public void StopCameraSelfRotate()
        {
            EventCameraRotate.RemoveListener(RotateCameraSelf);

            isSelfRotateCameraEnable = false;

            isCameraRotateSelfInitialization = false;
        }

        /// <summary>
        /// 相机绕点转的暂停和重启方法
        /// </summary>
        /// <param name="isenable"></param>
        public void PauseOrRestart_CameraRotateSelf(bool isenable)
        {
            if (!isCameraRotateSelfInitialization) return;
            if (isenable)
            {
                EventCameraRotate.AddListener(RotateCameraSelf);

            }
            else
            {
                EventCameraRotate.RemoveListener(RotateCameraSelf);
            }
        }


        #endregion


        #region 相机绕自身轴旋转处理
        /// <summary>
        /// 绕自己轴旋转
        /// </summary>
        /// <param name="handindex">手编号</param>
        /// <param name="vector3">手帧差向量</param>
        void RotateCameraSelf(Vector3 vector3)
        {
            InUpdateCameraRotate(mainCamera.transform, vector3);
        }

        /// <summary>
        /// 相机绕自身轴转的处理
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="pos"></param>
        void InUpdateCameraRotate(Transform camera, Vector3 pos)
        {
            //限制角度
            otherHelpCameraSelfRotate.MinmaxX = RotateAndZoomManager.Limit_CameraRotateSelf_HorizontalAxis;
            otherHelpCameraSelfRotate.MinmaxY = RotateAndZoomManager.Limit_CameraRotateSelf_VerticalAxis;

            //插件实现方式
            //otherHelpCameraSelfRotate.RotateFun(camera, pos);

            rotateCore.RotateFunInWorldSpace(camera, mainCamera, 0.1f, pos);
            rotateCore.AngleLimits(camera, AxisLimits.X, otherHelpCameraSelfRotate.MinmaxY);
            rotateCore.AngleLimits(camera, AxisLimits.Y, otherHelpCameraSelfRotate.MinmaxX);
            rotateCore.AngleLimits(camera, AxisLimits.Z, Vector2.zero);
        }

        #endregion



        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        float DistanceCameraToTarget(Transform camera, Transform target)
        {
            return Vector3.Distance(camera.position, target.position);
        }
    }
}
