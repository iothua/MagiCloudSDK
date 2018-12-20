using Chemistry.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// 关于倒水的容器类--此容器类主要功能用于计算倒水量
/// 
/// 1 当前被子液体集合 
/// 2 每帧接受和倒出的处理 
/// 3 容器能否可接 能否可倒的处理
/// 4 量的处理根据角度 倒水杯中发生液体流失 给接水杯 每一帧的
/// </summary>
public class PourContainer
{
    #region 容器本身

    /// <summary>
    /// 总容积 固定值
    /// </summary>
    private float containerMaxVolume;

    /// <summary>
    /// 当前的量
    /// </summary>
    private float containerCurrentVolume;

    private float startLiquidV;

    /// <summary>
    /// 容器的模型物体
    /// </summary>
    private Transform containerTra;

    #endregion

    #region 容器包含液体


    /// <summary>
    /// 容器存在的流体总集合
    /// </summary>
    private Dictionary<string, float> dicContainerWhat;

    /// <summary>
    /// 作为倒水容器每一帧的去传递的流体集合
    /// </summary>
    private Dictionary<string, float> dicContainerPreFrameWhat;

    #endregion

    #region 容器包含的距离检测点


    ///// <summary>
    ///// 容器的距离交互点 左边点 和 右边点 里面包含了是否可交互的处理属性
    ///// </summary>
    //private InteractionPourWater interactionPourWater_Left;
    //private InteractionPourWater interactionPourWater_Right;

    #endregion

    /// <summary>
    /// 流体速度流出--暂时无用
    /// </summary>
    //private float fluidSpeed = 1;

    /// <summary>
    /// 容器可被旋转的范围 
    /// </summary>
    public Vector2 leftRotateLimits = new Vector2(0, 120);
    public Vector2 rightRotateLimits = new Vector2(-120, 0);


    //[Header("当前容器对应的类")]
    //public EC_Container CurContainer;

    #region 属性
    public Dictionary<string, float> DicContainerWhat
    {
        get
        {
            return dicContainerWhat;
        }

        set
        {
            dicContainerWhat = value;
        }
    }

    public Dictionary<string, float> DicContainerPreFrameWhat
    {
        get
        {
            if (dicContainerPreFrameWhat==null)
            {
                dicContainerPreFrameWhat = new Dictionary<string, float>();
            }
            return dicContainerPreFrameWhat;
        }

        set
        {
            dicContainerPreFrameWhat = value;
        }
    }

    public float ContainerMaxVolume
    {
        get
        {
            return containerMaxVolume;
        }

        set
        {
            containerMaxVolume = value;
        }
    }

    bool hasChanged = false;//改变过才重新遍历，提高效率
    public float ContainerCurrentVolume
    {
        get
        {
            if (hasChanged)
            {
                hasChanged = false;
                containerCurrentVolume = 0;

                foreach (var item in dicContainerWhat)
                {
                    containerCurrentVolume += item.Value;
                }

                return containerCurrentVolume;
            }
            else
            {
                return containerCurrentVolume;
            }
        }

        //set--不能直接设置总量，需单独设置每个药品量
        //{
        //    //直接设置当前量--混合物待扩展
        //    containerCurrentVolume = value;
        //}
    }

    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="tra">模型本身</param>
    /// <param name="dicContainerWhat"></param>
    public PourContainer(Transform tra, float maxVolume, Dictionary<string, float> dicContainerWhat)
    {
        this.containerTra = tra;
        this.ContainerMaxVolume = maxVolume;
        this.DicContainerWhat = dicContainerWhat;

        hasChanged = true;//先保证至少遍历一次
        startLiquidV = ContainerCurrentVolume;
    }


    #region 液体量的处理 帧处理

    /// <summary>
    /// 倒出液体计算每一帧的流体值
    /// </summary>
    /// <param name="interactionPourWater">倒水杯那一边的倒水距离检测点</param>
    public float PourOut(InteractionPourWater interactionPourWater)
    {
        
        float outValume = RotateAngleToReduce(interactionPourWater);

        //Debug.Log("旋转角度差为：" + outValume);
        //Dictionary<string, FluidData> temp = null;
        if (outValume>= interactionPourWater.pourContainer.ContainerCurrentVolume)
        {
            outValume = interactionPourWater.pourContainer.ContainerCurrentVolume;
        }
       
        DicContainerPreFrameWhat.Clear();

        if (outValume <= 0.001f)
        {
            return 0.0f;
        }

        float sumV = 0.0f;
        foreach (var item in DicContainerWhat)
        {
            //计算每个液体占每一帧的量
            float tmpV = (item.Value / ContainerCurrentVolume) * outValume;
            Debug.Log("倒水   " + item.Value + "/" + ContainerCurrentVolume + "结果" + tmpV);
            //添加到临时字典中
            //dicContainerPreFrameWhat.Add(item.Key, new FluidData(item.Key, x, item.Value.FluidDensity));
            DicContainerPreFrameWhat.Add(item.Key, tmpV);
            sumV += tmpV;
        }
        
        //for (int i = 0; i < tmpLst.Count; i++)
        //{
        //    tmpLst[i]
        //}

        //可优化，目前只考虑纯净物
        DicContainerWhat[DicContainerWhat.First().Key] -= sumV;

        return sumV;
    }


    /// <summary>
    /// 传入的是倒水杯子的容器类
    /// </summary>
    /// <param name="pourContainer">倒水杯子的容器对象</param>
    public float ReceiveIn(PourContainer pourContainer)
    {
        float sumV = 0.0f;
        //做流体的接受处理 有相同的液体的合并处理 不相同的做增加处理
        foreach (var item in pourContainer.DicContainerPreFrameWhat)
        {
            if (DicContainerWhat.ContainsKey(item.Key))
            {
                //有则在原对应流体加上这一帧的量
                DicContainerWhat[item.Key] += item.Value;
                sumV += item.Value;
            }
            else
            {
                //若没有则再新加一种流体
                DicContainerWhat.Add(item.Key, item.Value);
                sumV += item.Value;
            }
        }
        if (sumV>0.01f)
        {
            hasChanged = true;
        }
        Debug.Log("接水   " + ContainerCurrentVolume + "结果");

        return sumV;
    }

    #endregion


    #region 物体的旋转和液量对应处理

    float currentAngel = 0;
    //float lastAngel = 0;
    //float offsetAngel = 0;
    //float angelToVolumeRate = 1;//角度转流体量量的转化率

    /// <summary>
    /// 返回的是 在满足可倒出液体的角度下 当每次角度提高一点就将这一点转化为浮点数用于去计算倒出的液体总量
    /// </summary>
    /// <returns></returns>
    float RotateAngleToReduce(InteractionPourWater interactionPourWater)
    {
        //若当前帧的角度无法倒出水，返回0
        if (!ISCanPourOut(interactionPourWater)) return 0;

        //currentAngel= interactionPourWater

        //Debug.Log("当前旋转度为" + containerTra.parent.eulerAngles);
        currentAngel = containerTra.parent.eulerAngles.z;

        if (interactionPourWater.pointSide == PourPointSide.Right)//-120-0
        {
            currentAngel = -currentAngel;
        }

        //速率的系数--几倍速
        int modulus = Mathf.CeilToInt((currentAngel - 90.0f) / 10.0f);
        float resultV = Mathf.Clamp(modulus, 1, 4)* interactionPourWater.pourHelper.WaterSpeed;
        
        Debug.Log("resultV" + resultV);

        return resultV;
        //offsetAngel = currentAngel - lastAngel;
        //lastAngel = currentAngel;

        //return Mathf.Abs(offsetAngel);
    }

    /// <summary>
    /// 能否再倒出来 依据现有的量去计算 可以倾倒的最小角度 在与现在的物体的倾斜角对比 
    /// </summary>
    /// <param name="interactionPourWater">倒水杯那一边的倒水距离检测点-----修改为自己倒水点的左右来判断</param>
    /// <returns></returns>
    public bool ISCanPourOut(InteractionPourWater interactionPourWater)
    {
        //没有了也不能倒水
        if (interactionPourWater.pourContainer.ContainerCurrentVolume<=0.1f)
        {
            Debug.Log("容器" + interactionPourWater.transform.parent.parent.name + "已空，不能倒水");
            return false;
        }

        //Debug.Log(interactionPourWater.FeaturesObjectController.name + "正在判断能否倒出水");
        switch (interactionPourWater.pointSide)
        {
            case PourPointSide.Left://采用(0,120)度限制杯子可倾斜角度
                float minangleLeft = EquationsOfTwoUnknowns(ContainerCurrentVolume, new Vector2(0, leftRotateLimits.y), new Vector2(containerMaxVolume, 0));

                if (minangleLeft>90.0f)
                {
                    minangleLeft = 90.0f;
                }
                //Debug.Log(interactionPourWater.FeaturesObjectController.transform.parent.name + "是能否倒出水的旋转中心");
                if (interactionPourWater.FeaturesObjectController.transform.parent.eulerAngles.z > minangleLeft)
                {
                    //Debug.Log("能倒出水");
                    hasChanged = true;
                    return true;
                }
                else
                {
                    Debug.Log("不能倒出水");
                    return false;
                }
            case PourPointSide.Right://采用(-120,0)度限制
                //Debug.Log("当前量---" + ContainerCurrentVolume + "容器容积---" + containerMaxVolume);
                float minangleRight = EquationsOfTwoUnknowns(ContainerCurrentVolume, new Vector2(0, rightRotateLimits.x), new Vector2(containerMaxVolume, 0));

                //if (containerTra.localRotation.eulerAngles.z < minangleRight)
                //{ return true; }
                //else
                //{ return false; }

                if (minangleRight<-90.0f)
                {
                    minangleRight = -90.0f;
                }
                //Debug.Log(interactionPourWater.FeaturesObjectController.transform.parent.name + "是能否倒出水的旋转中心" + "临界角度为--" + minangleRight);
                float tmpAngle = interactionPourWater.FeaturesObjectController.transform.parent.eulerAngles.z;
                if (tmpAngle > 0.0f)
                {
                    tmpAngle -= 360.0f;
                }
                if (tmpAngle < minangleRight)
                {
                    hasChanged = true;
                    return true;
                }
                else
                { return false; }
            default:
                return false;
        }
    }





    #endregion

    #region 数学工具
    /// <summary>
    ///  二元一次方程求值
    /// </summary>
    /// <param name="_x1y1"></param>
    /// <param name="_x2y2"></param>
    /// <param name="x">传入值</param>
    /// <returns></returns>
    private float EquationsOfTwoUnknowns(float x, Vector2 _x1y1, Vector2 _x2y2)
    {
        Vector2 ab = new Vector2((_x1y1.y - _x2y2.y) / (_x1y1.x - _x2y2.x), (_x1y1.x * _x2y2.y - _x2y2.x * _x1y1.y) / (_x1y1.x - _x2y2.x));
        return x * ab.x + ab.y;
    }
    #endregion


}
