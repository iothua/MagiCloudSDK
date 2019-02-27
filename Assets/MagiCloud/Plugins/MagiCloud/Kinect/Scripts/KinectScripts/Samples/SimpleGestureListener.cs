using UnityEngine;
//using Windows.Kinect;
using System.Collections;
using System;


public class SimpleGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
    // GUI Text to display the gesture messages.
    //显示手势消息的GUI文本
    public GUIText GestureInfo;

    // private bool to track if progress message has been displayed
    //跟踪进程消息是否已显示
    private bool progressDisplayed;

    /// <summary>
    /// 检测到用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="userIndex">用户索引</param>
    public void UserDetected(long userId, int userIndex)
	{
        // as an example - detect these user specific gestures
        //例如，检测这些用户的特定手势
        KinectManager manager = KinectManager.Instance;
		manager.DetectGesture(userId, KinectGestures.Gestures.Jump);
		manager.DetectGesture(userId, KinectGestures.Gestures.Squat);
		manager.DetectGesture(userId, KinectGestures.Gestures.Push);
		manager.DetectGesture(userId, KinectGestures.Gestures.Pull);
		
//		manager.DetectGesture(userId, KinectGestures.Gestures.SwipeUp);
//		manager.DetectGesture(userId, KinectGestures.Gestures.SwipeDown);
		
		if(GestureInfo != null)
		{
			GestureInfo.GetComponent<GUIText>().text = "SwipeLeft, SwipeRight, Squat, Push or Pull.";
		}
	}
	
    /// <summary>
    /// 用户丢失
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="userIndex">用户索引</param>
	public void UserLost(long userId, int userIndex)
	{
		if(GestureInfo != null)
		{
			GestureInfo.GetComponent<GUIText>().text = string.Empty;
		}
	}

    /// <summary>
    /// 手势进行中
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="userIndex">用户索引</param>
    /// <param name="gesture">手势</param>
    /// <param name="progress">进展</param>
    /// <param name="joint">关节</param>
    /// <param name="screenPos">屏幕坐标</param>
	public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectInterop.JointType joint, Vector3 screenPos)
	{
		//GestureInfo.guiText.text = string.Format("{0} Progress: {1:F1}%", gesture, (progress * 100));
//		if(gesture == KinectGestures.Gestures.Click && progress > 0.3f)
//		{
//			string sGestureText = string.Format ("{0} {1:F1}% complete", gesture, progress * 100);
//			if(GestureInfo != null)
//				GestureInfo.guiText.text = sGestureText;
//			
//			progressDisplayed = true;
//		}		
//		else 
		if((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
		{
			string sGestureText = string.Format ("{0} detected, zoom={1:F1}%", gesture, screenPos.z * 100);
			if(GestureInfo != null)
				GestureInfo.GetComponent<GUIText>().text = sGestureText;
			
			progressDisplayed = true;
		}
		else if(gesture == KinectGestures.Gestures.Wheel && progress > 0.5f)
		{
			string sGestureText = string.Format ("{0} detected, angle={1:F1} deg", gesture, screenPos.z);
			if(GestureInfo != null)
				GestureInfo.GetComponent<GUIText>().text = sGestureText;
			
			progressDisplayed = true;
		}
	}

    /// <summary>
    /// 手势完成
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="userIndex">用户索引</param>
    /// <param name="gesture">手势</param>
    /// <param name="joint">关节</param>
    /// <param name="screenPos">屏幕坐标</param>
    /// <returns></returns>
	public bool GestureCompleted (long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint, Vector3 screenPos)
	{
		string sGestureText = gesture + " detected";
//		if(gesture == KinectGestures.Gestures.Click)
//			sGestureText += string.Format(" at ({0:F1}, {1:F1})", screenPos.x, screenPos.y);
		
		if(GestureInfo != null)
			GestureInfo.GetComponent<GUIText>().text = sGestureText;
		
		progressDisplayed = false;
		
		return true;
	}

    /// <summary>
    /// 取消手势
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="userIndex">用户索引</param>
    /// <param name="gesture">手势</param>
    /// <param name="joint">关节</param>
    /// <returns></returns>
	public bool GestureCancelled (long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint)
	{
		if(progressDisplayed)
		{
			// clear the progress info
            //清除进展信息
			if(GestureInfo != null)
				GestureInfo.GetComponent<GUIText>().text = String.Empty;
			
			progressDisplayed = false;
		}
		
		return true;
	}
	
}
