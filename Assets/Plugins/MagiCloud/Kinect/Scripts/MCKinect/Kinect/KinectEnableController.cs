using System;
using System.Runtime.InteropServices;
using UnityEngine;
namespace MagiCloud.Kinect
{
    /// <summary>
    /// 根据当前窗口确定是否禁用手势
    /// </summary>
    public class KinectEnableController : MonoBehaviour
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]                             // 窗体句柄
        public static extern IntPtr GetActiveWindow();

        private static IntPtr currentIntPtr;

        void Start()
        {
            currentIntPtr = GetActiveWindow();
        }

        public static bool EnableKinect()
        {
#if UNITY_EDITOR

            return true;
#endif
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (currentIntPtr == GetActiveWindow())
            {
                return true;
            }
            return false;
#endif
            return true;
        }
    }
}