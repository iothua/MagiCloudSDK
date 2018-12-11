using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chemistry.Equipments
{
    public class EA_Magnify : MonoBehaviour
    {
        [SerializeField]
        private Camera camera;

        [SerializeField]
        private GameObject show;

        RenderTexture renderTexture;

        EO_Magnifier magnifier;

        [Header("调整视野"), Range(1f, 179f)]
        public float value = 1f;

        // Use this for initialization
        void Start()
        {
            renderTexture = Resources.Load<RenderTexture>("MagnifierUsedFreedom");

            if (renderTexture == null) return;
            camera.targetTexture = renderTexture;
            if (show != null)
                if (show.GetComponent<RawImage>() != null)
                    show.GetComponent<RawImage>().texture = renderTexture;

            magnifier = new EO_Magnifier(camera, 0f);
        }

        // Update is called once per frame
        void Update()
        {
            if (magnifier.IsChangeFieldOfView)
            {
                magnifier.FieldOfView = value;
            }
        }


        private void OnDestroy()
        {

        }
    }
}