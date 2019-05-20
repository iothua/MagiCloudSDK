using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using MagiCloud.KGUI;

namespace MagiCloud.RotateAndZoomTool
{

    public class TestRotateAndZoomTool : MonoBehaviour
    {


        // Use this for initialization
        public KGUI_Button button1;
        public KGUI_Button button2;
        public KGUI_Button button3;
        public KGUI_Button button4;
        public KGUI_Button button5;

        public KGUI_Toggle toggle1;
        public KGUI_Toggle toggle2;
        public KGUI_Toggle toggle3;

        //public bool iszoom = false;
        //public bool isrotate = false;
        //public bool isselfrotate = false;

        string s1 = "相机自身转";
        string s2 = "相机绕点转";
        string s3 = "相机缩放";
        string s4 = "物体自身旋转";
        string s5 = "相机自身转";

        string on = "开启";
        string off = "关闭";

        void Start()
        {
            button1.onClick.AddListener(SelfRotate);
            button2.onClick.AddListener(CenterRotate);
            button3.onClick.AddListener(Zoom);
            button5.onClick.AddListener(LookAtC);
            toggle1.OnValueChanged.AddListener(ReStartSelfRotate);
            toggle2.OnValueChanged.AddListener(ReStartRotate);
            toggle3.OnValueChanged.AddListener(ReStartZoom);
            RotateAndZoomManager.Limit_CameraRotateAroundCenter_HorizontalAxis = new Vector2(-100, 100);

        }

        private void Update()
        {
            //RotateAndZoomManager.IsPauseOrReStart_CameraZoom = iszoom;
            //RotateAndZoomManager.IsPauseOrReStart_CameraRotateAroundCenter = isrotate;
            //RotateAndZoomManager.IsPauseOrReStart_CameraSelfRotate = isselfrotate;
        }

        //

        bool a = true;
        public void SelfRotate(int i)
        {
            if (a)
            {
                RotateAndZoomManager.StartCameraSelfRotate();
                button1.GetComponentInChildren<Text>().text = s1 + on;
                a = false;
            }
            else
            {
                RotateAndZoomManager.StopCameraSelfRotate();
                button1.GetComponentInChildren<Text>().text = s1 + off;
                a = true;
            }
        }


        bool s = true;
        public void CenterRotate(int i)
        {
            if (s)
            {
                RotateAndZoomManager.StartCameraAroundCenter(transform);
                button2.GetComponentInChildren<Text>().text = s2 + on;
                s = false;
            }
            else
            {
                RotateAndZoomManager.StopCameraAroundCenter();
                button2.GetComponentInChildren<Text>().text = s2 + off;
                s = true;
            }
        }

        bool ss = true;
        public void Zoom(int i)
        {
            if (ss)
            {
                RotateAndZoomManager.StartCameraZoom(transform, 5, 50);
                button3.GetComponentInChildren<Text>().text = s3 + on;
                ss = false;
            }
            else
            {
                RotateAndZoomManager.StopCameraZoom();
                button3.GetComponentInChildren<Text>().text = s3 + off;
                ss = true;
            }

        }

        public void LookAtC(int i)
        {
            Camera.main.transform.LookAt(transform.position);
        }

        private void ReStartSelfRotate(bool ison)
        {
            RotateAndZoomManager.IsPauseOrReStart_CameraSelfRotate = ison;
        }

        private void ReStartRotate(bool ison)
        {
            RotateAndZoomManager.IsPauseOrReStart_CameraRotateAroundCenter = ison;
        }

        private void ReStartZoom(bool ison)
        {
            RotateAndZoomManager.IsPauseOrReStart_CameraZoom = ison;
        }


    }

}
