using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Chemistry.Equipments
{
    /// <summary>
    /// 封装放大镜
    /// </summary>
    public class EO_Magnifier //: MonoBehaviour
    {
        /// <summary>
        /// 相机视野参数
        /// </summary>
        float viewValue;

        public float FieldOfView
        {
            get { return viewValue; }
            set { viewValue = value; }
        }

        private Camera camera;

        float LastFrameView = -1f;

        public bool IsChangeFieldOfView { get { return IsChangeView(); } }

        public EO_Magnifier()
        {

        }

        public EO_Magnifier(Camera _camera, float a)
        {
            camera = _camera;
        }

        private bool IsChangeView()
        {
            if (viewValue != LastFrameView)
            {
                camera.fieldOfView = viewValue;
                LastFrameView = viewValue;
                return true;
            }
            LastFrameView = -1f;
            //camera.fieldOfView = aas;
            return false;
        }
    }
}