using UnityEngine;
//using Windows.Kinect;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Kinect手势
/// </summary>
public class KinectGestures
{
    /// <summary>
    /// 手势监听接口
    /// </summary>
	public interface GestureListenerInterface
	{
        // Invoked when a new user is detected and tracking starts
        // Here you can start gesture detection with KinectManager.DetectGesture()
        /// <summary>
        /// 当检测到新用户并启动跟踪时调用此处您可以使用KinectManager.DetectGesture（）开始手势检测。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userIndex">用户索引</param>
        void UserDetected(long userId, int userIndex);

        // Invoked when a user is lost
        // Gestures for this user are cleared automatically, but you can free the used resources
        /// <summary>
        /// 用户丢失时调用此用户的手势会自动清除，但您可以释放所使用的资源。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userIndex">用户索引</param>
        void UserLost(long userId, int userIndex);

        // Invoked when a gesture is in progress 
        /// <summary>
        /// 当一个手势在进步时被调用
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userIndex">用户索引</param>
        /// <param name="gesture">手势</param>
        /// <param name="progress">进展</param>
        /// <param name="joint">关节</param>
        /// <param name="screenPos">场景位置</param>
        void GestureInProgress(long userId, int userIndex, Gestures gesture, float progress, 
		                       KinectInterop.JointType joint, Vector3 screenPos);

        // Invoked if a gesture is completed.
        // Returns true, if the gesture detection must be restarted, false otherwise
        /// <summary>
        /// 如果一个手势已经完成，调用。
        /// 返回true，如果这个手势检测必须重新启动，否则将会出现错误。
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userIndex">用户索引</param>
        /// <param name="gesture">手势</param>
        /// <param name="joint">关节</param>
        /// <param name="screenPos">场景坐标</param>
        /// <returns></returns>
        bool GestureCompleted(long userId, int userIndex, Gestures gesture,
		                      KinectInterop.JointType joint, Vector3 screenPos);

        // Invoked if a gesture is cancelled.
        // Returns true, if the gesture detection must be retarted, false otherwise
        /// <summary>
        /// 取消手势
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userIndex">用户索引</param>
        /// <param name="gesture">手势</param>
        /// <param name="joint">关节</param>
        /// <returns></returns>
        bool GestureCancelled(long userId, int userIndex, Gestures gesture, 
		                      KinectInterop.JointType joint);
	}
	
	/// <summary>
    /// 手势
    /// </summary>
	public enum Gestures
	{
        /// <summary>
        /// 无
        /// </summary>
		None = 0,
        /// <summary>
        /// 举起右手
        /// </summary>
		RaiseRightHand,
        /// <summary>
        /// 举起左手
        /// </summary>
		RaiseLeftHand,
        /// <summary>
        /// 双手举起过肩并保持至少一秒
        /// </summary>
		Psi,
        /// <summary>
        /// 双手伸直站立 识别精度好
        /// </summary>
		Tpose,
        /// <summary>
        /// 双手下垂
        /// </summary>
		Stop,
        /// <summary>
        /// 左手或右手举起来回摆动
        /// </summary>
		Wave,
        ///// <summary>
        ///// 左手或右手在适当的位置停留至少2.5秒.
        ///// </summary>
        //Click,
        /// <summary>
        /// 右手向左挥
        /// </summary>
        SwipeLeft,
        /// <summary>
        /// 左手向右挥
        /// </summary>
		SwipeRight,
        /// <summary>
        /// 左手或者右手向上挥
        /// </summary>
		SwipeUp,
        /// <summary>
        /// 左手或者右手向下挥
        /// </summary>
		SwipeDown,
  //      /// <summary>
  //      /// 假手势，用来使光标随着手移动
  //      /// </summary>
		//RightHandCursor,
		//LeftHandCursor,
        /// <summary>
        /// 缩小
        /// </summary>
		ZoomOut,
        /// <summary>
        /// 放大
        /// </summary>
		ZoomIn,
        /// <summary>
        /// 盘旋
        /// </summary>
		Wheel,
        /// <summary>
        /// 跳
        /// </summary>
		Jump,
        /// <summary>
        /// 蹲
        /// </summary>
		Squat,
        /// <summary>
        /// 推
        /// </summary>
		Push,
        /// <summary>
        /// 拉
        /// </summary>
        Pull,
        RightHandGrip,
        RightHandGrip2,
        LeftHandGrip,
        LeftHandGrip2,
        HandRelease
	}
	
	/// <summary>
    /// 手势数据
    /// </summary>
	public struct GestureData
	{
        /// <summary>
        /// 用户ID
        /// </summary>
		public long userId;
        /// <summary>
        /// 手势
        /// </summary>
		public Gestures gesture;
        /// <summary>
        /// 状态
        /// </summary>
		public int state;

        /// <summary>
        /// 时间戳
        /// </summary>
		public float timestamp;
        /// <summary>
        /// 关节
        /// </summary>
		public int joint;
        /// <summary>
        /// 关节坐标
        /// </summary>
		public Vector3 jointPos;
        /// <summary>
        /// 屏幕坐标
        /// </summary>
		public Vector3 screenPos;
        /// <summary>
        /// tag浮点值
        /// </summary>
		public float tagFloat;
        
		public Vector3 tagVector;
        
		public Vector3 tagVector2;
        /// <summary>
        /// 进展
        /// </summary>
		public float progress;
        /// <summary>
        /// 完成
        /// </summary>
		public bool complete;
        /// <summary>
        /// 取消
        /// </summary>
		public bool cancelled;
        /// <summary>
        /// 检查手势
        /// </summary>
		public List<Gestures> checkForGestures;
        /// <summary>
        /// 开始跟踪时间
        /// </summary>
		public float startTrackingAtTime;
	}



    // Gesture related constants, variables and functions
    //手势相关的常数、变量和函数
    /// <summary>
    /// 左手常数
    /// </summary>
    private const int leftHandIndex = (int)KinectInterop.JointType.HandLeft;
    /// <summary>
    /// 右手常数
    /// </summary>
	private const int rightHandIndex = (int)KinectInterop.JointType.HandRight;
    /// <summary>
    /// 左手肘常数
    /// </summary>	
    private const int leftElbowIndex = (int)KinectInterop.JointType.ElbowLeft;
    /// <summary>
    /// 右手肘常数
    /// </summary>
	private const int rightElbowIndex = (int)KinectInterop.JointType.ElbowRight;
    /// <summary>
    /// 左肩常数
    /// </summary>	
    private const int leftShoulderIndex = (int)KinectInterop.JointType.ShoulderLeft;
    /// <summary>
    /// 右肩常数
    /// </summary>
	private const int rightShoulderIndex = (int)KinectInterop.JointType.ShoulderRight;
    /// <summary>
    /// 髋关节中心常数
    /// </summary>
    private const int hipCenterIndex = (int)KinectInterop.JointType.SpineBase;
    /// <summary>
    /// 肩中心常数
    /// </summary>
	private const int shoulderCenterIndex = (int)KinectInterop.JointType.SpineShoulder;
    /// <summary>
    /// 左髋常数
    /// </summary>
	private const int leftHipIndex = (int)KinectInterop.JointType.HipLeft;
    /// <summary>
    /// 右髋常数
    /// </summary>
	private const int rightHipIndex = (int)KinectInterop.JointType.HipRight;

	private const int rightTipIndex = (int)KinectInterop.JointType.HandTipRight;
    private const int leftTipIndex = (int)KinectInterop.JointType.HandTipLeft;
	
    /// <summary>
    /// 需要的关节数集合
    /// </summary>
	private static int[] neededJointIndexes = {
		leftHandIndex, rightHandIndex, leftElbowIndex, rightElbowIndex, leftShoulderIndex, rightShoulderIndex,
		hipCenterIndex, shoulderCenterIndex, leftHipIndex, rightHipIndex,rightTipIndex,leftTipIndex
	};


    // Returns the list of the needed gesture joint indexes
    /// <summary>
    /// 返回所需的手势关节索引的列表
    /// </summary>
    /// <returns></returns>
    public static int[] GetNeededJointIndexes()
	{
		return neededJointIndexes;
	}

    /// <summary>
    /// 设置手势关节
    /// </summary>
    /// <param name="gestureData">手势数据</param>
    /// <param name="timestamp">时间戳</param>
    /// <param name="joint">关节</param>
    /// <param name="jointPos">关节位置</param>
    private static void SetGestureJoint(ref GestureData gestureData, float timestamp, int joint, Vector3 jointPos)
	{
		gestureData.joint = joint;
		gestureData.jointPos = jointPos;
		gestureData.timestamp = timestamp;
		gestureData.state++;
	}
	
    /// <summary>
    /// 设置手势取消
    /// </summary>
    /// <param name="gestureData">手势数据</param>
	private static void SetGestureCancelled(ref GestureData gestureData)
	{
		gestureData.state = 0;
		gestureData.progress = 0f;
		gestureData.cancelled = true;
	}

    /// <summary>
    /// 检查姿势完成
    /// </summary>
    /// <param name="gestureData">手势数据</param>
    /// <param name="timestamp">时间戳</param>
    /// <param name="jointPos">关节位置</param>
    /// <param name="isInPose">是姿势？</param>
    /// <param name="durationToComplete">持续时间完成</param>
    private static void CheckPoseComplete(ref GestureData gestureData, float timestamp, Vector3 jointPos, bool isInPose, float durationToComplete)
	{
		if(isInPose)
		{
			float timeLeft = timestamp - gestureData.timestamp;
			gestureData.progress = durationToComplete > 0f ? Mathf.Clamp01(timeLeft / durationToComplete) : 1.0f;
	
			if(timeLeft >= durationToComplete)
			{
				gestureData.timestamp = timestamp;
				gestureData.jointPos = jointPos;
				gestureData.state++;
				gestureData.complete = true;
			}
		}
		else
		{
			SetGestureCancelled(ref gestureData);
		}
	}
	/// <summary>
    /// 设置屏幕位置
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="gestureData">手势数据</param>
    /// <param name="jointsPos">关节位置</param>
    /// <param name="jointsTracked">关节跟踪</param>
	private static void SetScreenPos(long userId, ref GestureData gestureData, ref Vector3[] jointsPos, ref bool[] jointsTracked)
	{
		Vector3 handPos = jointsPos[rightHandIndex];
//		Vector3 elbowPos = jointsPos[rightElbowIndex];
//		Vector3 shoulderPos = jointsPos[rightShoulderIndex];
		bool calculateCoords = false;
		
		if(gestureData.joint == rightHandIndex)
		{
			if(jointsTracked[rightHandIndex] /**&& jointsTracked[rightElbowIndex] && jointsTracked[rightShoulderIndex]*/)
			{
				calculateCoords = true;
			}
		}
		else if(gestureData.joint == leftHandIndex)
		{
			if(jointsTracked[leftHandIndex] /**&& jointsTracked[leftElbowIndex] && jointsTracked[leftShoulderIndex]*/)
			{
				handPos = jointsPos[leftHandIndex];
//				elbowPos = jointsPos[leftElbowIndex];
//				shoulderPos = jointsPos[leftShoulderIndex];
				
				calculateCoords = true;
			}
		}
		
		if(calculateCoords)
		{
//			if(gestureData.tagFloat == 0f || gestureData.userId != userId)
//			{
//				// get length from shoulder to hand (screen range)
//				Vector3 shoulderToElbow = elbowPos - shoulderPos;
//				Vector3 elbowToHand = handPos - elbowPos;
//				gestureData.tagFloat = (shoulderToElbow.magnitude + elbowToHand.magnitude);
//			}
			
			if(jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && 
				jointsTracked[leftShoulderIndex] && jointsTracked[rightShoulderIndex])
			{
				Vector3 shoulderToHips = jointsPos[shoulderCenterIndex] - jointsPos[hipCenterIndex];
				Vector3 rightToLeft = jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex];
				
				gestureData.tagVector2.x = rightToLeft.x; // * 1.2f;
				gestureData.tagVector2.y = shoulderToHips.y; // * 1.2f;
				
				if(gestureData.joint == rightHandIndex)
				{
					gestureData.tagVector.x = jointsPos[rightShoulderIndex].x - gestureData.tagVector2.x / 2;
					gestureData.tagVector.y = jointsPos[hipCenterIndex].y;
				}
				else
				{
					gestureData.tagVector.x = jointsPos[leftShoulderIndex].x - gestureData.tagVector2.x / 2;
					gestureData.tagVector.y = jointsPos[hipCenterIndex].y;
				}
			}
	
//			Vector3 shoulderToHand = handPos - shoulderPos;
//			gestureData.screenPos.x = Mathf.Clamp01((gestureData.tagFloat / 2 + shoulderToHand.x) / gestureData.tagFloat);
//			gestureData.screenPos.y = Mathf.Clamp01((gestureData.tagFloat / 2 + shoulderToHand.y) / gestureData.tagFloat);
			
			if(gestureData.tagVector2.x != 0 && gestureData.tagVector2.y != 0)
			{
				Vector3 relHandPos = handPos - gestureData.tagVector;
				gestureData.screenPos.x = Mathf.Clamp01(relHandPos.x / gestureData.tagVector2.x);
				gestureData.screenPos.y = Mathf.Clamp01(relHandPos.y / gestureData.tagVector2.y);
			}
			
			//Debug.Log(string.Format("{0} - S: {1}, H: {2}, SH: {3}, L : {4}", gestureData.gesture, shoulderPos, handPos, shoulderToHand, gestureData.tagFloat));
		}
	}
	
    /// <summary>
    /// 设置缩放因子
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="gestureData">手势数据</param>
    /// <param name="initialZoom">初始缩放</param>
    /// <param name="jointsPos">关节位置</param>
    /// <param name="jointsTracked">关节跟踪</param>
	private static void SetZoomFactor(long userId, ref GestureData gestureData, float initialZoom, ref Vector3[] jointsPos, ref bool[] jointsTracked)
	{
		Vector3 vectorZooming = jointsPos[rightHandIndex] - jointsPos[leftHandIndex];
		
		if(gestureData.tagFloat == 0f || gestureData.userId != userId)
		{
			gestureData.tagFloat = 0.5f; // this is 100%
		}

		float distZooming = vectorZooming.magnitude;
		gestureData.screenPos.z = initialZoom + (distZooming / gestureData.tagFloat);
	}
	
    /// <summary>
    /// 设置轮转
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="gestureData">手势数据</param>
    /// <param name="initialPos">初始位置</param>
    /// <param name="currentPos">当前位置</param>
	private static void SetWheelRotation(long userId, ref GestureData gestureData, Vector3 initialPos, Vector3 currentPos)
	{
		float angle = Vector3.Angle(initialPos, currentPos) * Mathf.Sign(currentPos.y - initialPos.y);
		gestureData.screenPos.z = angle;
	}

    // estimate the next state and completeness of the gesture
    /// <summary>
    /// 判断手势的下一个状态和完整性
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="gestureData">手势数据</param>
    /// <param name="timestamp">时间戳</param>
    /// <param name="jointsPos">关节位置</param>
    /// <param name="jointsTracked">关节跟踪</param>
    public static void CheckForGesture(long userId, ref GestureData gestureData, float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked)
	{
		if(gestureData.complete)
			return;
		
		switch(gestureData.gesture)
		{
            // check for RaiseRightHand
            //是否举起右手
            case Gestures.RaiseRightHand:
				switch(gestureData.state)
				{
					case 0:  // gesture detection//手势检测
                        if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) > 0.1f)
						{
                            //设置手势关节
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
						}
						break;
							
					case 1:  // gesture complete//动作完成
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
							(jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) > 0.1f;

						Vector3 jointPos = jointsPos[gestureData.joint];
                        //检查姿势完成
						CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
						break;
				}
				break;

            // check for RaiseLeftHand
            //是否举起左手
            case Gestures.RaiseLeftHand:
				switch(gestureData.state)
				{
					case 0:  // gesture detection//手势检测
                        if (jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
					            (jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) > 0.1f)
						{
							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
						}
						break;
							
					case 1:  // gesture complete//动作完成
                        bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
							(jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) > 0.1f;

						Vector3 jointPos = jointsPos[gestureData.joint];
						CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
						break;
				}
				break;

			// check for Psi
            //检查Psi
			case Gestures.Psi:
				switch(gestureData.state)
				{
					case 0:  // gesture detection//手势检测
                        if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) > 0.1f &&
					       jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
					       (jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) > 0.1f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
						}
						break;
							
					case 1:  // gesture complete//动作完成
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
							(jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) > 0.1f &&
							jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
							(jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) > 0.1f;

						Vector3 jointPos = jointsPos[gestureData.joint];
						CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
						break;
				}
				break;

			// check for Tpose
			case Gestures.Tpose:
				switch(gestureData.state)
				{
				case 0:  // gesture detection
					if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[rightShoulderIndex] &&
				       Mathf.Abs(jointsPos[rightElbowIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.07f
				       Mathf.Abs(jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.7f
				   	   jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[leftShoulderIndex] &&
				  	   Mathf.Abs(jointsPos[leftElbowIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f &&
				       Mathf.Abs(jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f)
					{
						SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
					}
					break;
					
				case 1:  // gesture complete
					bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[rightShoulderIndex] &&
							Mathf.Abs(jointsPos[rightElbowIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.7f
						    Mathf.Abs(jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.7f
						    jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[leftShoulderIndex] &&
							Mathf.Abs(jointsPos[leftElbowIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f &&
						    Mathf.Abs(jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f;
					
					Vector3 jointPos = jointsPos[gestureData.joint];
					CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
					break;
				}
				break;
				
			// check for Stop
			case Gestures.Stop:
				switch(gestureData.state)
				{
					case 0:  // gesture detection
						if(jointsTracked[rightHandIndex] && jointsTracked[rightHipIndex] &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightHipIndex].y) < 0f &&
					       jointsTracked[leftHandIndex] && jointsTracked[leftHipIndex] &&
					       (jointsPos[leftHandIndex].y - jointsPos[leftHipIndex].y) < 0f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
						}
						break;
							
					case 1:  // gesture complete
						bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightHipIndex] &&
							(jointsPos[rightHandIndex].y - jointsPos[rightHipIndex].y) < 0f &&
							jointsTracked[leftHandIndex] && jointsTracked[leftHipIndex] &&
							(jointsPos[leftHandIndex].y - jointsPos[leftHipIndex].y) < 0f;

						Vector3 jointPos = jointsPos[gestureData.joint];
						CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
						break;
				}
				break;

			// check for Wave
			case Gestures.Wave:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0.1f &&
					       (jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) > 0.05f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.progress = 0.3f;
						}
						else if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
					            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0.1f &&
					            (jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) < -0.05f)
						{
							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
							gestureData.progress = 0.3f;
						}
						break;
				
					case 1:  // gesture - phase 2
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = gestureData.joint == rightHandIndex ?
								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
								(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0.1f && 
								(jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) < -0.05f :
								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
								(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0.1f &&
								(jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) > 0.05f;
				
							if(isInPose)
							{
								gestureData.timestamp = timestamp;
								gestureData.state++;
								gestureData.progress = 0.7f;
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
									
					case 2:  // gesture phase 3 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = gestureData.joint == rightHandIndex ?
								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
								(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0.1f && 
								(jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) > 0.05f :
								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
								(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0.1f &&
								(jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) < -0.05f;

							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;
            #region 注释

            //			// check for Click
            //			case Gestures.Click:
            //				switch(gestureData.state)
            //				{
            //					case 0:  // gesture detection - phase 1
            //						if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
            //					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f)
            //						{
            //							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
            //							gestureData.progress = 0.3f;
            //
            //							// set screen position at the start, because this is the most accurate click position
            //							SetScreenPos(userId, ref gestureData, ref jointsPos, ref jointsTracked);
            //						}
            //						else if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
            //					            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f)
            //						{
            //							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
            //							gestureData.progress = 0.3f;
            //
            //							// set screen position at the start, because this is the most accurate click position
            //							SetScreenPos(userId, ref gestureData, ref jointsPos, ref jointsTracked);
            //						}
            //						break;
            //				
            //					case 1:  // gesture - phase 2
            ////						if((timestamp - gestureData.timestamp) < 1.0f)
            ////						{
            ////							bool isInPose = gestureData.joint == rightHandIndex ?
            ////								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
            ////								//(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f && 
            ////								Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.08f &&
            ////								(jointsPos[rightHandIndex].z - gestureData.jointPos.z) < -0.05f :
            ////								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
            ////								//(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
            ////								Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.08f &&
            ////								(jointsPos[leftHandIndex].z - gestureData.jointPos.z) < -0.05f;
            ////				
            ////							if(isInPose)
            ////							{
            ////								gestureData.timestamp = timestamp;
            ////								gestureData.jointPos = jointsPos[gestureData.joint];
            ////								gestureData.state++;
            ////								gestureData.progress = 0.7f;
            ////							}
            ////							else
            ////							{
            ////								// check for stay-in-place
            ////								Vector3 distVector = jointsPos[gestureData.joint] - gestureData.jointPos;
            ////								isInPose = distVector.magnitude < 0.05f;
            ////
            ////								Vector3 jointPos = jointsPos[gestureData.joint];
            ////								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, Constants.ClickStayDuration);
            ////							}
            ////						}
            ////						else
            //						{
            //							// check for stay-in-place
            //							Vector3 distVector = jointsPos[gestureData.joint] - gestureData.jointPos;
            //							bool isInPose = distVector.magnitude < 0.05f;
            //
            //							Vector3 jointPos = jointsPos[gestureData.joint];
            //							CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.ClickStayDuration);
            ////							SetGestureCancelled(gestureData);
            //						}
            //						break;
            //									
            ////					case 2:  // gesture phase 3 = complete
            ////						if((timestamp - gestureData.timestamp) < 1.0f)
            ////						{
            ////							bool isInPose = gestureData.joint == rightHandIndex ?
            ////								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
            ////								//(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f && 
            ////								Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.08f &&
            ////								(jointsPos[rightHandIndex].z - gestureData.jointPos.z) > 0.05f :
            ////								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
            ////								//(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
            ////								Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.08f &&
            ////								(jointsPos[leftHandIndex].z - gestureData.jointPos.z) > 0.05f;
            ////
            ////							if(isInPose)
            ////							{
            ////								Vector3 jointPos = jointsPos[gestureData.joint];
            ////								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
            ////							}
            ////						}
            ////						else
            ////						{
            ////							// cancel the gesture
            ////							SetGestureCancelled(ref gestureData);
            ////						}
            ////						break;
            //				}
            //				break;
            #endregion

            // check for SwipeLeft
            case Gestures.SwipeLeft:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.05f &&
					       (jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) > 0f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.progress = 0.5f;
						}
//						else if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
//					            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.05f &&
//					            (jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) > 0f)
//						{
//							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
//							//gestureData.jointPos = jointsPos[leftHandIndex];
//							gestureData.progress = 0.5f;
//						}
						break;
				
					case 1:  // gesture phase 2 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = gestureData.joint == rightHandIndex ?
								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
								Mathf.Abs(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) < 0.1f && 
								Mathf.Abs(jointsPos[rightHandIndex].y - gestureData.jointPos.y) < 0.08f && 
								(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < -0.15f :
								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
								Mathf.Abs(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) < 0.1f &&
								Mathf.Abs(jointsPos[leftHandIndex].y - gestureData.jointPos.y) < 0.08f && 
								(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < -0.15f;

							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;

			// check for SwipeRight
			case Gestures.SwipeRight:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
//						if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
//					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.05f &&
//					       (jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) < 0f)
//						{
//							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
//							//gestureData.jointPos = jointsPos[rightHandIndex];
//							gestureData.progress = 0.5f;
//						}
//						else 
						if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
					            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.05f &&
					            (jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) < 0f)
						{
							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
							gestureData.progress = 0.5f;
						}
						break;
				
					case 1:  // gesture phase 2 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = gestureData.joint == rightHandIndex ?
								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
								Mathf.Abs(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) < 0.1f && 
								Mathf.Abs(jointsPos[rightHandIndex].y - gestureData.jointPos.y) < 0.08f && 
								(jointsPos[rightHandIndex].x - gestureData.jointPos.x) > 0.15f :
								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
								Mathf.Abs(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) < 0.1f &&
								Mathf.Abs(jointsPos[leftHandIndex].y - gestureData.jointPos.y) < 0.08f && 
								(jointsPos[leftHandIndex].x - gestureData.jointPos.x) > 0.15f;

							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;

			// check for SwipeUp
			case Gestures.SwipeUp:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) < -0.05f &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.15f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.progress = 0.5f;
						}
						else if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
					            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) < -0.05f &&
					            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.15f)
						{
							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
							gestureData.progress = 0.5f;
						}
						break;
				
					case 1:  // gesture phase 2 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = gestureData.joint == rightHandIndex ?
								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
								//(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0.1f && 
								//(jointsPos[rightHandIndex].y - gestureData.jointPos.y) > 0.15f && 
								(jointsPos[rightHandIndex].y - jointsPos[leftShoulderIndex].y) > 0.05f && 
								Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.08f :
								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
								//(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0.1f &&
								//(jointsPos[leftHandIndex].y - gestureData.jointPos.y) > 0.15f && 
								(jointsPos[leftHandIndex].y - jointsPos[rightShoulderIndex].y) > 0.05f && 
								Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.08f;

							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;

			// check for SwipeDown
			case Gestures.SwipeDown:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						if(jointsTracked[rightHandIndex] && jointsTracked[leftShoulderIndex] &&
					       (jointsPos[rightHandIndex].y - jointsPos[leftShoulderIndex].y) >= 0.05f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.progress = 0.5f;
						}
						else if(jointsTracked[leftHandIndex] && jointsTracked[rightShoulderIndex] &&
					            (jointsPos[leftHandIndex].y - jointsPos[rightShoulderIndex].y) >= 0.05f)
						{
							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
							gestureData.progress = 0.5f;
						}
						break;
				
					case 1:  // gesture phase 2 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = gestureData.joint == rightHandIndex ?
								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
								//(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) < -0.1f && 
								(jointsPos[rightHandIndex].y - gestureData.jointPos.y) < -0.2f && 
								Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.08f :
								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
								//(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) < -0.1f &&
								(jointsPos[leftHandIndex].y - gestureData.jointPos.y) < -0.2f && 
								Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.08f;

							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;
            #region 注释

            //			// check for RightHandCursor
            //			case Gestures.RightHandCursor:
            //				switch(gestureData.state)
            //				{
            //					case 0:  // gesture detection - phase 1 (perpetual)
            //						if(jointsTracked[rightHandIndex] && jointsTracked[rightHipIndex] &&
            //							//(jointsPos[rightHandIndex].y - jointsPos[rightHipIndex].y) > -0.1f)
            //				   			(jointsPos[rightHandIndex].y - jointsPos[hipCenterIndex].y) >= 0f)
            //						{
            //							gestureData.joint = rightHandIndex;
            //							gestureData.timestamp = timestamp;
            //							gestureData.jointPos = jointsPos[rightHandIndex];
            //
            //							SetScreenPos(userId, ref gestureData, ref jointsPos, ref jointsTracked);
            //							gestureData.progress = 0.7f;
            //						}
            //						else
            //						{
            //							// cancel the gesture
            //							//SetGestureCancelled(ref gestureData);
            //							gestureData.progress = 0f;
            //						}
            //						break;
            //				
            //				}
            //				break;
            //
            //			// check for LeftHandCursor
            //			case Gestures.LeftHandCursor:
            //				switch(gestureData.state)
            //				{
            //					case 0:  // gesture detection - phase 1 (perpetual)
            //						if(jointsTracked[leftHandIndex] && jointsTracked[leftHipIndex] &&
            //							//(jointsPos[leftHandIndex].y - jointsPos[leftHipIndex].y) > -0.1f)
            //							(jointsPos[leftHandIndex].y - jointsPos[hipCenterIndex].y) >= 0f)
            //						{
            //							gestureData.joint = leftHandIndex;
            //							gestureData.timestamp = timestamp;
            //							gestureData.jointPos = jointsPos[leftHandIndex];
            //
            //							SetScreenPos(userId, ref gestureData, ref jointsPos, ref jointsTracked);
            //							gestureData.progress = 0.7f;
            //						}
            //						else
            //						{
            //							// cancel the gesture
            //							//SetGestureCancelled(ref gestureData);
            //							gestureData.progress = 0f;
            //						}
            //						break;
            //				
            //				}
            //				break;
            #endregion

            // check for ZoomOut
            case Gestures.ZoomOut:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						float distZoomOut = ((Vector3)(jointsPos[rightHandIndex] - jointsPos[leftHandIndex])).magnitude;
				
						if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
						   jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
					       (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0f &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0f &&
						   distZoomOut < 0.2f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.progress = 0.3f;
						}
						break;
				
					case 1:  // gesture phase 2 = zooming
						if((timestamp - gestureData.timestamp) < 1.0f)
						{
							bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
					   			jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
								((jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0f ||
				       			(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0f);

							if(isInPose)
							{
								SetZoomFactor(userId, ref gestureData, 1.0f, ref jointsPos, ref jointsTracked);
								gestureData.timestamp = timestamp;
								gestureData.progress = 0.7f;
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;

			// check for ZoomIn
			case Gestures.ZoomIn:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						float distZoomIn = ((Vector3)jointsPos[rightHandIndex] - jointsPos[leftHandIndex]).magnitude;

						if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
						   jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
					       (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0f &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0f &&
						   distZoomIn >= 0.7f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.tagFloat = distZoomIn;
							gestureData.progress = 0.3f;
						}
						break;
				
					case 1:  // gesture phase 2 = zooming
						if((timestamp - gestureData.timestamp) < 1.0f)
						{
							bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
					   			jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
								((jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0f ||
				       			(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0f);

							if(isInPose)
							{
								SetZoomFactor(userId, ref gestureData, 0.0f, ref jointsPos, ref jointsTracked);
								gestureData.timestamp = timestamp;
								gestureData.progress = 0.7f;
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;

			// check for Wheel
			case Gestures.Wheel:
				Vector3 vectorWheel = (Vector3)jointsPos[rightHandIndex] - jointsPos[leftHandIndex];
				float distWheel = vectorWheel.magnitude;

				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
						   jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
					       (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0f &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0f &&
						   distWheel > 0.2f && distWheel < 0.7f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.tagVector = vectorWheel;
							gestureData.tagFloat = distWheel;
							gestureData.progress = 0.3f;
						}
						break;
				
					case 1:  // gesture phase 2 = zooming
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
					   			jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
								((jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0f ||
				       			(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0f &&
								Mathf.Abs(distWheel - gestureData.tagFloat) < 0.1f);

							if(isInPose)
							{
								SetWheelRotation(userId, ref gestureData, gestureData.tagVector, vectorWheel);
								gestureData.timestamp = timestamp;
								gestureData.tagFloat = distWheel;
								gestureData.progress = 0.7f;
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;
			
			// check for Jump
			case Gestures.Jump:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						if(jointsTracked[hipCenterIndex] && 
							(jointsPos[hipCenterIndex].y > 0.8f) && (jointsPos[hipCenterIndex].y < 1.3f))
						{
							SetGestureJoint(ref gestureData, timestamp, hipCenterIndex, jointsPos[hipCenterIndex]);
							gestureData.progress = 0.5f;
						}
						break;
				
					case 1:  // gesture phase 2 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = jointsTracked[hipCenterIndex] &&
								(jointsPos[hipCenterIndex].y - gestureData.jointPos.y) > 0.15f && 
								Mathf.Abs(jointsPos[hipCenterIndex].x - gestureData.jointPos.x) < 0.15f;

							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;

			// check for Squat
			case Gestures.Squat:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						if(jointsTracked[hipCenterIndex] && 
							(jointsPos[hipCenterIndex].y < 0.8f))
						{
							SetGestureJoint(ref gestureData, timestamp, hipCenterIndex, jointsPos[hipCenterIndex]);
							gestureData.progress = 0.5f;
						}
						break;
				
					case 1:  // gesture phase 2 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = jointsTracked[hipCenterIndex] &&
								(jointsPos[hipCenterIndex].y - gestureData.jointPos.y) < -0.15f && 
								Mathf.Abs(jointsPos[hipCenterIndex].x - gestureData.jointPos.x) < 0.15f;

							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;

			// check for Push
			case Gestures.Push:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.05f &&
					       Mathf.Abs(jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) < 0.15f &&
						   (jointsPos[rightHandIndex].z - jointsPos[rightElbowIndex].z) < -0.05f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.progress = 0.5f;
						}
						else if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
					            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.05f &&
					            Mathf.Abs(jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) < 0.15f &&
							    (jointsPos[leftHandIndex].z - jointsPos[leftElbowIndex].z) < -0.05f)
						{
							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
							gestureData.progress = 0.5f;
						}
						break;
				
					case 1:  // gesture phase 2 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = gestureData.joint == rightHandIndex ?
								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
								Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.15f && 
								Mathf.Abs(jointsPos[rightHandIndex].y - gestureData.jointPos.y) < 0.15f && 
								(jointsPos[rightHandIndex].z - gestureData.jointPos.z) < -0.15f :
								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
								Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.15f &&
								Mathf.Abs(jointsPos[leftHandIndex].y - gestureData.jointPos.y) < 0.15f && 
								(jointsPos[leftHandIndex].z - gestureData.jointPos.z) < -0.15f;

							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
						}
						else
						{
							// cancel the gesture
							SetGestureCancelled(ref gestureData);
						}
						break;
				}
				break;

			// check for Pull
			case Gestures.Pull:
				switch(gestureData.state)
				{
					case 0:  // gesture detection - phase 1
						if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.05f &&
					       Mathf.Abs(jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) < 0.15f &&
						   (jointsPos[rightHandIndex].z - jointsPos[rightElbowIndex].z) < -0.15f)
						{
							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
							gestureData.progress = 0.5f;
						}
						else if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
					            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.05f &&
					            Mathf.Abs(jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) < 0.15f &&
							    (jointsPos[leftHandIndex].z - jointsPos[leftElbowIndex].z) < -0.15f)
						{
							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
							gestureData.progress = 0.5f;
						}
						break;
				
					case 1:  // gesture phase 2 = complete
						if((timestamp - gestureData.timestamp) < 1.5f)
						{
							bool isInPose = gestureData.joint == rightHandIndex ?
								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
								Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.15f && 
								Mathf.Abs(jointsPos[rightHandIndex].y - gestureData.jointPos.y) < 0.15f && 
								(jointsPos[rightHandIndex].z - gestureData.jointPos.z) > 0.15f :
								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
								Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.15f &&
								Mathf.Abs(jointsPos[leftHandIndex].y - gestureData.jointPos.y) < 0.15f && 
								(jointsPos[leftHandIndex].z - gestureData.jointPos.z) > 0.15f;

							if(isInPose)
							{
								Vector3 jointPos = jointsPos[gestureData.joint];
								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
							}
						}
						else
						{
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;
							// cancel the gesture
            case Gestures.RightHandGrip:
                switch (gestureData.state)
                {
                    case 0:
                        if (jointsTracked[rightHandIndex] && jointsTracked[rightTipIndex] &&
                            (Mathf.Abs(Vector3.Distance(jointsPos[rightHandIndex], jointsPos[rightTipIndex]))) > 0.07f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose =
                                jointsTracked[rightHandIndex] && jointsTracked[rightTipIndex] &&
                                Mathf.Abs(Vector3.Distance(jointsPos[rightHandIndex], jointsPos[rightTipIndex])) < 0.05f;

                            if (Mathf.Abs(Vector3.Distance(gestureData.jointPos, jointsPos[rightHandIndex])) > 0.1f)
                            {
                                SetGestureCancelled(ref gestureData);
                            }
                            else if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            case Gestures.RightHandGrip2:
                switch (gestureData.state)
                {
                    case 0:
                        if (jointsTracked[rightHandIndex] && jointsTracked[rightTipIndex] &&
                            (Mathf.Abs(Vector3.Distance(jointsPos[rightHandIndex], jointsPos[rightTipIndex]))) > 0.072f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose =
                                jointsTracked[rightHandIndex] && jointsTracked[rightTipIndex] &&
                                Mathf.Abs(Vector3.Distance(jointsPos[rightHandIndex], jointsPos[rightTipIndex])) < 0.067f;

                            if(Mathf.Abs(Vector3.Distance(gestureData.jointPos, jointsPos[rightHandIndex])) > 0.1f) {
                                SetGestureCancelled(ref gestureData);
                            }
                            else if(isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;
            case Gestures.LeftHandGrip:
                switch (gestureData.state)
                {

                    case 0:
                        if (jointsTracked[leftHandIndex] && jointsTracked[leftTipIndex] &&
                            (Mathf.Abs(Vector3.Distance(jointsPos[leftHandIndex], jointsPos[leftTipIndex]))) > 0.07f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose =
                                jointsTracked[leftHandIndex] && jointsTracked[leftTipIndex] &&
                                Mathf.Abs(Vector3.Distance(jointsPos[leftHandIndex], jointsPos[leftTipIndex])) < 0.05f;


                            if (Mathf.Abs(Vector3.Distance(gestureData.jointPos, jointsPos[leftHandIndex])) > 0.1f)
                            {
                                SetGestureCancelled(ref gestureData);
                            }
                            else if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;
            case Gestures.LeftHandGrip2:
                switch (gestureData.state)
                {
                    case 0:
                        if (jointsTracked[leftHandIndex] && jointsTracked[leftTipIndex] &&
                            (Mathf.Abs(Vector3.Distance(jointsPos[leftHandIndex], jointsPos[leftTipIndex]))) > 0.072f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose =
                                jointsTracked[leftHandIndex] && jointsTracked[leftTipIndex] &&
                                Mathf.Abs(Vector3.Distance(jointsPos[leftHandIndex], jointsPos[leftTipIndex])) < 0.067f;

                            if(Mathf.Abs(Vector3.Distance(gestureData.jointPos, jointsPos[leftHandIndex])) > 0.1f) {
                                SetGestureCancelled(ref gestureData);
                            }
                            else if(isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

        }
    }
}
