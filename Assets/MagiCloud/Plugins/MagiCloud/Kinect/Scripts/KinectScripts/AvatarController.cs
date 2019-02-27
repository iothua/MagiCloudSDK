using UnityEngine;
//using Windows.Kinect;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text; 

/// <summary>
/// Avatar控制
/// </summary>
[RequireComponent(typeof(Animator))]
public class AvatarController : MonoBehaviour
{
    // The index of the player, whose movements the model represents. Default 0 (first player)
    /// <summary>
    /// 玩家的索引，模型代表的动作。默认0(第一位)
    /// </summary>
    public int playerIndex = 0;

    // Bool that has the characters (facing the player) actions become mirrored. Default false.
    //具有角色的Bool(面对玩家)的行为会被镜像。默认的错误
    /// <summary>
    /// 具有角色的Bool(面对玩家)的行为会被镜像。默认的错误
    /// </summary>
    public bool mirroredMovement = false;

    // Bool that determines whether the avatar is allowed to do vertical movement
    /// <summary>
    /// 是否允许avatar 做垂直运动
    /// </summary>
    public bool verticalMovement = false;

    // Rate at which avatar will move through the scene.
    /// <summary>
    /// avatar 在场景中移动的速度。
    /// </summary>
    private int moveRate = 1;

    // Slerp smooth factor
    /// <summary>
    /// Slerp光滑因子
    /// </summary>
    public float smoothFactor = 5f;

    #region 注释

    //	// Public variables that will get matched to bones. If empty, the Kinect will simply not track it.
    //	public Transform HipCenter;
    //	public Transform Spine;
    //	public Transform ShoulderCenter;
    //	public Transform Neck;
    //	public Transform Head;
    //
    //	public Transform ClavicleLeft;
    //	public Transform ShoulderLeft;
    //	public Transform ElbowLeft; 
    //	public Transform HandLeft;
    //	public Transform FingersLeft;
    //	private Transform FingerTipsLeft = null;
    //	private Transform ThumbLeft = null;
    //
    //	public Transform ClavicleRight;
    //	public Transform ShoulderRight;
    //	public Transform ElbowRight;
    //	public Transform HandRight;
    //	public Transform FingersRight;
    //	private Transform FingerTipsRight = null;
    //	private Transform ThumbRight = null;
    //	
    //	public Transform HipLeft;
    //	public Transform KneeLeft;
    //	public Transform FootLeft;
    //	private Transform ToesLeft = null;
    //	
    //	public Transform HipRight;
    //	public Transform KneeRight;
    //	public Transform FootRight;
    //	private Transform ToesRight = null;
    #endregion

    /// <summary>
    /// 身体根
    /// </summary>
    private Transform bodyRoot;

    // A required variable if you want to rotate the model in space.
    /// <summary>
    /// 如果想要在空间中旋转模型，需要一个变量
    /// </summary>
    private GameObject offsetNode;

    // Variable to hold all them bones. It will initialize the same size as initialRotations.
    /// <summary>
    /// 可变的，能容纳所有的骨头。它将初始化与初始化相同的大小。
    /// </summary>
    private Transform[] bones;

    // Rotations of the bones when the Kinect tracking starts.
    /// <summary>
    /// Kinect跟踪开始时骨骼的旋转。
    /// </summary>
    private Quaternion[] initialRotations;
    /// <summary>
    /// Kinect跟踪开始时骨骼的局部旋转。
    /// </summary>
	private Quaternion[] initialLocalRotations;

    // Directions of the bones when the Kinect tracking starts.
    //Kinect跟踪开始时的骨骼方向。
    //private Vector3[] initialDirections;

    // Initial position and rotation of the transform
    /// <summary>
    /// 初始位置
    /// </summary>
    private Vector3 initialPosition;
    /// <summary>
    /// 初始旋转
    /// </summary>
	private Quaternion initialRotation;

    // Calibration Offset Variables for Character Position.
    /// <summary>
    /// 字符位置的校准偏移变量。
    /// </summary>
    private bool OffsetCalibrated = false;

	private float XOffset, YOffset, ZOffset;
    //private Quaternion originalRotation;

    // private instance of the KinectManager
    //KinectManager的私有实例
    private KinectManager kinectManager;

    // transform caching gives performance boost since Unity calls GetComponent<Transform>() each time you call transform 
    //转换缓存提供性能提升，因为Unity每次调用转换时调用GetComponent < transform >()
    private Transform _transformCache;
	public new Transform transform
	{
		get
		{
			if (!_transformCache) _transformCache = base.transform;
			return _transformCache;
		}
	}


	public void Awake()
    {
        // check for double start
        //检查双开始
        if (bones != null)
			return;

        // inits the bones array
        //init骨头数组
        bones = new Transform[27];

        // Initial rotations and directions of the bones.
        //最初的旋转和骨骼的方向
        initialRotations = new Quaternion[bones.Length];
		initialLocalRotations = new Quaternion[bones.Length];
        //initialDirections = new Vector3[bones.Length];

        // make OffsetNode as a parent of model transform. Just to not do it every time
        //将OffsetNode作为模型转换的父节点。只是不要每次都这么做
        offsetNode = new GameObject(name + "Ctrl") { layer = transform.gameObject.layer, tag = transform.gameObject.tag };
		offsetNode.transform.position = transform.position;
		offsetNode.transform.rotation = transform.rotation;

		transform.parent = offsetNode.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		
		bodyRoot = transform;

        // Map bones to the points the Kinect tracks
        //将骨头映射到Kinect的轨迹
        MapBones();

        // Get initial bone rotations
        //得到初始骨旋转
        GetInitialRotations();
	}

    // Update the avatar each frame.
    /// <summary>
    /// 每个帧更新头像
    /// </summary>
    /// <param name="UserID"></param>
    public void UpdateAvatar(Int64 UserID)
    {	
		if(!transform.gameObject.activeInHierarchy) 
			return;

        // Get the KinectManager instance
        //得到KinectManager实例
        if (kinectManager == null)
		{
			kinectManager = KinectManager.Instance;
		}

        // move the avatar to its Kinect position
        //将化身移动到它的Kinect位置
        MoveAvatar(UserID);

        #region 注释

        //		bool flipJoint = !mirroredMovement;
        //		
        //		// Update Head, Neck, Spine, and Hips
        //		TransformBone(UserID, KinectInterop.JointType.SpineBase, 0, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.SpineMid, 1, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.SpineShoulder, 2, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.Neck, 3, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.Head, 4, flipJoint);
        //		
        //		// Beyond this, switch the arms and legs.
        //		
        //		// Left Arm --> Right Arm
        //		TransformSpecialBone(UserID, KinectInterop.JointType.ShoulderLeft, KinectInterop.JointType.SpineShoulder, !mirroredMovement ? 25 : 26, Vector3.left, flipJoint);  // clavicle left
        //		TransformBone(UserID, KinectInterop.JointType.ShoulderLeft, !mirroredMovement ? 5 : 11, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.ElbowLeft, !mirroredMovement ? 6 : 12, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.WristLeft, !mirroredMovement ? 7 : 13, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.HandLeft, !mirroredMovement ? 8 : 14, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.HandTipLeft, !mirroredMovement ? 9 : 15, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.ThumbLeft, !mirroredMovement ? 10 : 16, flipJoint);
        //
        //		// Right Arm --> Left Arm
        //		TransformSpecialBone(UserID, KinectInterop.JointType.ShoulderRight, KinectInterop.JointType.SpineShoulder, !mirroredMovement ? 26 : 25, Vector3.right, flipJoint);  // clavicle right
        //		TransformBone(UserID, KinectInterop.JointType.ShoulderRight, !mirroredMovement ? 11 : 5, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.ElbowRight, !mirroredMovement ? 12 : 6, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.WristRight, !mirroredMovement ? 13 : 7, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.HandRight, !mirroredMovement ? 14 : 8, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.HandTipRight, !mirroredMovement ? 15 : 9, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.ThumbRight, !mirroredMovement ? 16 : 10, flipJoint);
        //
        //		// Left Leg --> Right Leg
        //		TransformBone(UserID, KinectInterop.JointType.HipLeft, !mirroredMovement ? 17 : 21, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.KneeLeft, !mirroredMovement ? 18 : 22, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.AnkleLeft, !mirroredMovement ? 19 : 23, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.FootLeft, !mirroredMovement ? 20 : 24, flipJoint);
        //
        //		// Right Leg --> Left Leg
        //		TransformBone(UserID, KinectInterop.JointType.HipRight, !mirroredMovement ? 21 : 17, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.KneeRight, !mirroredMovement ? 22 : 18, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.AnkleRight, !mirroredMovement ? 23 : 19, flipJoint);
        //		TransformBone(UserID, KinectInterop.JointType.FootRight, !mirroredMovement ? 24 : 20, flipJoint);	

        #endregion

        for (var boneIndex = 0; boneIndex < bones.Length; boneIndex++)
		{
			if (!bones[boneIndex]) 
				continue;

			if(boneIndex2JointMap.ContainsKey(boneIndex))
			{
				KinectInterop.JointType joint = !mirroredMovement ? boneIndex2JointMap[boneIndex] : boneIndex2MirrorJointMap[boneIndex];
				TransformBone(UserID, joint, boneIndex, !mirroredMovement);
			}
			else if(specIndex2JointMap.ContainsKey(boneIndex))
			{
                // special bones (clavicles)
                //特殊的骨头(锁骨)
                List<KinectInterop.JointType> alJoints = !mirroredMovement ? specIndex2JointMap[boneIndex] : specIndex2MirrorJointMap[boneIndex];

				if(alJoints.Count >= 2)
				{
					//Debug.Log(alJoints[0].ToString());
					Vector3 baseDir = alJoints[0].ToString().EndsWith("Left") ? Vector3.left : Vector3.right;
					TransformSpecialBone(UserID, alJoints[0], alJoints[1], boneIndex, baseDir, !mirroredMovement);
				}
			}
		}
	}

    // Set bones to their initial positions and rotations.
    /// <summary>
    /// 把骨头固定在他们的初始位置和旋转。
    /// </summary>
    public void ResetToInitialPosition()
    {	
		if(bones == null)
			return;
		
		if(offsetNode != null)
		{
			offsetNode.transform.rotation = Quaternion.identity;
		}
		else
		{
			transform.rotation = Quaternion.identity;
		}

        //transform.position = initialPosition;
        //transform.rotation = initialRotation;

        // For each bone that was defined, reset to initial position.
        //对于定义的每根骨头，重新设置为初始位置
        for (int pass = 0; pass < 2; pass++)  // 2 passes because clavicles are at the end//2超过，因为锁骨在最后
        {
			for(int i = 0; i < bones.Length; i++)
			{
				if(bones[i] != null)
				{
					bones[i].rotation = initialRotations[i];
				}
			}
		}

		if(bodyRoot != null)
		{
			bodyRoot.localPosition = Vector3.zero;
			bodyRoot.localRotation = Quaternion.identity;
		}

        // Restore the offset's position and rotation
        //恢复偏移位置的位置和旋转
        if (offsetNode != null)
		{
			offsetNode.transform.position = initialPosition;
			offsetNode.transform.rotation = initialRotation;
		}
		else
		{
			transform.position = initialPosition;
			transform.rotation = initialRotation;
		}
    }

    // Invoked on the successful calibration of a player.
    /// <summary>
    /// 在一个玩家的成功校准上被调用
    /// </summary>
    /// <param name="userId">用户ID</param>
    public void SuccessfulCalibration(Int64 userId)
	{
        // reset the models position
        //重置模型位置
        if (offsetNode != null)
		{
			offsetNode.transform.rotation = initialRotation;
		}
		else
		{
			transform.rotation = initialRotation;
		}
		
		// re-calibrate the position offset
        //重新校准位置偏差
		OffsetCalibrated = false;
	}

    // Apply the rotations tracked by kinect to the joints.
    /// <summary>
    /// 将kinect追踪到关节处。
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="joint">关节</param>
    /// <param name="boneIndex">骨骼索引</param>
    /// <param name="flip">是否镜像</param>
    void TransformBone(Int64 userId, KinectInterop.JointType joint, int boneIndex, bool flip)
    {
		Transform boneTransform = bones[boneIndex];
		if(boneTransform == null || kinectManager == null)
			return;
		
		int iJoint = (int)joint;
		if(iJoint < 0 || !kinectManager.IsJointTracked(userId, iJoint))
			return;

        // Get Kinect joint orientation
        //得到Kinect联合定位
        Quaternion jointRotation = kinectManager.GetJointOrientation(userId, iJoint, flip);
		if(jointRotation == Quaternion.identity)
			return;

        // Smoothly transition to the new rotation
        //平稳过渡到新的旋转
        Quaternion newRotation = Kinect2AvatarRot(jointRotation, boneIndex);

		if(smoothFactor != 0f)
        	boneTransform.rotation = Quaternion.Slerp(boneTransform.rotation, newRotation, smoothFactor * Time.deltaTime);
		else
			boneTransform.rotation = newRotation;
	}

    // Apply the rotations tracked by kinect to a special joint
    /// <summary>
    /// 将kinect追踪到一个特殊的关节
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="joint">关节</param>
    /// <param name="jointParent">父关节</param>
    /// <param name="boneIndex">骨骼索引</param>
    /// <param name="baseDir">距离</param>
    /// <param name="flip">是否镜像</param>
    void TransformSpecialBone(Int64 userId, KinectInterop.JointType joint, KinectInterop.JointType jointParent, int boneIndex, Vector3 baseDir, bool flip)
	{
		Transform boneTransform = bones[boneIndex];
		if(boneTransform == null || kinectManager == null)
			return;
		
		if(!kinectManager.IsJointTracked(userId, (int)joint) || 
		   !kinectManager.IsJointTracked(userId, (int)jointParent))
		{
			return;
		}
		
		Vector3 jointDir = kinectManager.GetJointDirection(userId, (int)joint, false, true);
		Quaternion jointRotation = Quaternion.FromToRotation(baseDir, jointDir);
		
		if(!flip)
		{
			Vector3 mirroredAngles = jointRotation.eulerAngles;
			mirroredAngles.y = -mirroredAngles.y;
			mirroredAngles.z = -mirroredAngles.z;

			jointRotation = Quaternion.Euler(mirroredAngles);
		}
		
		if(jointRotation != Quaternion.identity)
		{
            // Smoothly transition to the new rotation
            //平稳过渡到新的旋转
            Quaternion newRotation = Kinect2AvatarRot(jointRotation, boneIndex);
			
			if(smoothFactor != 0f)
				boneTransform.rotation = Quaternion.Slerp(boneTransform.rotation, newRotation, smoothFactor * Time.deltaTime);
			else
				boneTransform.rotation = newRotation;
		}
		
	}

    // Moves the avatar in 3D space - pulls the tracked position of the user and applies it to root.
    /// <summary>
    /// 在3D空间中移动化身-拉出用户的跟踪位置并将其应用到根
    /// </summary>
    /// <param name="UserID"></param>
    void MoveAvatar(Int64 UserID)
	{
		if(!kinectManager || !kinectManager.IsJointTracked(UserID, (int)KinectInterop.JointType.SpineBase))
			return;

        // Get the position of the body and store it.
        //获得身体的位置并存储它。
        Vector3 trans = kinectManager.GetUserPosition(UserID);

        // If this is the first time we're moving the avatar, set the offset. Otherwise ignore it.
        //如果这是我们第一次移动avatar，设置偏移量。否则忽略它
        if (!OffsetCalibrated)
		{
			OffsetCalibrated = true;
			
			XOffset = !mirroredMovement ? trans.x * moveRate : -trans.x * moveRate;
			YOffset = trans.y * moveRate;
			ZOffset = -trans.z * moveRate;
		}

        // Smoothly transition to the new position
        //顺利过渡到新位置
        Vector3 targetPos = Kinect2AvatarPos(trans, verticalMovement);

		if(bodyRoot != null)
		{
			bodyRoot.localPosition = smoothFactor != 0f ? 
				Vector3.Lerp(bodyRoot.localPosition, targetPos, smoothFactor * Time.deltaTime) : targetPos;
		}
		else
		{
			transform.localPosition = smoothFactor != 0f ? 
				Vector3.Lerp(transform.localPosition, targetPos, smoothFactor * Time.deltaTime) : targetPos;
		}
	}

    // If the bones to be mapped have been declared, map that bone to the model.
    /// <summary>
    /// 如果要绘制的骨头已经被声明，那就把骨头映射到模型上
    /// </summary>
    void MapBones()
	{
        #region 注释

        //		bones[0] = HipCenter;
        //		bones[1] = Spine;
        //		bones[2] = ShoulderCenter;
        //		bones[3] = Neck;
        //		bones[4] = Head;
        //	
        //		bones[5] = ShoulderLeft;
        //		bones[6] = ElbowLeft;
        //		bones[7] = HandLeft;
        //		bones[8] = FingersLeft;
        //		bones[9] = FingerTipsLeft;
        //		bones[10] = ThumbLeft;
        //	
        //		bones[11] = ShoulderRight;
        //		bones[12] = ElbowRight;
        //		bones[13] = HandRight;
        //		bones[14] = FingersRight;
        //		bones[15] = FingerTipsRight;
        //		bones[16] = ThumbRight;
        //	
        //		bones[17] = HipLeft;
        //		bones[18] = KneeLeft;
        //		bones[19] = FootLeft;
        //		bones[20] = ToesLeft;
        //	
        //		bones[21] = HipRight;
        //		bones[22] = KneeRight;
        //		bones[23] = FootRight;
        //		bones[24] = ToesRight;
        //
        //		// special bones
        //		bones[25] = ClavicleLeft;
        //		bones[26] = ClavicleRight;

        #endregion

        var animatorComponent = GetComponent<Animator>();
		
		for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
		{
			if (!boneIndex2MecanimMap.ContainsKey(boneIndex)) 
				continue;
			
			//_bones[kinectInt] = ClosestMecanimBoneByKinectId(kinectId, animatorComponent);
			bones[boneIndex] = animatorComponent.GetBoneTransform(boneIndex2MecanimMap[boneIndex]);
		}
	}

    // Capture the initial rotations of the bones
    /// <summary>
    /// 捕捉骨骼的最初旋转
    /// </summary>
    void GetInitialRotations()
	{
        // save the initial rotation
        //保存最初的旋转
        if (offsetNode != null)
		{
			initialPosition = offsetNode.transform.position;
			initialRotation = offsetNode.transform.rotation;

			offsetNode.transform.rotation = Quaternion.identity;
		}
		else
		{
			initialPosition = transform.position;
			initialRotation = transform.rotation;

			transform.rotation = Quaternion.identity;
		}

		for (int i = 0; i < bones.Length; i++)
		{
			if (bones[i] != null)
			{
				initialRotations[i] = bones[i].rotation; // * Quaternion.Inverse(initialRotation);
				initialLocalRotations[i] = bones[i].localRotation;
			}
		}

        // Restore the initial rotation
        //恢复最初的旋转
        if (offsetNode != null)
		{
			offsetNode.transform.rotation = initialRotation;
		}
		else
		{
			transform.rotation = initialRotation;
		}
	}

    // Converts kinect joint rotation to avatar joint rotation, depending on joint initial rotation and offset rotation
    /// <summary>
    /// 将kinect关节旋转转换为avatar关节旋转，取决于关节的初始旋转和偏置旋转
    /// </summary>
    /// <param name="jointRotation">关节旋转</param>
    /// <param name="boneIndex">骨骼索引</param>
    /// <returns></returns>
    Quaternion Kinect2AvatarRot(Quaternion jointRotation, int boneIndex)
	{
		Quaternion newRotation = jointRotation * initialRotations[boneIndex];

		if (offsetNode != null)
		{
			Vector3 totalRotation = newRotation.eulerAngles + offsetNode.transform.rotation.eulerAngles;
			newRotation = Quaternion.Euler(totalRotation);
		}
		
		return newRotation;
	}

    // Converts Kinect position to avatar skeleton position, depending on initial position, mirroring and move rate
    /// <summary>
    /// 将Kinect位置转换为avatar骨架位置，取决于初始位置，镜像和移动速度
    /// </summary>
    /// <param name="jointPosition">关节位置</param>
    /// <param name="bMoveVertically">垂直移动</param>
    /// <returns></returns>
    Vector3 Kinect2AvatarPos(Vector3 jointPosition, bool bMoveVertically)
	{
		float xPos;

		if(!mirroredMovement)
			xPos = jointPosition.x * moveRate - XOffset;
		else
			xPos = -jointPosition.x * moveRate - XOffset;
		
		float yPos = jointPosition.y * moveRate - YOffset;
		float zPos = -jointPosition.z * moveRate - ZOffset;
		
		Vector3 newPosition = new Vector3(xPos, bMoveVertically ? yPos : 0f, zPos);

		return newPosition;
	}

    // dictionaries to speed up bones' processing
    // the author of the terrific idea for kinect-joints to mecanim-bones mapping
    // along with its initial implementation, including following dictionary is
    // Mikhail Korchun (korchoon@gmail.com). Big thanks to this guy!
    /// <summary>
    /// 词典加速骨骼处理作者的理想关键点，以及其初始实现的包括以下字典在内的Kinect-Joint的作者：Mikhail Korchun（korchoon@gmail.com）。 
    /// 非常感谢这个家伙！
    /// </summary>
    private readonly Dictionary<int, HumanBodyBones> boneIndex2MecanimMap = new Dictionary<int, HumanBodyBones>
	{
		{0, HumanBodyBones.Hips},
		{1, HumanBodyBones.Spine},
//        {2, HumanBodyBones.Chest},
		{3, HumanBodyBones.Neck},
		{4, HumanBodyBones.Head},
		
		{5, HumanBodyBones.LeftUpperArm},
		{6, HumanBodyBones.LeftLowerArm},
		{7, HumanBodyBones.LeftHand},
		{8, HumanBodyBones.LeftIndexProximal},
		
		{9, HumanBodyBones.LeftIndexIntermediate},
		{10, HumanBodyBones.LeftThumbProximal},
		
		{11, HumanBodyBones.RightUpperArm},
		{12, HumanBodyBones.RightLowerArm},
		{13, HumanBodyBones.RightHand},
		{14, HumanBodyBones.RightIndexProximal},
		
		{15, HumanBodyBones.RightIndexIntermediate},
		{16, HumanBodyBones.RightThumbProximal},
		
		{17, HumanBodyBones.LeftUpperLeg},
		{18, HumanBodyBones.LeftLowerLeg},
		{19, HumanBodyBones.LeftFoot},
		{20, HumanBodyBones.LeftToes},
		
		{21, HumanBodyBones.RightUpperLeg},
		{22, HumanBodyBones.RightLowerLeg},
		{23, HumanBodyBones.RightFoot},
		{24, HumanBodyBones.RightToes},
		
		{25, HumanBodyBones.LeftShoulder},
		{26, HumanBodyBones.RightShoulder},
	};
	
    /// <summary>
    /// 骨骼指数关节图
    /// </summary>
	private readonly Dictionary<int, KinectInterop.JointType> boneIndex2JointMap = new Dictionary<int, KinectInterop.JointType>
	{
		{0, KinectInterop.JointType.SpineBase},
		{1, KinectInterop.JointType.SpineMid},
		{2, KinectInterop.JointType.SpineShoulder},
		{3, KinectInterop.JointType.Neck},
		{4, KinectInterop.JointType.Head},
		
		{5, KinectInterop.JointType.ShoulderLeft},
		{6, KinectInterop.JointType.ElbowLeft},
		{7, KinectInterop.JointType.WristLeft},
		{8, KinectInterop.JointType.HandLeft},
		
		{9, KinectInterop.JointType.HandTipLeft},
		{10, KinectInterop.JointType.ThumbLeft},
		
		{11, KinectInterop.JointType.ShoulderRight},
		{12, KinectInterop.JointType.ElbowRight},
		{13, KinectInterop.JointType.WristRight},
		{14, KinectInterop.JointType.HandRight},
		
		{15, KinectInterop.JointType.HandTipRight},
		{16, KinectInterop.JointType.ThumbRight},
		
		{17, KinectInterop.JointType.HipLeft},
		{18, KinectInterop.JointType.KneeLeft},
		{19, KinectInterop.JointType.AnkleLeft},
		{20, KinectInterop.JointType.FootLeft},
		
		{21, KinectInterop.JointType.HipRight},
		{22, KinectInterop.JointType.KneeRight},
		{23, KinectInterop.JointType.AnkleRight},
		{24, KinectInterop.JointType.FootRight},
	};
	
    /// <summary>
    /// 规则索引关节图
    /// </summary>
	private readonly Dictionary<int, List<KinectInterop.JointType>> specIndex2JointMap = new Dictionary<int, List<KinectInterop.JointType>>
	{
		{25, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderLeft, KinectInterop.JointType.SpineShoulder} },
		{26, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderRight, KinectInterop.JointType.SpineShoulder} },
	};
	
    /// <summary>
    /// 骨骼指数镜面图
    /// </summary>
	private readonly Dictionary<int, KinectInterop.JointType> boneIndex2MirrorJointMap = new Dictionary<int, KinectInterop.JointType>
	{
		{0, KinectInterop.JointType.SpineBase},
		{1, KinectInterop.JointType.SpineMid},
		{2, KinectInterop.JointType.SpineShoulder},
		{3, KinectInterop.JointType.Neck},
		{4, KinectInterop.JointType.Head},
		
		{5, KinectInterop.JointType.ShoulderRight},
		{6, KinectInterop.JointType.ElbowRight},
		{7, KinectInterop.JointType.WristRight},
		{8, KinectInterop.JointType.HandRight},
		
		{9, KinectInterop.JointType.HandTipRight},
		{10, KinectInterop.JointType.ThumbRight},
		
		{11, KinectInterop.JointType.ShoulderLeft},
		{12, KinectInterop.JointType.ElbowLeft},
		{13, KinectInterop.JointType.WristLeft},
		{14, KinectInterop.JointType.HandLeft},
		
		{15, KinectInterop.JointType.HandTipLeft},
		{16, KinectInterop.JointType.ThumbLeft},
		
		{17, KinectInterop.JointType.HipRight},
		{18, KinectInterop.JointType.KneeRight},
		{19, KinectInterop.JointType.AnkleRight},
		{20, KinectInterop.JointType.FootRight},
		
		{21, KinectInterop.JointType.HipLeft},
		{22, KinectInterop.JointType.KneeLeft},
		{23, KinectInterop.JointType.AnkleLeft},
		{24, KinectInterop.JointType.FootLeft},
	};
	
    /// <summary>
    /// 规格指数镜面图
    /// </summary>
	private readonly Dictionary<int, List<KinectInterop.JointType>> specIndex2MirrorJointMap = new Dictionary<int, List<KinectInterop.JointType>>
	{
		{25, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderRight, KinectInterop.JointType.SpineShoulder} },
		{26, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderLeft, KinectInterop.JointType.SpineShoulder} },
	};
	
}

