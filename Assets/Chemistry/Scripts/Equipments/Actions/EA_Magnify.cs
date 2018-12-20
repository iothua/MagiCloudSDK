using MagiCloud.Features;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        RenderTexture renderTexture;

        EO_Magnifier magnifier;

        [Header("调整视野"), Range(1f, 179f)]
        public float value_View = 1f;


        [Header("调整正交视野"), Range(0f, 3f)]
        public float value_Size = 1f;

        // Use this for initialization
        void Start()
        {
            renderTexture = Resources.Load<RenderTexture>("MagnifierUsedFreedom");

            if (renderTexture == null) return;

            cameraMagnify.targetTexture = renderTexture;
            if (show != null)
                if (show.GetComponent<RawImage>() != null)
                    show.GetComponent<RawImage>().texture = renderTexture;

            magnifier = new EO_Magnifier(cameraMagnify, 0f);

            oldPos = cameraMagnify.transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            //if (magnifier != null)
            //    if (magnifier.IsChangeFieldOfView)
            //    {
            //        magnifier.FieldOfView = value_View;
            //    }

            if (magnifier != null)
                if (magnifier.IsChangeFieldOfSize)
                {
                    magnifier.FieldOfSize = value_Size;
                }


            OnRayDetection();
        }
        
        private void OnRayDetection()
        {
            //Ray ray = //Camera.main.ScreenPointToRay(Input.mousePosition);
            //Ray ray = new Ray(transform.position, Vector3.forward);

            Ray ray = new Ray(transform.position, transform.position - Camera.main.transform.position);
            
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                //画线
                Debug.DrawLine(ray.origin, hitInfo.point,Color.red);

                GameObject gameObj = hitInfo.collider.gameObject;
                Debug.Log("ray object" + gameObj.name);
                
                if (gameObj.GetComponent<OperaObject>() != null)
                {
                    //if( gameObj.GetComponent<OperaObject>().FeaturesObject)
                    cameraMagnify.transform.position = new Vector3(gameObj.GetComponent<OperaObject>().FeaturesObject.transform.position.x, cameraMagnify.transform.position.y, cameraMagnify.transform.position.z);
                }
                else
                {
                    cameraMagnify.transform.localPosition = oldPos;
                }
            }
            
        }







        private void OnDestroy()
        {

        }
    }
}