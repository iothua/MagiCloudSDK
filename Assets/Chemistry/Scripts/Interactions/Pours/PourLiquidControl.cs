using Chemistry.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chemistry.Equipments;
using Chemistry.Chemicals;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using MagiCloud.Interactive.Distance;
using System.Linq;

/*
 *  倒水功能挂载脚本
 *  
 *  功能
 *  1 事件处理中心 提供出所有倒水过程中事件 
 *  2 旋转 动作 和限制角度
 *  3 距离检测 点触发相关对象的方法事件
 *  4 流体数据 与药品系统对接数据
 *  5 接口 给出可在实验中自动进行倒水的动作展示
 *   
 *  >>类<<
 *  容器类 
 *  流体类 (空气 液体) 
 *  倒水系统提供的单独的事件类
 *  
 *  >>考虑到点的问题<<
 *  1(目前的) 杯子 2个接水点 左右2点 分为2个距离检测点
 *  
 *  (未加入)2 多个接水或者倒水点  建议以倒水接水点 为对象 去处理
 *  
 *   
 *  >>事件<<
 *  1 靠近 Close 都通知一次  2 卡入点 Connect 通知一次 3开始传输 Transport 持续通知 4 断开接触 DisConnect 5 离开 Leave
 *   
 *  >>吸附设计<<
 *  1 编辑下生成吸附点的父子结构 
 *  2 两点需要手动调整位置  
 *  3 每次靠近 需要读条去吸附 延时吸附
 *  4 吸附之后 开启限制性可旋转 开启容器对象双方倒出量和接收量的方法
 *  
 *  >>脱离设计<<
 *  1 两种脱离方式 
 *      A 在倒得时候直接放手  放手杯子在接水杯附近
 *      B 在倒的时候强行拽出脱离 杯子在手中
 *  
 *  >>数据处理<<
 *  1 在容器对象中存储当前杯子液体种类和对应的量
 *  2 在传输过程中在每一帧按照已有的比例把液体字典传过去
 *  
 *  >>左右手的区分<<
 *  1 确定只能左边或者右边倒液体
 *  2 左手操作只能 在接水杯子的左边操作 右手操作只能在接水杯的右边操作
 *  
 *  >>涉及UI提示<<
 *  1 靠近接水点 会有一个读条操作
 *  
 *  
 *  >>预知问题<<
 *  1 2个手同时操作向一个杯子加入液体 现只允许一个对一个
 *  2 当容器有多个点 可接受 时
 *    
 */

/// <summary>
/// 倒水液体控制
/// 开发者：阮榆皓
/// </summary>

[DefaultExecutionOrder(300)]
[ExecuteInEditMode]
public class PourLiquidControl : MonoBehaviour
{

    //容器初始化
    public PourContainer pourContainer;

    [HideInInspector]
    public EC_Container CurContainer;

    [HideInInspector]
    public Transform tra;


    #region 编辑器调用
    [LabelText("倒水点交互名称")]
    public string pourPtName = "倒水交互";

    [LabelText("倒水点类型")]
    public PourPointSide curPourType = PourPointSide.Left;

    public List<InteractionPourWater> interactionLst = new List<InteractionPourWater>();

    [ButtonGroup]
    [Button("刷新")]
    public void GetPourPt()
    {
        interactionLst.Clear();

        interactionLst = transform.GetComponentsInChildren<InteractionPourWater>().ToList();
    }

    [ButtonGroup]
    [Button("创建倒水检测点")]
    public void OnCreate()
    {
        string tmpType = "";
        switch (curPourType)
        {
            case PourPointSide.Left:
                tmpType = "左";
                break;
            case PourPointSide.Right:
                tmpType = "右";
                break;
            default:
                break;
        }
        GameObject pourPtObj = new GameObject("pourObj_" + pourPtName + "_" + tmpType);
        var pourObj = pourPtObj.AddComponent<InteractionPourWater>();
        pourPtObj.transform.SetParent(FindDistanceParent());
        pourPtObj.transform.localPosition = Vector3.zero;

        pourObj.pointSide = curPourType;
        pourObj.distanceData.TagID = pourPtName;
        pourObj.distanceData.interactionType = InteractionType.Pour;
        pourObj.distanceData.detectType = InteractionDetectType.And;
        pourObj.distanceData.IsOnly = true;

        interactionLst.Add(pourObj);
    }


    //void CreatePourInteraction<T>(Transform parent) where T : DistanceInteraction
    //{
    //    GameObject distanceObject = new GameObject("distanceObject_" + distanceName);
    //    var distance = distanceObject.AddComponent<T>();
    //    distanceObject.transform.SetParent(parent);
    //    distanceObject.transform.localPosition = Vector3.zero;

    //    distances.Add(distance);

    //    Selection.activeGameObject = distanceObject;

    //    distance.distanceData.TagID = distanceName;
    //}

    Transform FindDistanceParent()
    {
        Transform parent = this.transform.Find("distanceParent");
        if (parent == null)
        {
            parent = new GameObject("distanceParent").transform;
            parent.SetParent(this.transform);
            parent.localPosition = Vector3.zero;
            parent.localRotation = Quaternion.identity;
            parent.localScale = Vector3.one;
        }

        return parent;
    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            for (int i = 0; i < interactionLst.Count; i++)
            {
                interactionLst[i].gameObject.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
        {
            for (int i = 0; i < interactionLst.Count; i++)
            {
                interactionLst[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        //Destroy
        //DestroyImmediate

        if (!Application.isPlaying)
        {
            for (int i = 0; i < interactionLst.Count; i++)
            {
                DestroyImmediate(interactionLst[i].gameObject);
            }
        }
    }


    #endregion

    //public float maxValume;



    //此处应从容器中获取药品信息--暂不处理//TODO--
    //[SerializeField]
    //public FluidData[] fluidData;
    

    Dictionary<string, float> dictionaryCup;


    //距离检测相关
    InteractionPourWater interactionPourWater_Left;
    InteractionPourWater interactionPourWater_Right;


    private int valoue = 100;
    public bool IsCanInteraction(PourLiquidControl pourLiquid)
    {



        return false;
    }


    // Use this for initialization
    void Start()
    {
        if (!Application.isPlaying) return;

        //容器trasfrom
        tra = this.transform;

        CurContainer = this.GetComponent<EC_Container>();


        dictionaryCup = new Dictionary<string, float>();

        //Debug.Log("PourLiquidControl 的Start被调用");

        //液体初始化
        //此处应遍历容器中的所有药品，然后添加至改集合中--可扩展TODO
        //倒水容器类与这个类---有时间还能优化

        float tmpV = CurContainer.DrugSystemIns.GetDrug(CurContainer.DrugName).Volume;

        dictionaryCup.Add(CurContainer.DrugName, tmpV);

        //for (int i = 0; i < fluidData.Length; i++)
        //{
        //    if (!dictionaryCup.ContainsKey(fluidData[i].FluidName))
        //    {
        //        dictionaryCup.Add(fluidData[i].FluidName, fluidData[i]);
        //    }
        //}

        //产生容器对象
        pourContainer = new PourContainer(tra, CurContainer.Volume, dictionaryCup);

        foreach (var item in interactionLst)
        {
            switch (item.pointSide)
            {
                case PourPointSide.Left:
                    {
                        interactionPourWater_Left = item;
                        item.pourContainer = this.pourContainer;
                        if (true)
                        {

                        }
                        //interactionPourWater_Left.OnEnterDistance.AddListener();

                        break;
                    }
                case PourPointSide.Right:
                    {
                        interactionPourWater_Right = item;
                        item.pourContainer = this.pourContainer;
                        break;
                    }
                default:
                    break;
            }
        }

        /*
        //pourContainer = new PourContainer(tra, 100.0f, dictionaryCup);
        //两个距离检测点的初始化 并把容器对象给两个距离检测点
        //if (transform.GetComponentsInChildren<InteractionPourWater>().Length == 2)
        //{

        //foreach (var item in transform.GetComponentsInChildren<InteractionPourWater>())
        //{
        //    switch (item.pointSide)
        //    {
        //        case PourPointSide.Left:
        //        { 
        //            interactionPourWater_Left = item;
        //            item.pourContainer = this.pourContainer;
        //            leftNum++;
        //            //interactionPourWater_Left.OnEnterDistance.AddListener();

        //            break;
        //        }
        //    case PourPointSide.Right:
        //        {
        //            rightNum++;
        //            interactionPourWater_Right = item;
        //            item.pourContainer = this.pourContainer;
        //            break;
        //        }
        //    default:
        //            break;
        //    }
        //}
        //}
        //else
        //{
        //    Debug.LogError("未找到2个倒水的距离检测点,请重新配置");
        //}
        */
    }



}
