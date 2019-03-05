using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

/// <summary>
/// Kinect1接口
/// </summary>
public class Kinect1Interface : DepthSensorInterface 
{
    /// <summary>
    /// 常量
    /// </summary>
	public static class Constants
	{
        /// <summary>
        /// 骨骼计数
        /// </summary>
		public const int NuiSkeletonCount = 6;

		public const NuiImageResolution ColorImageResolution = NuiImageResolution.resolution640x480;
		public const NuiImageResolution DepthImageResolution = NuiImageResolution.resolution640x480;
		
		public const bool IsNearMode = false;
	}

    // Structs and constants for interfacing C# with the Kinect.dll 
    //使用kinect . dll与c#接口的结构和常量

    /// <summary>
    /// 用于将C＃与Kinect.dll进行连接的结构和常量
    /// </summary>
    [Flags]
	public enum NuiInitializeFlags : uint
	{
        /// <summary>
        /// 无
        /// </summary>
		UsesNone = 0,
        /// <summary>
        /// 使用声音
        /// </summary>
		UsesAudio = 0x10000000,
        /// <summary>
        /// 使用深度和人物数
        /// </summary>
		UsesDepthAndPlayerIndex = 0x00000001,
        /// <summary>
        /// 使用颜色
        /// </summary>
		UsesColor = 0x00000002,
        /// <summary>
        /// 使用骨架
        /// </summary>
		UsesSkeleton = 0x00000008,
        /// <summary>
        /// 使用深度
        /// </summary>
		UsesDepth = 0x00000020,
        /// <summary>
        /// 使用高品质的颜色
        /// </summary>
		UsesHighQualityColor = 0x00000040
	}
	
    /// <summary>
    /// 错误代码
    /// </summary>
	public enum NuiErrorCodes : uint
	{
        /// <summary>
        /// 帧无数据
        /// </summary>
		FrameNoData = 0x83010001,
        /// <summary>
        /// 数据流未启用
        /// </summary>
		StreamNotEnabled = 0x83010002,
        /// <summary>
        /// 图像流在使用中
        /// </summary>
		ImageStreamInUse = 0x83010003,
        /// <summary>
        /// 帧限制超出
        /// </summary>
		FrameLimitExceeded = 0x83010004,
        /// <summary>
        /// 功能未初始化
        /// </summary>
		FeatureNotInitialized = 0x83010005,
        /// <summary>
        /// 设备不正版
        /// </summary>
		DeviceNotGenuine = 0x83010006,
        /// <summary>
        /// 带宽不足
        /// </summary>
		InsufficientBandwidth = 0x83010007,
        /// <summary>
        /// 设备不支持
        /// </summary>
		DeviceNotSupported = 0x83010008,
        /// <summary>
        /// 正在使用的设备
        /// </summary>
		DeviceInUse = 0x83010009,
        /// <summary>
        /// 找不到数据库
        /// </summary>
        DatabaseNotFound = 0x8301000D,
        /// <summary>
        /// 数据库版本不匹配
        /// </summary>
		DatabaseVersionMismatch = 0x8301000E,
        /// <summary>
        /// 硬件功能不可用
        /// </summary>
		HardwareFeatureUnavailable = 0x8301000F,
        /// <summary>
        /// 设备未连接
        /// </summary>
        DeviceNotConnected = 0x83010014,
        /// <summary>
        /// 设备未就绪
        /// </summary>
		DeviceNotReady = 0x83010015,
        /// <summary>
        /// 骨骼引擎忙
        /// </summary>
		SkeletalEngineBusy = 0x830100AA,
        /// <summary>
        /// 设备未上电
        /// </summary>
		DeviceNotPowered = 0x8301027F,
	}
	
    /// <summary>
    /// 骨骼位置索引
    /// </summary>
	public enum NuiSkeletonPositionIndex : int
	{
        /// <summary>
        /// 髋关节中心
        /// </summary>
		HipCenter = 0,
        /// <summary>
        /// 脊柱中
        /// </summary>
		Spine,
        /// <summary>
        /// 颈部
        /// </summary>
		ShoulderCenter,
        /// <summary>
        /// 头
        /// </summary>
		Head,
        /// <summary>
        /// 左肩膀
        /// </summary>
        ShoulderLeft,
        /// <summary>
        /// 左手肘
        /// </summary>
		ElbowLeft,
        /// <summary>
        /// 左手腕
        /// </summary>
		WristLeft,
        /// <summary>
        /// 左手
        /// </summary>
		HandLeft,
        /// <summary>
        /// 右肩
        /// </summary>
		ShoulderRight,
        /// <summary>
        /// 右手肘
        /// </summary>
		ElbowRight,
        /// <summary>
        /// 右手腕
        /// </summary>
		WristRight,
        /// <summary>
        /// 右手
        /// </summary>
		HandRight,
        /// <summary>
        /// 左髋
        /// </summary>
		HipLeft,
        /// <summary>
        /// 左膝
        /// </summary>
		KneeLeft,
        /// <summary>
        /// 左脚踝
        /// </summary>
		AnkleLeft,
        /// <summary>
        /// 左脚
        /// </summary>
		FootLeft,
        /// <summary>
        /// 右髋
        /// </summary>
		HipRight,
        /// <summary>
        /// 右膝盖
        /// </summary>
        KneeRight,
        /// <summary>
        /// 右脚踝
        /// </summary>
        AnkleRight,
        /// <summary>
        /// 右脚
        /// </summary>
		FootRight,
		Count
	}

    /// <summary>
    /// 骨骼位置跟踪状态
    /// </summary>
    public enum NuiSkeletonPositionTrackingState
	{
        /// <summary>
        /// 未跟踪
        /// </summary>
		NotTracked = 0,
        /// <summary>
        /// 推测
        /// </summary>
		Inferred,
        /// <summary>
        /// 跟踪
        /// </summary>
		Tracked
    }
    /// <summary>
    /// 骨架追踪状态
    /// </summary>
    public enum NuiSkeletonTrackingState
	{
        /// <summary>
        /// 未跟踪
        /// </summary>
		NotTracked = 0,
        /// <summary>
        /// 仅限位置
        /// </summary>
		PositionOnly,
        /// <summary>
        /// 骨骼追踪
        /// </summary>
		SkeletonTracked
	}
	
    /// <summary>
    /// 图像类型
    /// </summary>
	public enum NuiImageType
	{
        /// <summary>
        /// 深度和玩家索引 ushort
        /// </summary>
		DepthAndPlayerIndex = 0,    // USHORT
        /// <summary>
        /// 颜色 RGB32 data
        /// </summary>
        Color,                      // RGB32 data
        /// <summary>
        /// YUY2从相机h / w流，但在用户获取之前转换为RGB32。
        /// </summary>
        ColorYUV,                   // YUY2 stream from camera h/w, but converted to RGB32 before user getting it.
        /// <summary>
        /// YUY2流从相机h / w。
        /// </summary>
        ColorRawYUV,                // YUY2 stream from camera h/w.
        /// <summary>
        /// 深度 ushort
        /// </summary>
        Depth						// USHORT
	}
	
    /// <summary>
    /// 图像分辨率
    /// </summary>
	public enum NuiImageResolution
	{
        /// <summary>
        /// 分辨率无效
        /// </summary>
		resolutionInvalid = -1,
        /// <summary>
        /// 分辨率80x60
        /// </summary>
		resolution80x60 = 0,
        /// <summary>
        /// 分辨率320x240
        /// </summary>
		resolution320x240 = 1,
        /// <summary>
        /// 分辨率640x480
        /// </summary>
		resolution640x480 = 2,
        /// <summary>
        /// 分辨率1280x960
        /// </summary>
		resolution1280x960 = 3     // for hires color only
	}

    /// <summary>
    /// 图像流标志
    /// </summary>
	public enum NuiImageStreamFlags
	{
        /// <summary>
        /// 无
        /// </summary>
		None = 0x00000000,
        /// <summary>
        /// 禁止无帧数据
        /// </summary>
		SupressNoFrameData = 0x0001000,
        /// <summary>
        /// 启用近模式
        /// </summary>
		EnableNearMode = 0x00020000,
        /// <summary>
        /// 非常远也非零
        /// </summary>
		TooFarIsNonZero = 0x0004000
	}
	
    /// <summary>
    /// 骨骼数据
    /// </summary>
	public struct NuiSkeletonData
	{
        /// <summary>
        /// 骨骼追踪状态对象
        /// </summary>
		public NuiSkeletonTrackingState eTrackingState;
        /// <summary>
        /// 跟踪ID
        /// </summary>
		public uint dwTrackingID;
        /// <summary>
        /// 登记索引未使用
        /// </summary>
		public uint dwEnrollmentIndex_NotUsed;
        /// <summary>
        /// 用户索引
        /// </summary>
		public uint dwUserIndex;
        /// <summary>
        /// 位置
        /// </summary>
		public Vector4 Position;
        /// <summary>
        /// 骨骼位置
        /// </summary>
		[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.Struct)]
		public Vector4[] SkeletonPositions;
        /// <summary>
        /// 骨骼位置跟踪状态
        /// </summary>
		[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.Struct)]
		public NuiSkeletonPositionTrackingState[] eSkeletonPositionTrackingState;
        /// <summary>
        /// 质量标志
        /// </summary>
        public uint dwQualityFlags;
	}
	
    /// <summary>
    /// 骨骼帧
    /// </summary>
	public struct NuiSkeletonFrame
	{
        /// <summary>
        /// 时间戳
        /// </summary>
		public long liTimeStamp;
        /// <summary>
        /// 帧数
        /// </summary>
		public uint dwFrameNumber;
        /// <summary>
        /// 标志
        /// </summary>
		public uint dwFlags;
        /// <summary>
        /// 远裁剪面的距离
        /// </summary>
		public Vector4 vFloorClipPlane;
        /// <summary>
        /// 正常重力
        /// </summary>
		public Vector4 vNormalToGravity;
        /// <summary>
        /// 骨骼数据
        /// </summary>
		[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
		public NuiSkeletonData[] SkeletonData;
	}
	
    /// <summary>
    /// 转换平滑参数
    /// </summary>
	public struct NuiTransformSmoothParameters
	{
        /// <summary>
        /// 平滑参数
        /// </summary>
		public float fSmoothing;
        /// <summary>
        /// 更正
        /// </summary>
		public float fCorrection;
        /// <summary>
        /// 预测
        /// </summary>
		public float fPrediction;
        /// <summary>
        /// 抖动半径
        /// </summary>
		public float fJitterRadius;
        /// <summary>
        /// 最大偏差半径
        /// </summary>
		public float fMaxDeviationRadius;
	}
	
    /// <summary>
    /// 骨骼旋转
    /// </summary>
	public struct NuiSkeletonBoneRotation
	{
        /// <summary>
        /// 旋转矩阵
        /// </summary>
		public Matrix4x4 rotationMatrix;
        /// <summary>
        /// 旋转四元数
        /// </summary>
		public Quaternion rotationQuaternion;
	}
	
    /// <summary>
    /// 骨骼方向
    /// </summary>
	public struct NuiSkeletonBoneOrientation
	{
        /// <summary>
        /// 骨骼关节结束索引
        /// </summary>
		public NuiSkeletonPositionIndex endJoint;
        /// <summary>
        /// 骨骼关节开始索引
        /// </summary>
		public NuiSkeletonPositionIndex startJoint;
        /// <summary>
        /// 分层旋转
        /// </summary>
		public NuiSkeletonBoneRotation hierarchicalRotation;
        /// <summary>
        /// 绝对旋转
        /// </summary>
		public NuiSkeletonBoneRotation absoluteRotation;
	}

    /// <summary>
    /// 图像视图区域
    /// </summary>
    public struct NuiImageViewArea
	{
        /// <summary>
        /// 数字变焦
        /// </summary>
		public int eDigitalZoom;
        /// <summary>
        /// 中心X
        /// </summary>
		public int lCenterX;
        /// <summary>
        /// 中心Y
        /// </summary>
		public int lCenterY;
	}

    /// <summary>
    /// 图像缓冲区
    /// </summary>
    public class NuiImageBuffer
	{
        /// <summary>
        /// 宽度
        /// </summary>
		public int m_Width;
        /// <summary>
        /// 高度
        /// </summary>
		public int m_Height;
        /// <summary>
        /// 每像素字节数
        /// </summary>
		public int m_BytesPerPixel;
        /// <summary>
        /// 缓冲区
        /// </summary>
		public IntPtr m_pBuffer;
	}
	
    /// <summary>
    /// 图像帧
    /// </summary>
	public struct NuiImageFrame
	{
        /// <summary>
        /// 时间戳
        /// </summary>
		public Int64 liTimeStamp;
        /// <summary>
        /// 帧数
        /// </summary>
		public uint dwFrameNumber;
        /// <summary>
        /// 图像类型
        /// </summary>
		public NuiImageType eImageType;
        /// <summary>
        /// 图像分辨率
        /// </summary>
		public NuiImageResolution eResolution;
		//[MarshalAsAttribute(UnmanagedType.Interface)]
        /// <summary>
        /// Texture帧
        /// </summary>
		public IntPtr pFrameTexture;
        /// <summary>
        /// 未使用帧标志
        /// </summary>
		public uint dwFrameFlags_NotUsed;
        /// <summary>
        /// 未使用视图区域
        /// </summary>
		public NuiImageViewArea ViewArea_NotUsed;
	}
	
    /// <summary>
    /// 锁定矩阵
    /// </summary>
	public struct NuiLockedRect
	{
        /// <summary>
        /// Pitch
        /// </summary>
		public int pitch;
        /// <summary>
        /// Size
        /// </summary>
		public int size;
        /// <summary>
        /// Bits
        /// </summary>
		//[MarshalAsAttribute(UnmanagedType.U8)] 
		public IntPtr pBits; 
		
	}

    /// <summary>
    /// 手指状态
    /// </summary>
	public enum NuiHandpointerState : uint
	{
        /// <summary>
        /// 无
        /// </summary>
		None = 0,
        /// <summary>
        /// 跟踪
        /// </summary>
		Tracked = 1,
        /// <summary>
        /// 激活
        /// </summary>
		Active = 2,
        /// <summary>
        /// 互动
        /// </summary>
		Interactive = 4,
        /// <summary>
        /// 按下
        /// </summary>
		Pressed = 8,
        /// <summary>
        /// 初始
        /// </summary>
		PrimaryForUser = 0x10
	}
	
    /// <summary>
    /// 手指事件类型
    /// </summary>
	public enum InteractionHandEventType : int
	{
        /// <summary>
        /// 无
        /// </summary>
		None = 0,

        /// <summary>
        /// 握
        /// </summary>
		Grip = 1,
        /// <summary>
        /// 松
        /// </summary>
		Release = 2
	}
	

	// private interface data
    /// <summary>
    /// 帧源
    /// </summary>
	private KinectInterop.FrameSource sourceFlags;

	//private IntPtr colorStreamHandle;
	//private IntPtr depthStreamHandle;
    /// <summary>
    /// 骨骼帧
    /// </summary>
	private NuiSkeletonFrame skeletonFrame;
    /// <summary>
    /// 平滑参数
    /// </summary>
	private NuiTransformSmoothParameters smoothParameters;

	private NuiImageViewArea pcViewArea = new NuiImageViewArea 
	{
		eDigitalZoom = 0,
		lCenterX = 0,
		lCenterY = 0
	};

    // exported wrapper functions
    /// <summary>
    /// 获取传感器计数
    /// </summary>
    /// <param name="pCount">数</param>
    /// <returns></returns>
    [DllImport(@"Kinect10.dll")]
	private static extern int NuiGetSensorCount(out int pCount);

    /// <summary>
    /// 获取骨骼帧和平滑参数
    /// </summary>
    /// <param name="pSkeletonFrame">骨骼帧</param>
    /// <param name="pSmoothingParams">平滑参数</param>
    /// <returns></returns>
	[DllImport(@"Kinect10.dll")]
	private static extern int NuiTransformSmooth(ref NuiSkeletonFrame pSkeletonFrame, ref NuiTransformSmoothParameters pSmoothingParams);

    /// <summary>
    /// 图像从分辨率的深度像素获取颜色像素坐标
    /// </summary>
    /// <param name="eColorResolution">颜色分辨率</param>
    /// <param name="eDepthResolution">深度分辨率</param>
    /// <param name="pcViewArea">视图区域</param>
    /// <param name="lDepthX">深度X</param>
    /// <param name="lDepthY">深度Y</param>
    /// <param name="sDepthValue">深度值</param>
    /// <param name="plColorX">颜色X</param>
    /// <param name="plColorY">颜色Y</param>
    /// <returns></returns>
	[DllImport(@"Kinect10.dll")]
	private static extern int NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(NuiImageResolution eColorResolution, NuiImageResolution eDepthResolution, ref NuiImageViewArea pcViewArea, int lDepthX, int lDepthY, ushort sDepthValue, out int plColorX, out int plColorY);

    /// <summary>
    /// 初始化Kinect传感器
    /// </summary>
    /// <param name="dwFlags">标志</param>
    /// <param name="bEnableEvents">启用事件？</param>
    /// <param name="iColorResolution">颜色分辨率</param>
    /// <param name="iDepthResolution">深度分辨率</param>
    /// <param name="bNearMode">近距离模式？</param>
    /// <returns></returns>
    [DllImportAttribute(@"KinectUnityWrapper.dll")]
	private static extern int InitKinectSensor(NuiInitializeFlags dwFlags, bool bEnableEvents, int iColorResolution, int iDepthResolution, bool bNearMode);
	
    /// <summary>
    /// 关闭Kinect传感器
    /// </summary>
	[DllImportAttribute(@"KinectUnityWrapper.dll")]
	private static extern void ShutdownKinectSensor();

    /// <summary>
    /// 设置Kinect角度
    /// </summary>
    /// <param name="sensorAngle">传感器角度</param>
    /// <returns></returns>
    [DllImportAttribute(@"KinectUnityWrapper.dll")]
	private static extern int SetKinectElevationAngle(int sensorAngle);
	
    /// <summary>
    /// 获取Kinect角度
    /// </summary>
    /// <returns></returns>
	[DllImportAttribute(@"KinectUnityWrapper.dll")]
	private static extern int GetKinectElevationAngle();
	
    /// <summary>
    /// 更新Kinect传感器
    /// </summary>
    /// <returns></returns>
	[DllImportAttribute(@"KinectUnityWrapper.dll")]
	private static extern int UpdateKinectSensor();

    /// <summary>
    /// 获取骨骼帧长度
    /// </summary>
    /// <returns></returns>
    [DllImport(@"KinectUnityWrapper.dll")]
	private static extern int GetSkeletonFrameLength();
	
    /// <summary>
    /// 获取骨骼帧数据
    /// </summary>
    /// <param name="pSkeletonData">骨骼数据</param>
    /// <param name="iDataBufLen">数据长度</param>
    /// <param name="bNewFrame">新帧？</param>
    /// <returns></returns>
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern bool GetSkeletonFrameData(ref NuiSkeletonFrame pSkeletonData, ref uint iDataBufLen, bool bNewFrame);

    /// <summary>
    /// 获取下一骨骼帧
    /// </summary>
    /// <param name="dwWaitMs">毫秒等待</param>
    /// <returns></returns>
    [DllImport(@"KinectUnityWrapper.dll")]
	private static extern int GetNextSkeletonFrame(uint dwWaitMs);
    /// <summary>
    /// 获取颜色流处理
    /// </summary>
    /// <returns></returns>
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern IntPtr GetColorStreamHandle();
    /// <summary>
    /// 获取深度流处理
    /// </summary>
    /// <returns></returns>
    [DllImport(@"KinectUnityWrapper.dll")]
	private static extern IntPtr GetDepthStreamHandle();
	
    /// <summary>
    /// 获取颜色帧数据
    /// </summary>
    /// <param name="btVideoBuf">视频缓冲区</param>
    /// <param name="iVideoBufLen">视频缓冲区长度</param>
    /// <param name="bGetNewFrame">获取新一帧？</param>
    /// <returns></returns>
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern bool GetColorFrameData(IntPtr btVideoBuf, ref uint iVideoBufLen, bool bGetNewFrame);
	/// <summary>
    /// 获取深度帧数据
    /// </summary>
    /// <param name="shDepthBuf">深度缓冲区</param>
    /// <param name="iDepthBufLen">深度缓冲区长度</param>
    /// <param name="bGetNewFrame">获取新一帧？</param>
    /// <returns></returns>
	[DllImport(@"KinectUnityWrapper.dll")]
	private static extern bool GetDepthFrameData(IntPtr shDepthBuf, ref uint iDepthBufLen, bool bGetNewFrame);
    /// <summary>
    /// 获取红外帧数据
    /// </summary>
    /// <param name="shInfraredBuf">红外缓冲区</param>
    /// <param name="iInfraredBufLen">红外缓冲区长度</param>
    /// <param name="bGetNewFrame">新一帧？</param>
    /// <returns></returns>
    [DllImport(@"KinectUnityWrapper.dll")]
	private static extern bool GetInfraredFrameData(IntPtr shInfraredBuf, ref uint iInfraredBufLen, bool bGetNewFrame);

    /// <summary>
    /// 初始化Kinect互动
    /// </summary>
    /// <returns></returns>
	[DllImport(@"KinectUnityWrapper")]
	private static extern int InitKinectInteraction();
    /// <summary>
    /// 完成Kinect互动
    /// </summary>
    [DllImport(@"KinectUnityWrapper")]
	private static extern void FinishKinectInteraction();
	/// <summary>
    /// 获取互动数
    /// </summary>
    /// <returns></returns>
	[DllImport( @"KinectUnityWrapper")]
	private static extern uint GetInteractorsCount();
	
    /// <summary>
    /// 获取骨骼跟踪ID
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns></returns>
	[DllImport( @"KinectUnityWrapper", EntryPoint = "GetInteractorSkeletonTrackingID" )]
	private static extern uint GetSkeletonTrackingID( uint player );
	
    /// <summary>
    /// 获取左手状态
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
	[DllImport( @"KinectUnityWrapper", EntryPoint = "GetInteractorLeftHandState" )]
	private static extern uint GetLeftHandState( uint player );
	
    /// <summary>
    /// 获取右手状态
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
	[DllImport( @"KinectUnityWrapper", EntryPoint = "GetInteractorRightHandState" )]
	private static extern uint GetRightHandState( uint player );
	/// <summary>
    /// 获取左手事件
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
	[DllImport( @"KinectUnityWrapper", EntryPoint = "GetInteractorLeftHandEvent" )]
	private static extern InteractionHandEventType GetLeftHandEvent( uint player );
	/// <summary>
    /// 获取右手事件
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
	[DllImport( @"KinectUnityWrapper", EntryPoint = "GetInteractorRightHandEvent" )]
	private static extern InteractionHandEventType GetRightHandEvent( uint player );
	

    /// <summary>
    /// 返回错误信息
    /// </summary>
    /// <param name="hr">错误代码</param>
    /// <returns></returns>
	private string GetNuiErrorString(int hr)
	{
		string message = string.Empty;
		uint uhr = (uint)hr;
		
		switch(uhr)
		{
		case (uint)NuiErrorCodes.FrameNoData:
			message = "帧无数据（Frame contains no data.）";
			break;
		case (uint)NuiErrorCodes.StreamNotEnabled:
			message = "数据流未启用（Stream is not enabled.）";
			break;
		case (uint)NuiErrorCodes.ImageStreamInUse:
			message = "图像流在使用中（Image stream is already in use.）";
			break;
		case (uint)NuiErrorCodes.FrameLimitExceeded:
			message = "帧限制超出（Frame limit is exceeded.）";
			break;
		case (uint)NuiErrorCodes.FeatureNotInitialized:
			message = "功能未初始化（Feature is not initialized.）";
			break;
		case (uint)NuiErrorCodes.DeviceNotGenuine:
			message = "设备不是正版（Device is not genuine.）";
			break;
		case (uint)NuiErrorCodes.InsufficientBandwidth:
			message = "带宽不足（Bandwidth is not sufficient.）";
			break;
		case (uint)NuiErrorCodes.DeviceNotSupported:
			message = "设备不支持（Device is not supported (e.g. Kinect for XBox 360).）";
			break;
		case (uint)NuiErrorCodes.DeviceInUse:
			message = "设备已被使用（Device is already in use.）";
			break;
		case (uint)NuiErrorCodes.DatabaseNotFound:
			message = "找不到数据库（Database not found.）";
			break;
		case (uint)NuiErrorCodes.DatabaseVersionMismatch:
			message = "数据库版本不匹配（Database version mismatch.）";
			break;
		case (uint)NuiErrorCodes.HardwareFeatureUnavailable:
			message = "硬件功能不可用（Hardware feature is not available.）";
			break;
		case (uint)NuiErrorCodes.DeviceNotConnected:
			message = "设备未连接（Device is not connected.）";
			break;
		case (uint)NuiErrorCodes.DeviceNotReady:
			message = "设备未就绪（Device is not ready.）";
			break;
		case (uint)NuiErrorCodes.SkeletalEngineBusy:
			message = "骨骼引擎忙（Skeletal engine is busy.）";
			break;
		case (uint)NuiErrorCodes.DeviceNotPowered:
			message = "设备未上电（Device is not powered.）";
			break;
			
		default:
			message = "hr=0x" + uhr.ToString("X");
			break;
		}
		
		return message;
	}

    /// <summary>
    /// 图像分辨率大小
    /// </summary>
    /// <param name="res">图像分辨率</param>
    /// <param name="refWidth">宽度</param>
    /// <param name="refHeight">高度</param>
    /// <returns></returns>
	private bool NuiImageResolutionToSize(NuiImageResolution res, out int refWidth, out int refHeight)
	{
		switch( res )
		{
			case NuiImageResolution.resolution80x60:
				refWidth = 80;
				refHeight = 60;
				return true;
			case NuiImageResolution.resolution320x240:
				refWidth = 320;
				refHeight = 240;
				return true;
			case NuiImageResolution.resolution640x480:
				refWidth = 640;
				refHeight = 480;
				return true;
			case NuiImageResolution.resolution1280x960:
				refWidth = 1280;
				refHeight = 960;
				return true;
			default:
				refWidth = 0;
				refHeight = 0;
				break;
		}

		return false;
	}
	
    /// <summary>
    /// 初始化传感器接口
    /// </summary>
    /// <param name="bNeedRestart">需要重启？</param>
    /// <returns></returns>
	public bool InitSensorInterface (ref bool bNeedRestart)
	{
		bool bOneCopied = false, bAllCopied = true;
		
		KinectInterop.CopyResourceFile("KinectUnityWrapper.dll", "KinectUnityWrapper.dll", ref bOneCopied, ref bAllCopied);
		KinectInterop.CopyResourceFile("KinectInteraction180_32.dll", "KinectInteraction180_32.dll", ref bOneCopied, ref bAllCopied);
		KinectInterop.CopyResourceFile("FaceTrackData.dll", "FaceTrackData.dll", ref bOneCopied, ref bAllCopied);
		KinectInterop.CopyResourceFile("FaceTrackLib.dll", "FaceTrackLib.dll", ref bOneCopied, ref bAllCopied);
		KinectInterop.CopyResourceFile("KinectBackgroundRemoval180_32.dll", "KinectBackgroundRemoval180_32.dll", ref bOneCopied, ref bAllCopied);
		
		KinectInterop.CopyResourceFile("msvcp100d.dll", "msvcp100d.dll", ref bOneCopied, ref bAllCopied);
		KinectInterop.CopyResourceFile("msvcr100d.dll", "msvcr100d.dll", ref bOneCopied, ref bAllCopied);
		
		bNeedRestart = (bOneCopied && bAllCopied);

		return true;
	}

	public void FreeSensorInterface ()
	{
	}

    /// <summary>
    /// 获取传感器数量
    /// </summary>
    /// <returns></returns>
	public int GetSensorsCount ()
	{
		int iSensorCount = 0;
		int hr = NuiGetSensorCount(out iSensorCount);

		if(hr == 0)
			return iSensorCount;
		else
			return 0;
	}
    /// <summary>
    /// 打开默认传感器
    /// </summary>
    /// <param name="dwFlags">标志</param>
    /// <param name="sensorAngle">传感器角度</param>
    /// <param name="bUseMultiSource">使用多源？</param>
    /// <returns></returns>
	public KinectInterop.SensorData OpenDefaultSensor (KinectInterop.FrameSource dwFlags, float sensorAngle, bool bUseMultiSource)
	{
		sourceFlags = dwFlags;

		NuiInitializeFlags nuiFlags = // NuiInitializeFlags.UsesNone;
			NuiInitializeFlags.UsesSkeleton | NuiInitializeFlags.UsesDepthAndPlayerIndex;

		if((dwFlags & KinectInterop.FrameSource.TypeBody) != 0)
		{
			nuiFlags |= NuiInitializeFlags.UsesSkeleton;
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeColor) != 0)
		{
			nuiFlags |= NuiInitializeFlags.UsesColor;
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeDepth) != 0)
		{
			nuiFlags |= NuiInitializeFlags.UsesDepthAndPlayerIndex;
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeBodyIndex) != 0)
		{
			nuiFlags |= NuiInitializeFlags.UsesDepthAndPlayerIndex;
		}
		
		if((dwFlags & KinectInterop.FrameSource.TypeInfrared) != 0)
		{
			nuiFlags |= (NuiInitializeFlags.UsesColor | (NuiInitializeFlags)0x8000);
		}
        //初始化Kinect传感器
        int hr = InitKinectSensor(nuiFlags, true, (int)Constants.ColorImageResolution, (int)Constants.DepthImageResolution, Constants.IsNearMode);

		if(hr == 0)
		{
			// set sensor angle
			SetKinectElevationAngle((int)sensorAngle);

			// initialize Kinect interaction
			hr = InitKinectInteraction();
			if(hr != 0)
			{
				Debug.LogError("Initialization of KinectInteraction failed.");
			}
			
			KinectInterop.SensorData sensorData = new KinectInterop.SensorData();

			sensorData.bodyCount = Constants.NuiSkeletonCount;
			sensorData.jointCount = 20;

			NuiImageResolutionToSize(Constants.ColorImageResolution, out sensorData.colorImageWidth, out sensorData.colorImageHeight);
//			sensorData.colorImageWidth = Constants.ColorImageWidth;
//			sensorData.colorImageHeight = Constants.ColorImageHeight;

			if((dwFlags & KinectInterop.FrameSource.TypeColor) != 0)
			{
				//colorStreamHandle =  GetColorStreamHandle();
				sensorData.colorImage = new byte[sensorData.colorImageWidth * sensorData.colorImageHeight * 4];
			}

			NuiImageResolutionToSize(Constants.DepthImageResolution, out sensorData.depthImageWidth, out sensorData.depthImageHeight);
//			sensorData.depthImageWidth = Constants.DepthImageWidth;
//			sensorData.depthImageHeight = Constants.DepthImageHeight;
			
			if((dwFlags & KinectInterop.FrameSource.TypeDepth) != 0)
			{
				//depthStreamHandle = GetDepthStreamHandle();
				sensorData.depthImage = new ushort[sensorData.depthImageWidth * sensorData.depthImageHeight];
			}
			
			if((dwFlags & KinectInterop.FrameSource.TypeBodyIndex) != 0)
			{
				sensorData.bodyIndexImage = new byte[sensorData.depthImageWidth * sensorData.depthImageHeight];
			}
			
			if((dwFlags & KinectInterop.FrameSource.TypeInfrared) != 0)
			{
				sensorData.infraredImage = new ushort[sensorData.colorImageWidth * sensorData.colorImageHeight];
			}

			if((dwFlags & KinectInterop.FrameSource.TypeBody) != 0)
			{
				skeletonFrame = new NuiSkeletonFrame() 
				{ 
					SkeletonData = new NuiSkeletonData[Constants.NuiSkeletonCount] 
				};
				
				// default values used to pass to smoothing function
				smoothParameters = new NuiTransformSmoothParameters();

				smoothParameters.fSmoothing = 0.5f;
				smoothParameters.fCorrection = 0.5f;
				smoothParameters.fPrediction = 0.5f;
				smoothParameters.fJitterRadius = 0.05f;
				smoothParameters.fMaxDeviationRadius = 0.04f;
			}
			
			return sensorData;
		}
		else
		{
			Debug.LogError("InitKinectSensor failed: " + GetNuiErrorString(hr));
		}

		return null;
	}

    /// <summary>
    /// 关闭传感器
    /// </summary>
    /// <param name="sensorData"></param>
	public void CloseSensor (KinectInterop.SensorData sensorData)
	{
		FinishKinectInteraction();
		ShutdownKinectSensor();
	}

    /// <summary>
    /// 更新传感器数据
    /// </summary>
    /// <param name="sensorData"></param>
    /// <returns></returns>
	public bool UpdateSensorData (KinectInterop.SensorData sensorData)
	{
		int hr = UpdateKinectSensor();
		return (hr == 0);
	}

    /// <summary>
    /// 获取多源帧(暂时无效)
    /// </summary>
    /// <param name="sensorData"></param>
    /// <returns></returns>
	public bool GetMultiSourceFrame (KinectInterop.SensorData sensorData)
	{
		return false;
	}

	public void FreeMultiSourceFrame (KinectInterop.SensorData sensorData)
	{
	}

    /// <summary>
    /// 获取骨骼下一帧
    /// </summary>
    /// <param name="dwMillisecondsToWait">毫秒等待</param>
    /// <param name="pSkeletonFrame">骨骼帧</param>
    /// <returns></returns>
	private int NuiSkeletonGetNextFrame(uint dwMillisecondsToWait, ref NuiSkeletonFrame pSkeletonFrame)
	{
		if(sourceFlags != KinectInterop.FrameSource.TypeAudio)
		{
			// non-audio sources
			uint iFrameLength = (uint)GetSkeletonFrameLength();
			bool bSuccess = GetSkeletonFrameData(ref pSkeletonFrame, ref iFrameLength, true);
			return bSuccess ? 0 : -1;
		}
		else
		{
			// audio only
			int hr = GetNextSkeletonFrame(dwMillisecondsToWait);

			if(hr == 0)
			{
				uint iFrameLength = (uint)GetSkeletonFrameLength();
				bool bSuccess = GetSkeletonFrameData(ref pSkeletonFrame, ref iFrameLength, true);
				
				return bSuccess ? 0 : -1;
			}
			
			return hr;
		}
	}

    /// <summary>
    /// 获取手状态和置信度
    /// </summary>
    /// <param name="handState">手状态</param>
    /// <param name="handEvent">手事件</param>
    /// <param name="refHandState">手状态</param>
    /// <param name="refHandConf">手的置信度</param>
	private void GetHandStateAndConf(uint handState, InteractionHandEventType handEvent, 
	                                 ref KinectInterop.HandState refHandState, ref KinectInterop.TrackingConfidence refHandConf)
	{
        Debug.Log("执行GetHandStateAndConf()");

		bool bHandPrimary = (handState & (uint)NuiHandpointerState.PrimaryForUser) != 0;

		refHandConf = bHandPrimary ? KinectInterop.TrackingConfidence.High : KinectInterop.TrackingConfidence.Low;

		if(bHandPrimary)
		{
			switch(handEvent)
			{
				case InteractionHandEventType.Grip:
					refHandState = KinectInterop.HandState.Closed;
					break;

				case InteractionHandEventType.Release:
				//case InteractionHandEventType.None:
					refHandState = KinectInterop.HandState.Open;
					break;
			}
		}
		else
		{
			refHandState = KinectInterop.HandState.NotTracked;
		}
	}

    /// <summary>
    /// 轮询身体帧
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="bodyFrame">身体帧</param>
    /// <param name="kinectToWorld">Kinect到世界矩阵</param>
    /// <returns></returns>
	public bool PollBodyFrame (KinectInterop.SensorData sensorData, ref KinectInterop.BodyFrameData bodyFrame, ref Matrix4x4 kinectToWorld)
	{
		bool newSkeleton = false;
		
		int hr = NuiSkeletonGetNextFrame(0, ref skeletonFrame);
		if(hr == 0)
		{
			newSkeleton = true;
		}
		
		if(newSkeleton)
		{
			hr = NuiTransformSmooth(ref skeletonFrame, ref smoothParameters);
			if(hr != 0)
			{
				Debug.LogError("Skeleton Data Smoothing failed");
			}

			for(uint i = 0; i < sensorData.bodyCount; i++)
			{
				NuiSkeletonData body = skeletonFrame.SkeletonData[i];
				
				bodyFrame.bodyData[i].bIsTracked = (short)(body.eTrackingState ==  NuiSkeletonTrackingState.SkeletonTracked ? 1 : 0);
				
				if(body.eTrackingState ==  NuiSkeletonTrackingState.SkeletonTracked)
				{
					// transfer body and joints data
					bodyFrame.bodyData[i].liTrackingID = (long)body.dwTrackingID;
					
					for(int j = 0; j < sensorData.jointCount; j++)
					{
						KinectInterop.JointData jointData = bodyFrame.bodyData[i].joint[j];
						
						jointData.jointType = GetJointAtIndex(j);
						jointData.trackingState = (KinectInterop.TrackingState)body.eSkeletonPositionTrackingState[j];
						
						if(jointData.trackingState != KinectInterop.TrackingState.NotTracked)
						{
							jointData.kinectPos = body.SkeletonPositions[j];
							jointData.position = kinectToWorld.MultiplyPoint3x4(jointData.kinectPos);
						}
						
						jointData.orientation = Quaternion.identity;
//							Windows.Kinect.Vector4 vQ = body.JointOrientations[jointData.jointType].Orientation;
//							jointData.orientation = new Quaternion(vQ.X, vQ.Y, vQ.Z, vQ.W);
						
						if(j == 0)
						{
							bodyFrame.bodyData[i].position = jointData.position;
							bodyFrame.bodyData[i].orientation = jointData.orientation;
						}
						
						bodyFrame.bodyData[i].joint[j] = jointData;
					}


					// tranfer hand states
					uint intCount = GetInteractorsCount();

					for(uint intIndex = 0; intIndex < intCount; intIndex++)
					{
						uint skeletonId = GetSkeletonTrackingID(intIndex);

						if(skeletonId == body.dwTrackingID)
						{
                            //从C++ dll中获取左手状态
							uint leftHandState = GetLeftHandState(intIndex);
                            //从C++ dll中获取到左右事件
							InteractionHandEventType leftHandEvent = GetLeftHandEvent(intIndex);


							uint rightHandState = GetRightHandState(intIndex);
							InteractionHandEventType rightHandEvent = GetRightHandEvent(intIndex);

                            GetHandStateAndConf(leftHandState, leftHandEvent, 
							                    ref bodyFrame.bodyData[i].leftHandState, 
							                    ref bodyFrame.bodyData[i].leftHandConfidence);
							
							GetHandStateAndConf(rightHandState, rightHandEvent, 
							                    ref bodyFrame.bodyData[i].rightHandState, 
							                    ref bodyFrame.bodyData[i].rightHandConfidence);
						}
					}

				}
			}
			
		}
		
		return newSkeleton;
	}

    /// <summary>
    /// 轮询颜色帧
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns></returns>
	public bool PollColorFrame (KinectInterop.SensorData sensorData)
	{
		uint videoBufLen = (uint)sensorData.colorImage.Length;
		
		var pColorData = GCHandle.Alloc(sensorData.colorImage, GCHandleType.Pinned);
		bool newColor = GetColorFrameData(pColorData.AddrOfPinnedObject(), ref videoBufLen, true);
		pColorData.Free();
		
		if (newColor)
		{
			for (int i = 0; i < videoBufLen; i += 4)
			{
				byte btTmp = sensorData.colorImage[i];
				sensorData.colorImage[i] = sensorData.colorImage[i + 2];
				sensorData.colorImage[i + 2] = btTmp;
				sensorData.colorImage[i + 3] = 255;
			}
		}

		return newColor;
	}

    /// <summary>
    /// 轮询深度帧
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns></returns>
	public bool PollDepthFrame (KinectInterop.SensorData sensorData)
	{
		uint depthBufLen = (uint)(sensorData.depthImage.Length * sizeof(ushort));
		
		var pDepthData = GCHandle.Alloc(sensorData.depthImage, GCHandleType.Pinned);
		bool newDepth = GetDepthFrameData(pDepthData.AddrOfPinnedObject(), ref depthBufLen, true);
		pDepthData.Free();

		if(newDepth)
		{
			uint depthLen = (uint)sensorData.depthImage.Length;

			for (int i = 0; i < depthLen; i++)
			{
				if((sensorData.depthImage[i] & 7) != 0)
					sensorData.bodyIndexImage[i] = (byte)((sensorData.depthImage[i] & 7) - 1);
				else
					sensorData.bodyIndexImage[i] = 255;

				sensorData.depthImage[i] = (ushort)(sensorData.depthImage[i] >> 3);
			}
		}

		return newDepth;
	}

    /// <summary>
    /// 轮询红外帧
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns></returns>
	public bool PollInfraredFrame (KinectInterop.SensorData sensorData)
	{
		uint infraredBufLen = (uint)(sensorData.infraredImage.Length * sizeof(ushort));
		
		var pInfraredData = GCHandle.Alloc(sensorData.infraredImage, GCHandleType.Pinned);
		bool newInfrared = GetInfraredFrameData(pInfraredData.AddrOfPinnedObject(), ref infraredBufLen, true);
		pInfraredData.Free();
		
		return newInfrared;
	}

    /// <summary>
    /// 固定关节方向
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="bodyData">身体数据</param>
	public void FixJointOrientations(KinectInterop.SensorData sensorData, ref KinectInterop.BodyData bodyData)
	{
		// fix the hips-to-spine tilt (it is about 40 degrees to the back)
		int hipsIndex = (int)KinectInterop.JointType.SpineBase;

		Quaternion quat = bodyData.joint[hipsIndex].normalRotation;
		//quat *= Quaternion.Euler(40f, 0f, 0f);
		bodyData.joint[hipsIndex].normalRotation = quat;

		Vector3 mirroredAngles = quat.eulerAngles;
		mirroredAngles.y = -mirroredAngles.y;
		mirroredAngles.z = -mirroredAngles.z;
		bodyData.joint[hipsIndex].mirroredRotation = Quaternion.Euler(mirroredAngles);

		bodyData.normalRotation = bodyData.joint[hipsIndex].normalRotation;
		bodyData.mirroredRotation = bodyData.joint[hipsIndex].mirroredRotation;
	}
	
    /// <summary>
    /// 将骨骼变换为深度图像
    /// </summary>
    /// <param name="vPoint">点</param>
    /// <param name="pfDepthX">深度X</param>
    /// <param name="pfDepthY">深度Y</param>
    /// <param name="pfDepthZ">深度Z</param>
	private static void NuiTransformSkeletonToDepthImage(Vector3 vPoint, out float pfDepthX, out float pfDepthY, out float pfDepthZ)
	{
		if (vPoint.z > float.Epsilon)
		{
			pfDepthX = 0.5f + ((vPoint.x * 285.63f) / (vPoint.z * 320f));
			pfDepthY = 0.5f - ((vPoint.y * 285.63f) / (vPoint.z * 240f));
			pfDepthZ = vPoint.z * 1000f;
		}
		else
		{
			pfDepthX = 0f;
			pfDepthY = 0f;
			pfDepthZ = 0f;
		}
	}

    /// <summary>
    /// 将空间点映射到深度线
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="spacePos">空间位置</param>
    /// <returns></returns>
    public Vector2 MapSpacePointToDepthCoords (KinectInterop.SensorData sensorData, Vector3 spacePos)
	{
		float fDepthX, fDepthY, fDepthZ;
		NuiTransformSkeletonToDepthImage(spacePos, out fDepthX, out fDepthY, out fDepthZ);
		
		Vector3 point = new Vector3();
		point.x = (int)((fDepthX * sensorData.depthImageWidth) + 0.5f);
		point.y = (int)((fDepthY * sensorData.depthImageHeight) + 0.5f);
		point.z = (int)(fDepthZ + 0.5f);
		
		return point;
	}
    /// <summary>
    /// 地图深度点到空间线
    /// </summary>
    /// <param name="sensorData"></param>
    /// <param name="depthPos"></param>
    /// <param name="depthVal"></param>
    /// <returns></returns>
	public Vector3 MapDepthPointToSpaceCoords (KinectInterop.SensorData sensorData, Vector2 depthPos, ushort depthVal)
	{
		throw new System.NotImplementedException ();
	}
    /// <summary>
    /// 将深度点映射到彩色线
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="depthPos">深度位置</param>
    /// <param name="depthVal">深度值</param>
    /// <returns></returns>
	public Vector2 MapDepthPointToColorCoords (KinectInterop.SensorData sensorData, Vector2 depthPos, ushort depthVal)
	{
		int cx, cy;
		NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(
			Constants.ColorImageResolution,
			Constants.DepthImageResolution,
			ref pcViewArea,
			(int)depthPos.x, (int)depthPos.y, (ushort)(depthVal << 3),
			out cx, out cy);
		
		return new Vector2(cx, cy);
	}
    /// <summary>
    /// 将深度帧映射到彩色线
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="vColorCoords">彩色线</param>
    /// <returns></returns>
	public bool MapDepthFrameToColorCoords (KinectInterop.SensorData sensorData, ref Vector2[] vColorCoords)
	{
		return false;
	}

    // returns the index of the given joint in joint's array or -1 if joint is not applicable
    /// <summary>
    /// 在关节数组中返回给定关节的索引，如果关节不适用则返回-1
    /// </summary>
    /// <param name="joint">关节</param>
    /// <returns></returns>
    public int GetJointIndex(KinectInterop.JointType joint)
	{
		switch(joint)
		{
			case KinectInterop.JointType.SpineBase:
				return (int)NuiSkeletonPositionIndex.HipCenter;
			case KinectInterop.JointType.SpineMid:
				return (int)NuiSkeletonPositionIndex.Spine;
			case KinectInterop.JointType.Neck:
				return (int)NuiSkeletonPositionIndex.ShoulderCenter;
			case KinectInterop.JointType.Head:
				return (int)NuiSkeletonPositionIndex.Head;
				
			case KinectInterop.JointType.ShoulderLeft:
				return (int)NuiSkeletonPositionIndex.ShoulderLeft;
			case KinectInterop.JointType.ElbowLeft:
				return (int)NuiSkeletonPositionIndex.ElbowLeft;
			case KinectInterop.JointType.WristLeft:
				return (int)NuiSkeletonPositionIndex.WristLeft;
			case KinectInterop.JointType.HandLeft:
				return (int)NuiSkeletonPositionIndex.HandLeft;
				
			case KinectInterop.JointType.ShoulderRight:
				return (int)NuiSkeletonPositionIndex.ShoulderRight;
			case KinectInterop.JointType.ElbowRight:
				return (int)NuiSkeletonPositionIndex.ElbowRight;
			case KinectInterop.JointType.WristRight:
				return (int)NuiSkeletonPositionIndex.WristRight;
			case KinectInterop.JointType.HandRight:
				return (int)NuiSkeletonPositionIndex.HandRight;
				
			case KinectInterop.JointType.HipLeft:
				return (int)NuiSkeletonPositionIndex.HipLeft;
			case KinectInterop.JointType.KneeLeft:
				return (int)NuiSkeletonPositionIndex.KneeLeft;
			case KinectInterop.JointType.AnkleLeft:
				return (int)NuiSkeletonPositionIndex.AnkleLeft;
			case KinectInterop.JointType.FootLeft:
				return (int)NuiSkeletonPositionIndex.FootLeft;
				
			case KinectInterop.JointType.HipRight:
				return (int)NuiSkeletonPositionIndex.HipRight;
			case KinectInterop.JointType.KneeRight:
				return (int)NuiSkeletonPositionIndex.KneeRight;
			case KinectInterop.JointType.AnkleRight:
				return (int)NuiSkeletonPositionIndex.AnkleRight;
			case KinectInterop.JointType.FootRight:
				return (int)NuiSkeletonPositionIndex.FootRight;
		}
		
		return -1;
	}

    // returns the joint at given index
    /// <summary>
    /// 以给定的索引返回关节
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns></returns>
    public KinectInterop.JointType GetJointAtIndex(int index)
	{
		switch(index)
		{
		case (int)NuiSkeletonPositionIndex.HipCenter:
			return KinectInterop.JointType.SpineBase;
		case (int)NuiSkeletonPositionIndex.Spine:
			return KinectInterop.JointType.SpineMid;
		case (int)NuiSkeletonPositionIndex.ShoulderCenter:
			return KinectInterop.JointType.Neck;
		case (int)NuiSkeletonPositionIndex.Head:
			return KinectInterop.JointType.Head;
			
		case (int)NuiSkeletonPositionIndex.ShoulderLeft:
			return KinectInterop.JointType.ShoulderLeft;
		case (int)NuiSkeletonPositionIndex.ElbowLeft:
			return KinectInterop.JointType.ElbowLeft;
		case (int)NuiSkeletonPositionIndex.WristLeft:
			return KinectInterop.JointType.WristLeft;
		case (int)NuiSkeletonPositionIndex.HandLeft:
			return KinectInterop.JointType.HandLeft;
			
		case (int)NuiSkeletonPositionIndex.ShoulderRight:
			return KinectInterop.JointType.ShoulderRight;
		case (int)NuiSkeletonPositionIndex.ElbowRight:
			return KinectInterop.JointType.ElbowRight;
		case (int)NuiSkeletonPositionIndex.WristRight:
			return KinectInterop.JointType.WristRight;
		case (int)NuiSkeletonPositionIndex.HandRight:
			return KinectInterop.JointType.HandRight;
			
		case (int)NuiSkeletonPositionIndex.HipLeft:
			return KinectInterop.JointType.HipLeft;
		case (int)NuiSkeletonPositionIndex.KneeLeft:
			return KinectInterop.JointType.KneeLeft;
		case (int)NuiSkeletonPositionIndex.AnkleLeft:
			return KinectInterop.JointType.AnkleLeft;
		case (int)NuiSkeletonPositionIndex.FootLeft:
			return KinectInterop.JointType.FootLeft;
			
		case (int)NuiSkeletonPositionIndex.HipRight:
			return KinectInterop.JointType.HipRight;
		case (int)NuiSkeletonPositionIndex.KneeRight:
			return KinectInterop.JointType.KneeRight;
		case (int)NuiSkeletonPositionIndex.AnkleRight:
			return KinectInterop.JointType.AnkleRight;
		case (int)NuiSkeletonPositionIndex.FootRight:
			return KinectInterop.JointType.FootRight;
		}
		
		return (KinectInterop.JointType)(-1);
	}

    // returns the parent joint of the given joint
    /// <summary>
    /// 返回给定连接的父节点
    /// </summary>
    /// <param name="joint"></param>
    /// <returns></returns>
    public KinectInterop.JointType GetParentJoint(KinectInterop.JointType joint)
	{
		switch(joint)
		{
			case KinectInterop.JointType.SpineBase:
				return KinectInterop.JointType.SpineBase;
				
			case KinectInterop.JointType.ShoulderLeft:
			case KinectInterop.JointType.ShoulderRight:
				return KinectInterop.JointType.Neck;
				
			case KinectInterop.JointType.HipLeft:
			case KinectInterop.JointType.HipRight:
				return KinectInterop.JointType.SpineBase;
		}
		
		return (KinectInterop.JointType)((int)joint - 1);
	}

    // returns the next joint in the hierarchy, as to the given joint
    /// <summary>
    /// 在层次结构中返回下一个节点，对于给定的节点
    /// </summary>
    /// <param name="joint">关节</param>
    /// <returns></returns>
    public KinectInterop.JointType GetNextJoint(KinectInterop.JointType joint)
	{
		switch(joint)
		{
			case KinectInterop.JointType.SpineBase:
				return KinectInterop.JointType.SpineMid;
			case KinectInterop.JointType.SpineMid:
				return KinectInterop.JointType.Neck;
			case KinectInterop.JointType.Neck:
				return KinectInterop.JointType.Head;
				
			case KinectInterop.JointType.ShoulderLeft:
				return KinectInterop.JointType.ElbowLeft;
			case KinectInterop.JointType.ElbowLeft:
				return KinectInterop.JointType.WristLeft;
			case KinectInterop.JointType.WristLeft:
				return KinectInterop.JointType.HandLeft;
				
			case KinectInterop.JointType.ShoulderRight:
				return KinectInterop.JointType.ElbowRight;
			case KinectInterop.JointType.ElbowRight:
				return KinectInterop.JointType.WristRight;
			case KinectInterop.JointType.WristRight:
				return KinectInterop.JointType.HandRight;
				
			case KinectInterop.JointType.HipLeft:
				return KinectInterop.JointType.KneeLeft;
			case KinectInterop.JointType.KneeLeft:
				return KinectInterop.JointType.AnkleLeft;
			case KinectInterop.JointType.AnkleLeft:
				return KinectInterop.JointType.FootLeft;
				
			case KinectInterop.JointType.HipRight:
				return KinectInterop.JointType.KneeRight;
			case KinectInterop.JointType.KneeRight:
				return KinectInterop.JointType.AnkleRight;
			case KinectInterop.JointType.AnkleRight:
				return KinectInterop.JointType.FootRight;
		}
		
		return joint;  // in case of end joint - Head, HandLeft, HandRight, FootLeft, FootRight
	}
	
}
