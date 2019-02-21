using UnityEngine;
using System.Collections;

/// <summary>
/// 深度传感器接口
/// </summary>
public interface DepthSensorInterface
{
    // inits libraries and resources needed by this sensor interface
    // returns true if the resources are successfully initialized, false otherwise
    /// <summary>
    /// 如果资源成功初始化，则此传感器接口所需的库和资源将返回true，否则为false
    /// </summary>
    /// <param name="bNeedRestart">是否需要重新启动</param>
    /// <returns>返回资源状态</returns>
    bool InitSensorInterface(ref bool bNeedRestart);

    // frees the resources and libraries used by this interface
    /// <summary>
    /// 释放此界面使用的资源和库
    /// </summary>
    void FreeSensorInterface();

    // returns the number of available sensors, controllable by this interface
    // if the number of sensors is 0, FreeSensorInterface() is invoked and the interface is not used any more
    /// <summary>
    /// 返回可用传感器的数量，如果传感器数量为0，则由此接口控制，FreeSensorInterface（）被调用，并且接口不再被使用
    /// </summary>
    /// <returns>可用传感器的数量</returns>
    int GetSensorsCount();

    // opens the default sensor and inits needed resources. returns new sensor-data object
    /// <summary>
    /// 打开默认传感器并输入所需的资源。 返回新的传感器数据对象
    /// </summary>
    /// <param name="dwFlags">帧源</param>
    /// <param name="sensorAngle">传感器角度</param>
    /// <param name="bUseMultiSource">使用多源</param>
    /// <returns></returns>
    KinectInterop.SensorData OpenDefaultSensor(KinectInterop.FrameSource dwFlags, float sensorAngle, bool bUseMultiSource);

    // closes the sensor and frees used resources
    /// <summary>
    /// 关闭传感器并释放已使用的资源
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    void CloseSensor(KinectInterop.SensorData sensorData);

    // invoked periodically to update sensor data, if needed
    // returns true if update is successful, false otherwise
    /// <summary>
    /// 定期调用更新传感器数据，如果需要，如果更新成功则返回true，否则为false
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns>更新状态</returns>
    bool UpdateSensorData(KinectInterop.SensorData sensorData);


    // gets next multi source frame, if one is available
    // returns true if there is a new multi-source frame, false otherwise
    /// <summary>
    /// 获得下一个多源帧，如果有可用，则如果存在新的多源帧，则返回true，否则为false
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns>则如果存在新的多源帧，则返回true，否则为false</returns>
    bool GetMultiSourceFrame(KinectInterop.SensorData sensorData);

    // frees the resources taken by the last multi-source frame
    /// <summary>
    /// 释放最后一个多源帧所占用的资源
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    void FreeMultiSourceFrame(KinectInterop.SensorData sensorData);

    // polls for new body/skeleton frame. must fill in all needed body and joints' elements (tracking state and position)
    // returns true if new body frame is available, false otherwise
    /// <summary>
    /// 轮询新的身体/骨架帧。 必须填写所有需要的身体和关节的元素（跟踪状态和位置）如果新的身体帧可用则返回true，否则为false
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="bodyFrame"></param>
    /// <param name="kinectToWorld">kinect世界矩阵</param>
    /// <returns></returns>
    bool PollBodyFrame(KinectInterop.SensorData sensorData, ref KinectInterop.BodyFrameData bodyFrame, ref Matrix4x4 kinectToWorld);

    // polls for new color frame data
    // returns true if new color frame is available, false otherwise
    /// <summary>
    /// 如果新的颜色框架可用，轮廓的新颜色框架数据将返回true，否则为false
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns>有新颜色框架则返回true，否则为false</returns>
    bool PollColorFrame(KinectInterop.SensorData sensorData);

    // polls for new depth and body index frame data
    // returns true if new depth or body index frame is available, false otherwise
    /// <summary>
    /// 如果新的深度或身体索引帧可用，则对新深度和身体索引帧数据的轮询返回true，否则为false
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns>如果新的深度或身体索引帧可用返回true，否则为false</returns>
    bool PollDepthFrame(KinectInterop.SensorData sensorData);

    // polls for new infrared frame data
    // returns true if new infrared frame is available, false otherwise
    /// <summary>
    /// 如果新的红外线帧可用，则新红外帧数据的轮询返回true，否则为false
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <returns></returns>
    bool PollInfraredFrame(KinectInterop.SensorData sensorData);

    // performs sensor-specific fixes of joint positions and orientations
    /// <summary>
    /// 执行传感器特定的关节位置和方向的修复
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="bodyData">身体数据</param>
    void FixJointOrientations(KinectInterop.SensorData sensorData, ref KinectInterop.BodyData bodyData);

    // returns depth frame coordinates for the given 3d space point
    /// <summary>
    /// 返回给定3d空间点的深度帧坐标
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="spacePos">空间位置</param>
    /// <returns></returns>
    Vector2 MapSpacePointToDepthCoords(KinectInterop.SensorData sensorData, Vector3 spacePos);

    // returns 3d Kinect-space coordinates for the given depth frame point
    /// <summary>
    /// 返回给定深度帧点的3D Kinect-space坐标
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="depthPos">深度位置</param>
    /// <param name="depthVal">深度值</param>
    /// <returns>3D Kinect-space坐标</returns>
    Vector3 MapDepthPointToSpaceCoords(KinectInterop.SensorData sensorData, Vector2 depthPos, ushort depthVal);

    // returns color-space coordinates for the given depth point
    /// <summary>
    /// 返回给定深度点的颜色空间坐标
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="depthPos">深度位置</param>
    /// <param name="depthVal">深度值</param>
    /// <returns></returns>
    Vector2 MapDepthPointToColorCoords(KinectInterop.SensorData sensorData, Vector2 depthPos, ushort depthVal);

    // estimates all color-space coordinates for the current depth frame
    // returns true on success, false otherwise
    /// <summary>
    /// 估计当前深度帧的所有颜色空间坐标在成功时返回true，否则为false
    /// </summary>
    /// <param name="sensorData">传感器数据</param>
    /// <param name="vColorCoords">颜色系</param>
    /// <returns></returns>
    bool MapDepthFrameToColorCoords(KinectInterop.SensorData sensorData, ref Vector2[] vColorCoords);


    // returns the index of the given joint in joint's array
    /// <summary>
    /// 返回关节数组中给定关节的索引
    /// </summary>
    /// <param name="joint">关节类型</param>
    /// <returns></returns>
    int GetJointIndex(KinectInterop.JointType joint);

    // returns the joint at given index
    /// <summary>
    /// 以给定的索引返回关节
    /// </summary>
    /// <param name="index">关节索引</param>
    /// <returns></returns>
    KinectInterop.JointType GetJointAtIndex(int index);

    // returns the parent joint of the given joint
    /// <summary>
    /// 返回给定关节的父关节
    /// </summary>
    /// <param name="joint">关节类型</param>
    /// <returns></returns>
    KinectInterop.JointType GetParentJoint(KinectInterop.JointType joint);

    // returns the next joint in the hierarchy, as to the given joint
    /// <summary>
    /// 返回层次结构中的下一个关节，关于给定的关节
    /// </summary>
    /// <param name="joint"></param>
    /// <returns></returns>
    KinectInterop.JointType GetNextJoint(KinectInterop.JointType joint);
}
