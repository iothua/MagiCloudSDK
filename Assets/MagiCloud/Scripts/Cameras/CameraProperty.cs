using UnityEngine;

namespace MagiCloud.Cameras
{
    /// <summary>
    /// 摄像机属性
    /// </summary>
    public struct CameraProperty
    {
        public Transform parent;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public CameraClearFlags clearFlags;
        public Color backgroundColor;
        

        public CameraProperty(Camera camera)
        {
            parent = camera.transform.parent;
            localPosition = camera.transform.localPosition;
            localRotation = camera.transform.localRotation;
            clearFlags = camera.clearFlags;
            backgroundColor = camera.backgroundColor;
        }

        /// <summary>
        /// 设置摄像机属性
        /// </summary>
        /// <param name="camera"></param>
        public void SetCameraProperty(Camera camera)
        {
            if (parent == null)
                camera.transform.parent = null;
            else
                camera.transform.SetParent(parent);

            camera.transform.localPosition = localPosition;
            camera.transform.localRotation = localRotation;
            camera.clearFlags = clearFlags;
            camera.backgroundColor = backgroundColor;
        }
    }
}
