using UnityEngine;
using System.Collections;
//using Windows.Kinect;

/// <summary>
/// 获取关节位置Demo
/// </summary>
public class GetJointPositionDemo : MonoBehaviour
{
    // the joint we want to track
    /// <summary>
    /// 我们要追踪的关节
    /// </summary>
    public KinectInterop.JointType joint = KinectInterop.JointType.HandRight;

    // joint position at the moment, in Kinect coordinates
    /// <summary>
    /// 关节位置，在Kinect坐标中
    /// </summary>
    public Vector3 outputPosition;

    void Update()
    {
        KinectManager manager = KinectManager.Instance;

        if (manager && manager.IsInitialized())
        {
            if (manager.IsUserDetected())
            {
                long userId = manager.GetPrimaryUserID();

                if (manager.IsJointTracked(userId, (int)joint))
                {
                    outputPosition = manager.GetJointPosition(userId, (int)joint);
                }
            }
        }
    }
}
