using MagiCloud.Features;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace Chemistry.Equipments
{
    public class EA_Magnify : MonoBehaviour
    {
        [SerializeField]
        private Camera cameraMagnify;

        [SerializeField]
        private GameObject show;

        /// <summary>
        /// 记录移动前位置
        /// </summary>
        private Vector3 oldPos;

        /// <summary>
        /// 记录父物体
        /// </summary>
        private Transform parentTra;


        RenderTexture renderTexture;

        EO_Magnifier magnifier;

        [Header("调整视野"), Range(1f, 179f)]
        public float value_View = 1f;


        [Header("调整正交视野"), Range(0f, 3f)]
        public float value_Size = 1f;

        //[SerializeField, Header("true为启用优化，false为禁用优化")]
        //private bool IsUse = true;

        [SerializeField, Header("true为优化1，false为优化2")]
        private bool ChooseEffect = true;




        [SerializeField, Header("俯视相机纵深"), Range(0f, 20f)]
        public float Depth = 0;

        /// <summary>
        /// 设置相机方向true为向前，false为向下
        /// </summary>
        [SerializeField,Header("设置相机方向true为向前，false为向下")]
        bool IsCameraDirection=true;
       
        /// <summary>
        /// 限制执行次数
        /// </summary>
        bool IsLimit;//限制执行次数


        void OnChangeDirection()
        {
            if (IsCameraDirection)
            {
                if (IsLimit)
                {
                    cameraMagnify.transform.localPosition = new Vector3(0, -1f, 0);//初始本地位置
                    cameraMagnify.transform.localEulerAngles = Vector3.zero;//初始本地角度
                    IsLimit = false;
                }
            }
            else
            {
                if (!IsLimit)
                {
                    cameraMagnify.transform.localPosition = new Vector3(0, 0, Depth);//初始本地位置
                    cameraMagnify.transform.localEulerAngles = new Vector3(90f,90f, 90f);//初始本地角度
                    IsLimit = true;
                }
            }
        }


        // Use this for initialization
        void Start()
        {
            //renderTexture = Resources.Load<RenderTexture>("MagnifierUsedFreedom");
            
            renderTexture = Instantiate(Resources.Load<RenderTexture>("MagnifierUsedFreedom"));
            
            if (renderTexture == null) return;

            cameraMagnify.targetTexture = renderTexture;
            if (show != null)
                if (show.GetComponent<RawImage>() != null)
                    show.GetComponent<RawImage>().texture = renderTexture;

            magnifier = new EO_Magnifier(cameraMagnify, 0f);

            oldPos = cameraMagnify.transform.localPosition;
            parentTra = cameraMagnify.transform.parent;
        }

        // Update is called once per frame
        void Update()
        {
            //if (magnifier != null)
            //    if (magnifier.IsChangeFieldOfView)
            //    {
            //        magnifier.FieldOfView = value_View;
            //    }

            #region 调整放大镜摄像机方向位置
            OnChangeDirection();

            if (!IsCameraDirection)//俯视相机才可调整纵深
                cameraMagnify.transform.localPosition = new Vector3(0, 0, Depth);//初始本地位置
            #endregion

            #region 放大镜摄像机位置优化

            if (IsCameraDirection)
            {
                if (ChooseEffect)
                    OnRayDetection();
                else
                    ComplexComputation();
            }
            #endregion

            if (magnifier != null)
                if (magnifier.IsChangeFieldOfSize)
                {
                    magnifier.FieldOfSize = value_Size;
                }
        }

        private void OnRayDetection()
        {
            //Ray ray = //Camera.main.ScreenPointToRay(Input.mousePosition);
            //Ray ray = new Ray(transform.position, Vector3.forward);


            //相机到放大镜的向量
            var c_m_tan = Mathf.Abs(Camera.main.transform.position.x - transform.position.x) / Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

            Ray ray = new Ray(transform.position, transform.position - Camera.main.transform.position);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                //画线
                Debug.DrawLine(ray.origin, hitInfo.point, Color.red);

                GameObject gameObj = hitInfo.collider.gameObject;
                
                if (gameObj.GetComponent<OperaObject>() != null)
                {
                    cameraMagnify.transform.position = new Vector3(gameObj.GetComponent<OperaObject>().FeaturesObject.transform.position.x, cameraMagnify.transform.position.y, cameraMagnify.transform.position.z);
                }
                else
                {
                    cameraMagnify.transform.localPosition = oldPos;

                    //if (transform.position.x >= Camera.main.transform.position.x)
                    //{
                    //    cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                    //}
                    //else
                    //{
                    //    cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, -Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                    //}
                }
            }
            else
            {
                cameraMagnify.transform.localPosition = oldPos;

                //if (transform.position.x >= Camera.main.transform.position.x)
                //{
                //    cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                //}
                //else
                //{
                //    cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, -Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                //}
            }

        }




        /// <summary>
        /// 
        /// </summary>
        Transform Rayobject;

        /// <summary>
        /// 复杂计算
        /// </summary>
        void ComplexComputation()
        {
            //相机到物体（放大镜）的向量
            Vector3 direction = transform.position - Camera.main.transform.position;

            Ray ray = new Ray(transform.position, direction);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                //画线
                Debug.DrawLine(ray.origin, hitInfo.point, Color.red);

                GameObject gObj = hitInfo.collider.gameObject;

                Debug.Log("照射到   " + gObj.name);

                if (gObj.GetComponent<OperaObject>() != null)
                {
                    Rayobject = gObj.transform;
                    //cameraMagnify.transform.position = new Vector3(gObj.GetComponent<OperaObject>().FeaturesObject.transform.position.x, cameraMagnify.transform.position.y, cameraMagnify.transform.position.z);
                    //Test();
                }
                else
                {
                    //cameraMagnify.transform.localPosition = oldPos;
                    //Rayobject = null;
                    //Test();
                }
            }
            else
            {
                //cameraMagnify.transform.localPosition = oldPos;
                //Rayobject = null;
                //Test();
            }



            //if (Rayobject != null)
            //{
            //    //相机到检测物体的向量
            //    var c_o_tan = Mathf.Abs(Camera.main.transform.position.x - Rayobject.transform.position.x) / Mathf.Abs(Camera.main.transform.position.z - Rayobject.transform.position.z);
            //    //放大镜到检查物体的向量
            //    var m_o_tan= Mathf.Abs(transform.position.x - Rayobject.transform.position.x) / Mathf.Abs(transform.position.z - Rayobject.transform.position.z);

            //    var c_m_tan= Mathf.Abs(Camera.main.transform.position.x - transform.position.x) / Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

            //    //cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);


            //    //if (Mathf.Atan(m_tan) * 180f / Mathf.PI > Mathf.Atan(tan) * 180f / Mathf.PI)
            //    //{
            //    //    cameraMagnify.transform.localEulerAngles = Vector3.zero;
            //    //}
            //    //else
            //    //{
            //    //    if (transform.position.x >= Camera.main.transform.position.x)
            //    //    {

            //    //        if (transform.position.x <= Rayobject.transform.position.x)
            //    //        {
            //    //            cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
            //    //        }
            //    //        else
            //    //        {
            //    //            //cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, -Mathf.Atan(m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
            //    //            cameraMagnify.transform.localEulerAngles = Vector3.zero;
            //    //        }
            //    //    }
            //    //    else
            //    //    {
            //    //        if (transform.position.x <= Rayobject.transform.position.x)
            //    //        {
            //    //            //cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
            //    //            cameraMagnify.transform.localEulerAngles = Vector3.zero;
            //    //        }
            //    //        else
            //    //        {
            //    //            cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, -Mathf.Atan(m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);

            //    //        }
            //    //    }
            //    //}


            //    if (transform.position.x >= Camera.main.transform.position.x)
            //    {
            //        if (transform.position.x <= Rayobject.transform.position.x)
            //        {
            //            if (Mathf.Atan(m_o_tan) * 180f / Mathf.PI > Mathf.Atan(c_o_tan) * 180f / Mathf.PI)
            //            {
            //                //cameraMagnify.transform.localEulerAngles = Vector3.zero;
            //                cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
            //            }
            //            else
            //            {
            //                cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(m_o_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
            //            }
            //        }
            //        else
            //        {
            //            cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);

            //            //cameraMagnify.transform.localEulerAngles = Vector3.zero;
            //        }
            //    }
            //    else
            //    {
            //        if (transform.position.x <= Rayobject.transform.position.x)
            //        {
            //            //cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
            //            cameraMagnify.transform.localEulerAngles = Vector3.zero;
            //        }
            //        else
            //        {
            //            if (Mathf.Atan(m_o_tan) * 180f / Mathf.PI > Mathf.Atan(c_o_tan) * 180f / Mathf.PI)
            //            {
            //                cameraMagnify.transform.localEulerAngles = Vector3.zero;
            //            }
            //            else
            //            {
            //                cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, -Mathf.Atan(m_o_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
            //            }
            //        }
            //    }
            //}
            Test();
        }
        
        void Test()
        {
            //相机到放大镜的向量
            var c_m_tan = Mathf.Abs(Camera.main.transform.position.x - transform.position.x) / Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

            if (Rayobject != null)
            {
                //相机到检测物体的向量
                var c_o_tan = Mathf.Abs(Camera.main.transform.position.x - Rayobject.transform.position.x) / Mathf.Abs(Camera.main.transform.position.z - Rayobject.transform.position.z);
                //放大镜到检查物体的向量
                var m_o_tan = Mathf.Abs(transform.position.x - Rayobject.transform.position.x) / Mathf.Abs(transform.position.z - Rayobject.transform.position.z);

                if (transform.position.x >= Camera.main.transform.position.x)
                {
                    //if (transform.position.x <= Rayobject.transform.position.x)
                    //{
                    //    if (Mathf.Atan(m_o_tan) * 180f / Mathf.PI > Mathf.Atan(c_o_tan) * 180f / Mathf.PI)
                    //    {
                    //        //cameraMagnify.transform.localEulerAngles = Vector3.zero;
                    //        cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);



                    //        Rayobject = null;
                    //    }
                    //    else
                    //    {
                    //        cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(m_o_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                    //    }
                    //}
                    //else
                    //{


                    //    if (Mathf.Atan(m_o_tan) * 180f / Mathf.PI < 5f)
                    //    {

                    //    }
                    //    else



                    //        cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);

                    //    //cameraMagnify.transform.localEulerAngles = Vector3.zero;
                    //}

                    cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                }
                else
                {
                    //if (transform.position.x <= Rayobject.transform.position.x)
                    //{
                    //    //cameraMagnify.transform.localEulerAngles = Vector3.zero;
                    //    cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, -Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                    //}
                    //else
                    //{
                    //    if (Mathf.Atan(m_o_tan) * 180f / Mathf.PI > Mathf.Atan(c_o_tan) * 180f / Mathf.PI)
                    //    {
                    //        //cameraMagnify.transform.localEulerAngles = Vector3.zero;
                    //        cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, -Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                    //    }
                    //    else
                    //    {
                    //        cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, -Mathf.Atan(m_o_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                    //    }
                    //}

                    cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, -Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                }
            }
            else
            {
                if (transform.position.x >= Camera.main.transform.position.x)
                {
                    cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                }
                else
                {
                    cameraMagnify.transform.localEulerAngles = new Vector3(cameraMagnify.transform.localEulerAngles.x, -Mathf.Atan(c_m_tan) * 180f / Mathf.PI, cameraMagnify.transform.localEulerAngles.z);
                }
            }
        }


        private void OnDestroy()
        {

        }
    }
}