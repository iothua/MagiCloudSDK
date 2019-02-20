using UnityEngine;
//using Windows.Kinect;

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.IO;


/// <summary>
/// Kinect操作
/// </summary>
public class KinectInterop
{
	// constants
    /// <summary>
    /// 常量
    /// </summary>
	public static class Constants
	{
//		public const int BodyCount = 6;
        //关节计数
		public const int JointCount = 25;
        /// <summary>
        /// 相同手势之间的最小时间
        /// </summary>
		public const float MinTimeBetweenSameGestures = 0.0f;
        /// <summary>
        /// 姿势完成时间
        /// </summary>
		public const float PoseCompleteDuration = 1.0f;
        /// <summary>
        /// 点击最大距离
        /// </summary>
		public const float ClickMaxDistance = 0.05f;
        /// <summary>
        /// 点击保持持续时间
        /// </summary>
		public const float ClickStayDuration = 1.5f;
	}

    // Data structures for interfacing C# with the native wrapper
    // 
    /// <summary>
    /// 将C＃与本机包装器进行接口的数据结构
    /// </summary>
    [Flags]
    public enum FrameSource : uint
    {
        /// <summary>
        /// 无
        /// </summary>
		TypeNone = 0x0,
        /// <summary>
        /// 类型颜色
        /// </summary>
        TypeColor = 0x1,
        /// <summary>
        /// 类型接口
        /// </summary>
        TypeInfrared = 0x2,
        /// <summary>
        /// 类型深度
        /// </summary>
        TypeDepth = 0x8,
        /// <summary>
        /// 体型指数
        /// </summary>
        TypeBodyIndex = 0x10,
        /// <summary>
        /// 类型体
        /// </summary>
        TypeBody = 0x20,
        /// <summary>
        /// 类型声音
        /// </summary>
        TypeAudio = 0x40
    }
	
    /// <summary>
    /// 关节类型
    /// </summary>
    public enum JointType : int
    {
        /// <summary>
        /// 髋关节中心
        /// </summary>
		SpineBase = 0,
        /// <summary>
        /// 脊柱中
        /// </summary>
		SpineMid = 1,
        /// <summary>
        /// 颈部
        /// </summary>
        Neck = 2,
        /// <summary>
        /// 头
        /// </summary>
        Head = 3,
        /// <summary>
        /// 左肩膀
        /// </summary>
        ShoulderLeft = 4,
        /// <summary>
        /// 左手肘
        /// </summary>
        ElbowLeft = 5,
        /// <summary>
        /// 左手腕
        /// </summary>
        WristLeft = 6,
        /// <summary>
        /// 左手
        /// </summary>
        HandLeft = 7,
        /// <summary>
        /// 右肩
        /// </summary>
        ShoulderRight = 8,
        /// <summary>
        /// 右手肘
        /// </summary>
        ElbowRight = 9,
        /// <summary>
        /// 右手腕
        /// </summary>
        WristRight = 10,
        /// <summary>
        /// 右手
        /// </summary>
        HandRight = 11,
        /// <summary>
        /// 左髋
        /// </summary>
        HipLeft = 12,
        /// <summary>
        /// 左膝
        /// </summary>
        KneeLeft = 13,
        /// <summary>
        /// 左脚踝
        /// </summary>
        AnkleLeft = 14,
        /// <summary>
        /// 左脚
        /// </summary>
        FootLeft = 15,
        /// <summary>
        /// 右髋
        /// </summary>
        HipRight = 16,
        /// <summary>
        /// 右膝盖
        /// </summary>
        KneeRight = 17,
        /// <summary>
        /// 右脚踝
        /// </summary>
        AnkleRight = 18,
        /// <summary>
        /// 右脚
        /// </summary>
        FootRight = 19,
        /// <summary>
        /// 肩中心
        /// </summary>
        SpineShoulder = 20,
        /// <summary>
        /// 左手指尖
        /// </summary>
        HandTipLeft = 21,
        /// <summary>
        /// 左拇指
        /// </summary>
        ThumbLeft = 22,
        /// <summary>
        /// 右手指尖
        /// </summary>
        HandTipRight = 23,
        /// <summary>
        /// 右拇指
        /// </summary>
        ThumbRight = 24
		//Count = 25
    }

    public static readonly Vector3[] JointBaseDir =
    {
        Vector3.zero,
        Vector3.up,
        Vector3.up,
        Vector3.up,
        Vector3.left,
        Vector3.left,
        Vector3.left,
        Vector3.left,
        Vector3.right,
        Vector3.right,
        Vector3.right,
        Vector3.right,
        Vector3.down,
        Vector3.down,
        Vector3.down,
        Vector3.forward,
        Vector3.down,
        Vector3.down,
        Vector3.down,
        Vector3.forward,
        Vector3.up,
        Vector3.left,
        Vector3.forward,
        Vector3.right,
        Vector3.forward
    };

    /// <summary>
    /// 跟踪状态
    /// </summary>
    public enum TrackingState
    {
        /// <summary>
        /// 未跟踪
        /// </summary>
        NotTracked = 0,
        /// <summary>
        /// 推测
        /// </summary>
        Inferred = 1,
        /// <summary>
        /// 跟踪
        /// </summary>
        Tracked = 2
    }

    /// <summary>
    /// 手状态
    /// </summary>
	public enum HandState
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 未跟踪
        /// </summary>
        NotTracked = 1,
        /// <summary>
        /// 打开
        /// </summary>
        Open = 2,
        /// <summary>
        /// 关闭
        /// </summary>
        Closed = 3,
        /// <summary>
        /// 拉拢
        /// </summary>
        Lasso = 4
    }
    /// <summary>
    /// 跟踪置信度
    /// </summary>
    public enum TrackingConfidence
    {
        /// <summary>
        /// 低
        /// </summary>
        Low = 0,
        /// <summary>
        /// 高
        /// </summary>
        High = 1
    }

//    [Flags]
//    public enum ClippedEdges
//    {
//        None = 0,
//        Right = 1,
//        Left = 2,
//        Top = 4,
//        Bottom = 8
//    }


    /// <summary>
    /// 传感器数据
    /// </summary>
	public class SensorData
	{
        /// <summary>
        /// 深度传感器接口
        /// </summary>
		public DepthSensorInterface sensorInterface;

        /// <summary>
        /// 人数
        /// </summary>
		public int bodyCount;
        /// <summary>
        /// 关节数
        /// </summary>
		public int jointCount;
        /// <summary>
        /// 彩色图像宽度
        /// </summary>
		public int colorImageWidth;
        /// <summary>
        /// 彩色图像高度
        /// </summary>
		public int colorImageHeight;

        /// <summary>
        /// 彩色图像
        /// </summary>
		public byte[] colorImage;
        /// <summary>
        /// 最后的颜色帧时间
        /// </summary>
		public long lastColorFrameTime = 0;

        /// <summary>
        /// 深度图像宽度
        /// </summary>
		public int depthImageWidth;
        /// <summary>
        /// 深度图像高度
        /// </summary>
		public int depthImageHeight;

        /// <summary>
        /// 深度图像
        /// </summary>
        public ushort[] depthImage;
        /// <summary>
        /// 最后的深度帧时间
        /// </summary>
		public long lastDepthFrameTime = 0;

        /// <summary>
        /// 红外图像
        /// </summary>
		public ushort[] infraredImage;
        /// <summary>
        /// 最后红外帧时间
        /// </summary>
		public long lastInfraredFrameTime = 0;
        /// <summary>
        /// 身体索引图像
        /// </summary>
		public byte[] bodyIndexImage;
        /// <summary>
        /// 最后身体指数帧时间
        /// </summary>
		public long lastBodyIndexFrameTime = 0;
	}

    /// <summary>
    /// 平滑参数
    /// </summary>
	public struct SmoothParameters
	{
        /// <summary>
        /// 平滑
        /// </summary>
		public float smoothing;
        /// <summary>
        /// 更正
        /// </summary>
		public float correction;
        /// <summary>
        /// 预测
        /// </summary>
		public float prediction;
        /// <summary>
        /// 抖动半径
        /// </summary>
		public float jitterRadius;
        /// <summary>
        /// 最大偏差半径
        /// </summary>
		public float maxDeviationRadius;
	}
	
    /// <summary>
    /// 关节数据
    /// </summary>
	public struct JointData
    {
        // parameters filled in by the sensor interface
        /// <summary>
        /// 由传感器接口填充的参数
        /// </summary>
        public JointType jointType;
        /// <summary>
        /// 跟踪状态
        /// </summary>
    	public TrackingState trackingState;
        /// <summary>
        /// kinect位置
        /// </summary>
    	public Vector3 kinectPos;
        /// <summary>
        /// 位置
        /// </summary>
    	public Vector3 position;
        /// <summary>
        /// 方向（已弃用）
        /// </summary>
		public Quaternion orientation;  // deprecated

		// KM calculated parameters
        /// <summary>
        /// 方向
        /// </summary>
		public Vector3 direction;
        /// <summary>
        /// 正常旋转
        /// </summary>
		public Quaternion normalRotation;
        /// <summary>
        /// 镜像旋转
        /// </summary>
		public Quaternion mirroredRotation;
    }

    /// <summary>
    /// 身体数据
    /// </summary>
    public struct BodyData
    {
        // parameters filled in by the sensor interface
        /// <summary>
        /// 参数由传感器接口填充
        /// </summary>
        public Int64 liTrackingID;
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// 方向（已弃用）
        /// </summary>
		public Quaternion orientation;  // deprecated

        /// <summary>
        /// 关节数据
        /// </summary>
		public JointData[] joint;

		// KM calculated parameters
        /// <summary>
        /// 正常旋转
        /// </summary>
		public Quaternion normalRotation;
        /// <summary>
        /// 镜像旋转
        /// </summary>
		public Quaternion mirroredRotation;
        /// <summary>
        /// 臀部方向
        /// </summary>
        public Vector3 hipsDirection;
        /// <summary>
        /// 肩膀方向
        /// </summary>
		public Vector3 shouldersDirection;
        /// <summary>
        /// 身体转角
        /// </summary>
		public float bodyTurnAngle;
        /// <summary>
        /// 左拇指方向
        /// </summary>
		public Vector3 leftThumbDirection;
        /// <summary>
        /// 左手方向
        /// </summary>
		public Vector3 leftHandDirection;
        /// <summary>
        /// 左拇指前进
        /// </summary>
		public Vector3 leftThumbForward;
        /// <summary>
        /// 左拇指指角
        /// </summary>
		public float leftThumbAngle;

        /// <summary>
        /// 右拇指方向
        /// </summary>
		public Vector3 rightThumbDirection;
        /// <summary>
        /// 右手方向
        /// </summary>
		public Vector3 rightHandDirection;
        /// <summary>
        /// 右拇指前进
        /// </summary>
		public Vector3 rightThumbForward;
        /// <summary>
        /// 右拇指指角
        /// </summary>
		public float rightThumbAngle;

        //public Vector3 leftLegDirection;
        //public Vector3 leftFootDirection;
        //public Vector3 rightLegDirection;
        //public Vector3 rightFootDirection;

        /// <summary>
        /// 左手状态
        /// </summary>
        public HandState leftHandState;
        /// <summary>
        /// 左手跟踪信息
        /// </summary>
		public TrackingConfidence leftHandConfidence;
        /// <summary>
        /// 右手状态
        /// </summary>
		public HandState rightHandState;
        /// <summary>
        /// 右手跟踪信息
        /// </summary>
		public TrackingConfidence rightHandConfidence;

        /// <summary>
        /// 裁剪边缘
        /// </summary>
        public uint dwClippedEdges;
        /// <summary>
        /// 被跟踪
        /// </summary>
        public short bIsTracked;
        /// <summary>
        /// 受限制
        /// </summary>
		public short bIsRestricted;
    }
	
    /// <summary>
    /// 身体帧数据
    /// </summary>
    public struct BodyFrameData
    {
        /// <summary>
        /// 相对时间
        /// </summary>
        public Int64 liRelativeTime;
        /// <summary>
        /// 身体数据 从C++传送数据
        /// </summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
        public BodyData[] bodyData;
        /// <summary>
        /// 地板裁面
        /// </summary>
        public UnityEngine.Vector4 floorClipPlane;
		
        /// <summary>
        /// 实例化该类
        /// </summary>
        /// <param name="bodyCount">身体数</param>
        /// <param name="jointCount">关节数</param>
		public BodyFrameData(int bodyCount, int jointCount)
		{
			liRelativeTime = 0;
			floorClipPlane = UnityEngine.Vector4.zero;

			bodyData = new BodyData[bodyCount];
			
			for(int i = 0; i < bodyCount; i++)
			{
				bodyData[i].joint = new JointData[jointCount];
			}
		}
    }


    // initializes the available sensor interfaces
    /// <summary>
    /// 初始化可用的传感器接口
    /// </summary>
    /// <param name="bNeedRestart">需要重新启动</param>
    /// <returns></returns>
    public static List<DepthSensorInterface> InitSensorInterfaces(ref bool bNeedRestart)
	{
		List<DepthSensorInterface> listInterfaces = new List<DepthSensorInterface>();

        //通过反射获取到接口的类型
		var typeInterface = typeof(DepthSensorInterface);
		Type[] typesAvailable = typeInterface.Assembly.GetTypes();

		foreach(Type type in typesAvailable)
		{
			if(typeInterface.IsAssignableFrom(type) && type != typeInterface)
			{
				DepthSensorInterface sensorInt = null;

				try 
				{
                    //创建一个接口对象
					sensorInt = (DepthSensorInterface)Activator.CreateInstance(type);

					bool bIntNeedRestart = false;
					if(sensorInt.InitSensorInterface(ref bIntNeedRestart))
					{
						bNeedRestart |= bIntNeedRestart;//进行或运算
					}
					else
					{
						sensorInt.FreeSensorInterface();    //释放资源
						sensorInt = null;
						continue;
					}

                    //返回传感器数量
					if(sensorInt.GetSensorsCount() <= 0)
					{
						sensorInt.FreeSensorInterface();    //释放资源
						sensorInt = null;
					}
				} 
				catch (Exception) 
				{
					if(sensorInt != null)
					{
						try 
						{
							sensorInt.FreeSensorInterface();    //释放资源
						}
						catch (Exception) 
						{
							// do nothing
						}
						finally
						{
							sensorInt = null;
						}
					}
				}
				if(sensorInt != null)
				{
					listInterfaces.Add(sensorInt);  //添加到集合中
				}
			}
		}

		return listInterfaces;  //返回集合
	}

    // opens the default sensor and needed readers
    /// <summary>
    /// 打开默认的传感器和需要的读卡器
    /// </summary>
    /// <param name="listInterfaces">接口集合</param>
    /// <param name="dwFlags">绘制标志</param>
    /// <param name="sensorAngle">传感器角度</param>
    /// <param name="bUseMultiSource">用户多源</param>
    /// <returns></returns>
    public static SensorData OpenDefaultSensor(List<DepthSensorInterface> listInterfaces, 
	                                           FrameSource dwFlags, float sensorAngle, bool bUseMultiSource)
	{
		SensorData sensorData = null;
		if(listInterfaces == null)
			return sensorData;

        //遍历传感器接口
		foreach(DepthSensorInterface sensorInt in listInterfaces)
		{
			try 
			{
                if (sensorData == null)
				{
                    //更新传感器数据
					sensorData = sensorInt.OpenDefaultSensor(dwFlags, sensorAngle, bUseMultiSource);

					if(sensorData != null)
					{
						sensorData.sensorInterface = sensorInt;
						//Debug.Log("Interface used: " + sensorInt.GetType().Name);
					}
				}
				else
				{
					sensorInt.FreeSensorInterface();    //释放资源
				}
			} 
			catch (Exception ex) 
			{
				//Debug.LogError("Initialization of sensor failed.");
				Debug.LogError(ex.ToString());

				try 
				{
					sensorInt.FreeSensorInterface();    //释放资源
				} 
				catch (Exception) 
				{
					// do nothing
				}
			}
		}

		return sensorData;
	}

    // closes opened readers and closes the sensor
    /// <summary>
    /// 关闭打开的读取器并关闭传感器
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    public static void CloseSensor(SensorData sensorData)
	{
		if(sensorData != null && sensorData.sensorInterface != null)
		{
            //关闭传感器并释放资源
			sensorData.sensorInterface.CloseSensor(sensorData);
		}
	}

    // invoked periodically to update sensor data, if needed
    /// <summary>
    /// 如果需要的话定期调用，以更新传感器数据
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns>更新成功返回true，否则返回false</returns>
    public static bool UpdateSensorData(SensorData sensorData)
	{
		bool bResult = false;

		if(sensorData.sensorInterface != null)
		{
			bResult = sensorData.sensorInterface.UpdateSensorData(sensorData);
		}

		return bResult;
	}

    //
    /// <summary>
    /// 返回给定连接的镜像节点
    /// </summary>
    /// <param name="joint">关节类型</param>
    /// <returns></returns>
    public static JointType GetMirrorJoint(JointType joint)
	{
		switch(joint)
		{
            
			case JointType.ShoulderLeft:        //左关节
                return JointType.ShoulderRight; //返回右关节
	        case JointType.ElbowLeft:           //左手肘
				return JointType.ElbowRight;    //返回右手肘
	        case JointType.WristLeft:           //左手腕
				return JointType.WristRight;    //返回右手腕
	        case JointType.HandLeft:            //左手
				return JointType.HandRight;     //返回右手
					
	        case JointType.ShoulderRight:       //右肩
				return JointType.ShoulderLeft;  //返回左肩
	        case JointType.ElbowRight:          //右手肘
				return JointType.ElbowLeft;     //返回左手肘
	        case JointType.WristRight:          //右手腕
				return JointType.WristLeft;     //返回左手腕
	        case JointType.HandRight:           //右手
				return JointType.HandLeft;      //左手
					
	        case JointType.HipLeft:             //左臀部
				return JointType.HipRight;      //右臀部
	        case JointType.KneeLeft:            //左膝
				return JointType.KneeRight;     //返回右膝
	        case JointType.AnkleLeft:           //左脚裸
				return JointType.AnkleRight;    //返回右脚裸
	        case JointType.FootLeft:            //左脚
				return JointType.FootRight;     //返回右脚
					
	        case JointType.HipRight:            //右臀部
				return JointType.HipLeft;       //返回左臀部
	        case JointType.KneeRight:           //右膝盖
				return JointType.KneeLeft;      //左膝盖
	        case JointType.AnkleRight:          //右脚裸
				return JointType.AnkleLeft;     //左脚裸
	        case JointType.FootRight:           //右脚
				return JointType.FootLeft;      //左脚
					
	        case JointType.HandTipLeft:         //左手指尖
				return JointType.HandTipRight;  //右手指尖
	        case JointType.ThumbLeft:           //左拇指
				return JointType.ThumbRight;    //右拇指
			
	        case JointType.HandTipRight:        //右手指尖
				return JointType.HandTipLeft;   //左手指尖
	        case JointType.ThumbRight:          //右拇指
				return JointType.ThumbLeft;     //左拇指
		}
	
		return joint;
	}

    // gets new multi source frame
    /// <summary>
    /// 获取新的多源帧
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns></returns>
    public static bool GetMultiSourceFrame(SensorData sensorData)
	{
		bool bResult = false;

		if(sensorData.sensorInterface != null)
		{
			bResult = sensorData.sensorInterface.GetMultiSourceFrame(sensorData);
		}

		return bResult;
	}

    // frees last multi source frame
    /// <summary>
    /// 释放最后的多源帧
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    public static void FreeMultiSourceFrame(SensorData sensorData)
	{
		if(sensorData.sensorInterface != null)
		{
			sensorData.sensorInterface.FreeMultiSourceFrame(sensorData);
		}
	}

    // Polls for new skeleton data
    /// <summary>
    /// 轮询新的骨架数据
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="bodyFrame">身体帧</param>
    /// <param name="kinectToWorld">Kinect世界矩阵</param>
    /// <returns></returns>
    public static bool PollBodyFrame(SensorData sensorData, ref BodyFrameData bodyFrame, ref Matrix4x4 kinectToWorld)
	{
		bool bNewFrame = false;

		if(sensorData.sensorInterface != null)
		{
            //获取新的身体帧
			bNewFrame = sensorData.sensorInterface.PollBodyFrame(sensorData, ref bodyFrame, ref kinectToWorld);

			if(bNewFrame)
			{
				for(int i = 0; i < sensorData.bodyCount; i++)
				{
					if(bodyFrame.bodyData[i].bIsTracked != 0)
					{
						// calculate joint directions
                        //计算关节方向
						for(int j = 0; j < sensorData.jointCount; j++)
						{
							if(j == 0)
							{
								bodyFrame.bodyData[i].joint[j].direction = Vector3.zero;
							}
							else
							{
                                //获取关节的父节点
								int jParent = (int)sensorData.sensorInterface.GetParentJoint(bodyFrame.bodyData[i].joint[j].jointType);
								
                                //当前关节和父关节都不是未跟踪时，则进行处理
								if(bodyFrame.bodyData[i].joint[j].trackingState != TrackingState.NotTracked && 
								   bodyFrame.bodyData[i].joint[jParent].trackingState != TrackingState.NotTracked)
								{
                                    //设置身体数据关节方向
									bodyFrame.bodyData[i].joint[j].direction = 
										bodyFrame.bodyData[i].joint[j].position - bodyFrame.bodyData[i].joint[jParent].position;
								}
							}
						}
					}

				}
			}
		}
		
		return bNewFrame;
	}

    // Polls for new color frame data
    /// <summary>
    /// 轮询新的彩色帧数据
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns></returns>
    public static bool PollColorFrame(SensorData sensorData)
	{
		bool bNewFrame = false;

		if(sensorData.sensorInterface != null)
		{
			bNewFrame = sensorData.sensorInterface.PollColorFrame(sensorData);
		}

		return bNewFrame;
	}

    // Polls for new depth frame data
    /// <summary>
    /// 轮询新的深度帧数据
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns></returns>
    public static bool PollDepthFrame(SensorData sensorData)
	{
		bool bNewFrame = false;

		if(sensorData.sensorInterface != null)
		{
			bNewFrame = sensorData.sensorInterface.PollDepthFrame(sensorData);
		}

		return bNewFrame;
	}

    // Polls for new infrared frame data
    /// <summary>
    /// 轮询新的红外帧数据
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns></returns>
    public static bool PollInfraredFrame(SensorData sensorData)
	{
		bool bNewFrame = false;

		if(sensorData.sensorInterface != null)
		{
			bNewFrame = sensorData.sensorInterface.PollInfraredFrame(sensorData);
		}

		return bNewFrame;
	}

    // returns depth frame coordinates for the given 3d Kinect-space point
    /// <summary>
    /// 返回深度帧坐标为给定的3d Kinect-space点
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="kinectPos">kinect坐标</param>
    /// <returns></returns>
    public static Vector2 MapSpacePointToDepthCoords(SensorData sensorData, Vector3 kinectPos)
	{
		Vector2 vPoint = Vector2.zero;

		if(sensorData.sensorInterface != null)
		{
			vPoint = sensorData.sensorInterface.MapSpacePointToDepthCoords(sensorData, kinectPos);
		}

		return vPoint;
	}

    // returns 3d coordinates for the given depth-map point
    /// <summary>
    /// 返回给定深度图点的三维坐标
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="depthPos">深度坐标</param>
    /// <param name="depthVal">深度值</param>
    /// <returns></returns>
    public static Vector3 MapDepthPointToSpaceCoords(SensorData sensorData, Vector2 depthPos, ushort depthVal)
	{
		Vector3 vPoint = Vector3.zero;

		if(sensorData.sensorInterface != null)
		{
			vPoint = sensorData.sensorInterface.MapDepthPointToSpaceCoords(sensorData, depthPos, depthVal);
		}

		return vPoint;
	}

    // returns color-map coordinates for the given depth point
    /// <summary>
    /// 返回给定深度点的色图坐标
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="depthPos">深度坐标</param>
    /// <param name="depthVal">深度值</param>
    /// <returns></returns>
    public static Vector2 MapDepthPointToColorCoords(SensorData sensorData, Vector2 depthPos, ushort depthVal)
	{
		Vector2 vPoint = Vector2.zero;

		if(sensorData.sensorInterface != null)
		{
			vPoint = sensorData.sensorInterface.MapDepthPointToColorCoords(sensorData, depthPos, depthVal);
		}

		return vPoint;
	}

    // estimates color-map coordinates for the current depth frame
    /// <summary>
    /// 估计当前深度帧的颜色映射坐标
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="vColorCoords">颜色系</param>
    /// <returns></returns>
    public static bool MapDepthFrameToColorCoords(SensorData sensorData, ref Vector2[] vColorCoords)
	{
		bool bResult = false;

		if(sensorData.sensorInterface != null)
		{
			bResult = sensorData.sensorInterface.MapDepthFrameToColorCoords(sensorData, ref vColorCoords);
		}

		return bResult;
	}

    // Copy a resource file to the target
    /// <summary>
    /// 将资源文件复制到目标
    /// </summary>
    /// <param name="targetFilePath">目标路径</param>
    /// <param name="resFileName">文件名称</param>
    /// <param name="bOneCopied">一份拷贝</param>
    /// <param name="bAllCopied">全部拷贝</param>
    /// <returns></returns>
    public static bool CopyResourceFile(string targetFilePath, string resFileName, ref bool bOneCopied, ref bool bAllCopied)
	{
        //加载资源
		TextAsset textRes = Resources.Load(resFileName, typeof(TextAsset)) as TextAsset;
		if(textRes == null)
		{
			bOneCopied = false;
			bAllCopied = false;
			
			return false;
		}
		
		FileInfo targetFile = new FileInfo(targetFilePath);
        //判断文件是否存在
		if(!targetFile.Directory.Exists)
		{
			targetFile.Directory.Create();
		}
		
		if(!targetFile.Exists || targetFile.Length !=  textRes.bytes.Length)
		{
			if(textRes != null)
			{
				using (FileStream fileStream = new FileStream (targetFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					fileStream.Write(textRes.bytes, 0, textRes.bytes.Length);
				}
				
				bool bFileCopied = File.Exists(targetFilePath);
				
				bOneCopied = bOneCopied || bFileCopied;
				bAllCopied = bAllCopied && bFileCopied;
				
				return bFileCopied;
			}
		}
		
		return false;
	}
	
}