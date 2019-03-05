using UnityEngine;
//using Windows.Kinect;

using System;
using System.Collections;

/// <summary>
/// 骨骼控制
/// </summary>
public class CubemanController : MonoBehaviour
{
    public bool MoveVertically = false;     //垂直移动
    public bool MirroredMovement = false;   //镜像移动

    //public GameObject debugText;
    #region	骨骼绑定的物体

	/// <summary>
	/// 髋关节中心
	/// </summary>
    public GameObject Hip_Center;
	/// <summary>
	/// 脊椎骨
	/// </summary>
    public GameObject Spine;

    public GameObject Neck;
	/// <summary>
	/// 头
	/// </summary>
    public GameObject Head;
	/// <summary>
	/// 左肩
	/// </summary>
    public GameObject Shoulder_Left;
	/// <summary>
	/// 左手肘
	/// </summary>
    public GameObject Elbow_Left;
	/// <summary>
	/// 左手腕
	/// </summary>
    public GameObject Wrist_Left;
	/// <summary>
	/// 左手
	/// </summary>
    public GameObject Hand_Left;
	/// <summary>
	/// 右肩
	/// </summary>
    public GameObject Shoulder_Right;
	/// <summary>
	/// 右手肘
	/// </summary>
    public GameObject Elbow_Right;
	/// <summary>
	/// 右手腕
	/// </summary>
    public GameObject Wrist_Right;
	/// <summary>
	/// 右手
	/// </summary>
    public GameObject Hand_Right;		
	/// <summary>
	/// 左臀部
	/// </summary>
    public GameObject Hip_Left;			
	/// <summary>
	/// 左膝关节
	/// </summary>
    public GameObject Knee_Left;		
	/// <summary>
	/// 左脚裸
	/// </summary>
    public GameObject Ankle_Left;		
	/// <summary>
	/// 左脚
	/// </summary>
    public GameObject Foot_Left;		
	/// <summary>
	/// 右臀部
	/// </summary>
    public GameObject Hip_Right;		
	/// <summary>
	/// 右膝关节
	/// </summary>
    public GameObject Knee_Right;		
	/// <summary>
	/// 右脚裸
	/// </summary>
    public GameObject Ankle_Right;		
	/// <summary>
	/// 右脚
	/// </summary>
    public GameObject Foot_Right;		
	/// <summary>
	/// 脊柱肩
	/// </summary>
    public GameObject Spine_Shoulder;	
	/// <summary>
	/// 左手指尖
	/// </summary>
    public GameObject Hand_Tip_Left;	
	/// <summary>
	/// 左拇指
	/// </summary>
    public GameObject Thumb_Left;		
	/// <summary>
	/// 右手指尖
	/// </summary>
    public GameObject Hand_Tip_Right;	
	/// <summary>
	/// 右拇指
	/// </summary>
    public GameObject Thumb_Right;		
    #endregion

    /// <summary>
    /// 线预制物体
    /// </summary>
    public LineRenderer LinePrefab;
    //	public LineRenderer DebugLine;
    /// <summary>
    /// 骨骼
    /// </summary>
    private GameObject[] bones;
    /// <summary>
    /// 线集合，用于连接各个部位
    /// </summary>
    private LineRenderer[] lines;

    private LineRenderer lineTLeft;
    private LineRenderer lineTRight;
    private LineRenderer lineFLeft;
    private LineRenderer lineFRight;

    private Vector3 initialPosition;    //初始位置
    private Quaternion initialRotation; //初始旋转值
    private Vector3 initialPosOffset = Vector3.zero;    //初始位置偏移量
    private Int64 initialPosUserID = 0;                 //初始用户ID


    void Start()
    {
        //store bones in a list for easier access
        //将骨骼存储在列表中以便于访问
        bones = new GameObject[] {
            Hip_Center,
            Spine,
            Neck,
            Head,
            Shoulder_Left,
            Elbow_Left,
            Wrist_Left,
            Hand_Left,
            Shoulder_Right,
            Elbow_Right,
            Wrist_Right,
            Hand_Right,
            Hip_Left,
            Knee_Left,
            Ankle_Left,
            Foot_Left,
            Hip_Right,
            Knee_Right,
            Ankle_Right,
            Foot_Right,
            Spine_Shoulder,
            Hand_Tip_Left,
            Thumb_Left,
            Hand_Tip_Right,
            Thumb_Right
        };

        // array holding the skeleton lines
        //实例化一个数组，用于连接骨骼
        lines = new LineRenderer[bones.Length];

		//生成线模型
        if (LinePrefab)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Instantiate(LinePrefab) as LineRenderer;
            }
        }

        //		if(DebugLine)
        //		{
        //			lineTLeft = Instantiate(DebugLine) as LineRenderer;
        //			lineTRight = Instantiate(DebugLine) as LineRenderer;
        //		}
        //
        //		if(LinePrefab)
        //		{
        //			lineFLeft = Instantiate(LinePrefab) as LineRenderer;
        //			lineFRight = Instantiate(LinePrefab) as LineRenderer;
        //		}

        //设置初始位置
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }


    void Update()
    {
        //获取KinectManager静态实例
        KinectManager manager = KinectManager.Instance;

        // get 1st player
        //获取到摄像头照射到的人物ID
        Int64 userID = manager ? manager.GetPrimaryUserID() : 0;

        if (userID <= 0)
        {
            // reset the pointman position and rotation
            //重置点人位置和旋转
            if (transform.position != initialPosition)
                transform.position = initialPosition;

            if (transform.rotation != initialRotation)
                transform.rotation = initialRotation;

            //显示骨骼线
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].gameObject.SetActive(true);

                bones[i].transform.localPosition = Vector3.zero;
                bones[i].transform.localRotation = Quaternion.identity;

                if (LinePrefab)
                {
                    lines[i].gameObject.SetActive(false);
                }
            }

            return;
        }

        // set the position in space
        //设置空间的位置
        Vector3 posPointMan = manager.GetUserPosition(userID);
        //根据镜像运行来设置z轴的方向
        posPointMan.z = !MirroredMovement ? -posPointMan.z : posPointMan.z;

        // store the initial position
        // 存储初始位置 如果当前用户ID与实时获取到的ID不一致，则重新赋值
        if (initialPosUserID != userID)
        {
            initialPosUserID = userID;
            //设置偏移量
            initialPosOffset = transform.position - (MoveVertically ? posPointMan : new Vector3(posPointMan.x, 0, posPointMan.z));
        }
        //设置位置
        transform.position = initialPosOffset + (MoveVertically ? posPointMan : new Vector3(posPointMan.x, 0, posPointMan.z));

        // update the local positions of the bones
        // 更新骨骼的本地位置
        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                int joint = (int)manager.GetJointAtIndex(
                    !MirroredMovement ? i : (int)KinectInterop.GetMirrorJoint((KinectInterop.JointType)i));
                if (joint < 0)
                    continue;

                if (manager.IsJointTracked(userID, joint))
                {
                    bones[i].gameObject.SetActive(true);

                    Vector3 posJoint = manager.GetJointPosition(userID, joint);
                    posJoint.z = !MirroredMovement ? -posJoint.z : posJoint.z;

                    Quaternion rotJoint = manager.GetJointOrientation(userID, joint, !MirroredMovement);

                    posJoint -= posPointMan;

                    if (MirroredMovement)
                    {
                        posJoint.x = -posJoint.x;
                        posJoint.z = -posJoint.z;
                    }

                    bones[i].transform.localPosition = posJoint;
                    bones[i].transform.localRotation = rotJoint;

                    if (LinePrefab)
                    {
                        lines[i].gameObject.SetActive(true);
                        Vector3 posJoint2 = bones[i].transform.position;

                        Vector3 dirFromParent = manager.GetJointDirection(userID, joint, false, false);
                        dirFromParent.z = !MirroredMovement ? -dirFromParent.z : dirFromParent.z;
                        Vector3 posParent = posJoint2 - dirFromParent;

                        //lines[i].SetVertexCount(2);
                        lines[i].SetPosition(0, posParent);
                        lines[i].SetPosition(1, posJoint2);
                    }
                    #region 注释

                    //					KinectInterop.BodyData bodyData = manager.GetUserBodyData(userID);
                    //					if(lineTLeft != null && bodyData.liTrackingID != 0 && joint == (int)JointType.HandLeft)
                    //					{
                    //						Vector3 leftTDir = bodyData.leftThumbDirection.normalized;
                    //						leftTDir.z = !MirroredMovement ? -leftTDir.z : leftTDir.z;
                    //
                    //						Vector3 posTStart = bones[i].transform.position;
                    //						Vector3 posTEnd = posTStart + leftTDir;
                    //
                    //						lineTLeft.SetPosition(0, posTStart);
                    //						lineTLeft.SetPosition(1, posTEnd);
                    //
                    //						if(lineFLeft != null)
                    //						{
                    //							Vector3 leftFDir = bodyData.leftThumbForward.normalized;
                    //							leftFDir.z = !MirroredMovement ? -leftFDir.z : leftFDir.z;
                    //							
                    //							Vector3 posFStart = bones[i].transform.position;
                    //							Vector3 posFEnd = posTStart + leftFDir;
                    //							
                    //							lineFLeft.SetPosition(0, posFStart);
                    //							lineFLeft.SetPosition(1, posFEnd);
                    //						}
                    //					}
                    //					
                    //					if(lineTRight != null && bodyData.liTrackingID != 0 && joint == (int)JointType.HandRight)
                    //					{
                    //						Vector3 rightTDir = bodyData.rightThumbDirection.normalized;
                    //						rightTDir.z = !MirroredMovement ? -rightTDir.z : rightTDir.z;
                    //						
                    //						Vector3 posTStart = bones[i].transform.position;
                    //						Vector3 posTEnd = posTStart + rightTDir;
                    //						
                    //						lineTRight.SetPosition(0, posTStart);
                    //						lineTRight.SetPosition(1, posTEnd);
                    //						
                    //						if(lineFRight != null)
                    //						{
                    //							Vector3 rightFDir = bodyData.rightThumbForward.normalized;
                    //							rightFDir.z = !MirroredMovement ? -rightFDir.z : rightFDir.z;
                    //							
                    //							Vector3 posFStart = bones[i].transform.position;
                    //							Vector3 posFEnd = posTStart + rightFDir;
                    //							
                    //							lineFRight.SetPosition(0, posFStart);
                    //							lineFRight.SetPosition(1, posFEnd);
                    //						}
                    //					}
                    #endregion

                }
                else
                {
                    bones[i].gameObject.SetActive(false);

                    if (LinePrefab)
                    {
                        lines[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

}
