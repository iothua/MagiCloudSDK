using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
//using Windows.Kinect;

// Implementation of a Holt Double Exponential Smoothing filter. The double exponential
// smooths the curve and predicts.  There is also noise jitter removal. And maximum
// prediction bounds.  The parameters are commented in the Init function.
/// <summary>
/// Holt双指数平滑滤波器的实现。 
/// 双指数平滑曲线并预测。 
/// 还有噪声抖动消除。 
/// 和最大预测边界。 
/// 这些参数在Init函数中被注释。
/// </summary>
public class JointPositionsFilter
{
    // The history data.
    /// <summary>
    /// 历史数据。
    /// </summary>
    private FilterDoubleExponentialData[,] history;

    // The smoothing parameters for this filter.
    /// <summary>
    /// 该滤镜的平滑参数。
    /// </summary>
	private KinectInterop.SmoothParameters smoothParameters;

    // True when the filter parameters are initialized.
    /// <summary>
    /// 当过滤器参数初始化时为真。
    /// </summary>
    private bool init;


    // Initializes a new instance of the class.
    /// <summary>
    /// 初始化类的新实例。
    /// </summary>
    public JointPositionsFilter()
    {
        this.init = false;
    }

    // Initialize the filter with a default set of TransformSmoothParameters.
    /// <summary>
    /// 使用默认的TransformSmoothParameters集初始化过滤器。
    /// </summary>
    public void Init()
    {
        // Specify some defaults
        //this.Init(0.25f, 0.25f, 0.25f, 0.03f, 0.05f);
        //指定一些默认值
        this.Init(0.5f, 0.5f, 0.5f, 0.05f, 0.04f);
    }

    /// <summary>
    /// 使用一组手动指定Transform的SmoothParameters初始化过滤器。
    /// </summary>
    /// <param name="smoothingValue">Smoothing = [0..1], 较低的值更接近于原始数据，并且噪声更大。</param>
    /// <param name="correctionValue">Correction = [0..1], 更高的值正确更快，感觉更敏感。</param>
    /// <param name="predictionValue">Prediction = [0..n], 将来我们想预测多少帧。</param>
    /// <param name="jitterRadiusValue">JitterRadius = 定义抖动的以m为单位的偏差距离。</param>
    /// <param name="maxDeviationRadiusValue">MaxDeviation = 过滤位置的最大距离（m）允许偏离原始数据。</param>
    public void Init(float smoothingValue, float correctionValue, float predictionValue, float jitterRadiusValue, float maxDeviationRadiusValue)
    {
		this.smoothParameters = new KinectInterop.SmoothParameters();

        this.smoothParameters.smoothing = smoothingValue;                   // 会发生多少事情 会滞后的时候太高
        this.smoothParameters.correction = correctionValue;                 // 从预测中纠正多少。 可以使事情发生变化
        this.smoothParameters.prediction = predictionValue;                 // 预计未来使用量。 当拍摄太高时可以拍摄
        this.smoothParameters.jitterRadius = jitterRadiusValue;             // 去除抖动的半径的大小。 太高时可以做太多的平滑
        this.smoothParameters.maxDeviationRadius = maxDeviationRadiusValue; // 最大预测半径的大小如果太高，可以回到嘈杂的数据

        // 检查除以零。 使用十分之一毫米的ε
        this.smoothParameters.jitterRadius = Math.Max(0.0001f, this.smoothParameters.jitterRadius);
		
		this.Reset();
        this.init = true;
    }

    /// <summary>
    /// 使用一组Transform SmoothParameters初始化过滤器。
    /// </summary>
    /// <param name="smoothParameters"></param>
	public void Init(KinectInterop.SmoothParameters smoothParameters)
    {
		this.smoothParameters = smoothParameters;
		
        this.Reset();
        this.init = true;
    }

    // Resets the filter to default values.
    /// <summary>
    /// 将过滤器重置为默认值。
    /// </summary>
    public void Reset()
    {
		KinectManager manager = KinectManager.Instance;
		this.history = new FilterDoubleExponentialData[manager.GetSensorBodyCount(), manager.GetSensorJointCount()];
    }

    // Update the filter with a new frame of data and smooth.
    /// <summary>
    /// 使用新的数据帧更新滤镜并平滑。
    /// </summary>
    /// <param name="bodyFrame">身体帧</param>
    public void UpdateFilter(ref KinectInterop.BodyFrameData bodyFrame)
    {
        if (this.init == false)
        {
            this.Init();    // 使用默认参数初始化               
        }

		KinectInterop.SmoothParameters tempSmoothingParams = new KinectInterop.SmoothParameters();

		tempSmoothingParams.smoothing = this.smoothParameters.smoothing;
		tempSmoothingParams.correction = this.smoothParameters.correction;
		tempSmoothingParams.prediction = this.smoothParameters.prediction;

		KinectManager manager = KinectManager.Instance;
		int bodyCount = manager.GetSensorBodyCount();

		for(int bodyIndex = 0; bodyIndex < bodyCount; bodyIndex++)
		{
			if(bodyFrame.bodyData[bodyIndex].bIsTracked != 0)
			{
				FilterBodyJoints(ref bodyFrame.bodyData[bodyIndex], bodyIndex, ref tempSmoothingParams);
			}
		}
	}

    // Update the filter for all body joints
    /// <summary>
    /// 更新所有身体关节的过滤器
    /// </summary>
    /// <param name="bodyData">身体数据</param>
    /// <param name="bodyIndex">身体索引</param>
    /// <param name="tempSmoothingParams">平滑参数</param>
    private void FilterBodyJoints(ref KinectInterop.BodyData bodyData, int bodyIndex, ref KinectInterop.SmoothParameters tempSmoothingParams)
	{
		KinectManager manager = KinectManager.Instance;
		int jointsCount = manager.GetSensorJointCount();

		for(int jointIndex = 0; jointIndex < jointsCount; jointIndex++)
		{
            // If not tracked, we smooth a bit more by using a bigger jitter radius
            // Always filter feet highly as they are so noisy
            //如果没有跟踪，我们通过使用更大的抖动半径来平滑一点总是尽可能高效地过滤脚，因为它们很嘈杂
            if (bodyData.joint[jointIndex].trackingState != KinectInterop.TrackingState.Tracked)
			{
				tempSmoothingParams.jitterRadius = this.smoothParameters.jitterRadius * 2.0f;
				tempSmoothingParams.maxDeviationRadius = smoothParameters.maxDeviationRadius * 2.0f;
			}
			else
			{
				tempSmoothingParams.jitterRadius = smoothParameters.jitterRadius;
				tempSmoothingParams.maxDeviationRadius = smoothParameters.maxDeviationRadius;
			}

			bodyData.joint[jointIndex].position = FilterJoint(bodyData.joint[jointIndex].position, bodyIndex, jointIndex, ref tempSmoothingParams);
		}
		
		for(int jointIndex = 0; jointIndex < jointsCount; jointIndex++)
		{
			if(jointIndex == 0)
			{
				bodyData.position = bodyData.joint[jointIndex].position;
				bodyData.orientation = bodyData.joint[jointIndex].orientation;

				bodyData.joint[jointIndex].direction = Vector3.zero;
			}
			else
			{
				int jParent = (int)manager.GetParentJoint(bodyData.joint[jointIndex].jointType);
				
				if(bodyData.joint[jointIndex].trackingState != KinectInterop.TrackingState.NotTracked && 
				   bodyData.joint[jParent].trackingState != KinectInterop.TrackingState.NotTracked)
				{
					bodyData.joint[jointIndex].direction = 
						bodyData.joint[jointIndex].position - bodyData.joint[jParent].position;
				}
			}
		}
	}

    // Update the filter for one joint
    //更新一个关节的过滤器
    private Vector3 FilterJoint(Vector3 rawPosition, int bodyIndex, int jointIndex, ref KinectInterop.SmoothParameters smoothingParameters)
    {
        Vector3 filteredPosition;
        Vector3 diffVec;
        Vector3 trend;
        float diffVal;

		Vector3 prevFilteredPosition = history[bodyIndex, jointIndex].filteredPosition;
		Vector3 prevTrend = this.history[bodyIndex, jointIndex].trend;
		Vector3 prevRawPosition = this.history[bodyIndex, jointIndex].rawPosition;
        bool jointIsValid = (rawPosition != Vector3.zero);

        // 如果关节无效，请重置过滤器
        if (!jointIsValid)
        {
			history[bodyIndex, jointIndex].frameCount = 0;
        }

        // 初始值
        if (this.history[bodyIndex, jointIndex].frameCount == 0)
        {
            filteredPosition = rawPosition;
            trend = Vector3.zero;
        }
		else if (this.history[bodyIndex, jointIndex].frameCount == 1)
        {
            filteredPosition = (rawPosition + prevRawPosition) * 0.5f;
			diffVec = filteredPosition - prevFilteredPosition;
			trend = (diffVec * smoothingParameters.correction) + (prevTrend * (1.0f - smoothingParameters.correction));
        }
        else
        {
            // 首先应用抖动筛选
            diffVec = rawPosition - prevFilteredPosition;
            diffVal = Math.Abs(diffVec.magnitude);

            if (diffVal <= smoothingParameters.jitterRadius)
            {
                filteredPosition = (rawPosition * (diffVal / smoothingParameters.jitterRadius)) + (prevFilteredPosition * (1.0f - (diffVal / smoothingParameters.jitterRadius)));
            }
            else
            {
                filteredPosition = rawPosition;
            }

            // Now the double exponential smoothing filter
            //现在是双指数平滑滤波器
            filteredPosition = (filteredPosition * (1.0f - smoothingParameters.smoothing)) + ((prevFilteredPosition + prevTrend) * smoothingParameters.smoothing);

            diffVec = filteredPosition - prevFilteredPosition;
            trend = (diffVec * smoothingParameters.correction) + (prevTrend * (1.0f - smoothingParameters.correction));
        }

        // Predict into the future to reduce latency
        //预测未来将减少延迟
        Vector3 predictedPosition = filteredPosition + (trend * smoothingParameters.prediction);

        // Check that we are not too far away from raw data
        //检查我们离原始数据不太远
        diffVec = predictedPosition - rawPosition;
        diffVal = Mathf.Abs(diffVec.magnitude);

        if (diffVal > smoothingParameters.maxDeviationRadius)
        {
            predictedPosition = (predictedPosition * (smoothingParameters.maxDeviationRadius / diffVal)) + (rawPosition * (1.0f - (smoothingParameters.maxDeviationRadius / diffVal)));
        }

        // Save the data from this frame
        //保存此帧中的数据
        history[bodyIndex, jointIndex].rawPosition = rawPosition;
		history[bodyIndex, jointIndex].filteredPosition = filteredPosition;
		history[bodyIndex, jointIndex].trend = trend;
		history[bodyIndex, jointIndex].frameCount++;
        
		return predictedPosition;
    }
	

    // Historical Filter Data.  
    private struct FilterDoubleExponentialData
    {
        // Gets or sets Historical Position.  
        public Vector3 rawPosition;

        // Gets or sets Historical Filtered Position.  
        public Vector3 filteredPosition;

        // Gets or sets Historical Trend.  
        public Vector3 trend;

        // Gets or sets Historical FrameCount.  
        public uint frameCount;
    }
}
