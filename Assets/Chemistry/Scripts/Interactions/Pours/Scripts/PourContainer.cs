using Chemistry.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关于倒水的容器类
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

    /// <summary>
    /// 容器的模型物体
    /// </summary>
    private Transform containerTra;

    #endregion

    #region 容器包含液体


    /// <summary>
    /// 容器存在的流体总集合
    /// </summary>
    private Dictionary<string, FluidData> dicContainerWhat;

    /// <summary>
    /// 作为倒水容器每一帧的去传递的流体集合
    /// </summary>
    private Dictionary<string, FluidData> dicContainerPreFrameWhat;

    #endregion

    #region 容器包含的距离检测点


    ///// <summary>
    ///// 容器的距离交互点 左边点 和 右边点 里面包含了是否可交互的处理属性
    ///// </summary>
    //private InteractionPourWater interactionPourWater_Left;
    //private InteractionPourWater interactionPourWater_Right;

    #endregion

    #region 其他参数
    /// <summary>
    /// 流体速度流出--暂时无用
    /// </summary>
    private float fluidSpeed = 1;

    /// <summary>
    /// 容器可被旋转的范围 
    /// </summary>
    public Vector2 leftRotateLimits = new Vector2(0, 120);
    public Vector2 rightRotateLimits = new Vector2(-120, 0);
    #endregion



    #region 属性
    public Dictionary<string, FluidData> DicContainerWhat
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

    public Dictionary<string, FluidData> DicContainerPreFrameWhat
    {
        get
        {
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

    float currenttemp;//暂存存放量来防止每次get都执行循环
    public float ContainerCurrentVolume
    {
        get
        {
            if (currenttemp != containerCurrentVolume)
            {
                float sumVolume = 0;
                foreach (var item in dicContainerWhat)
                {
                    sumVolume += item.Value.FluidVolume;
                }
                //sumVolume = containerCurrentVolume;
                containerCurrentVolume = sumVolume;
                return containerCurrentVolume;
            }
            else
            {
                return containerCurrentVolume;
            }
        }

        set
        {
            containerCurrentVolume = value;
        }
    }

    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="tra">模型本身</param>
    /// <param name="dicContainerWhat"></param>
    public PourContainer(Transform tra, float maxVolume, Dictionary<string, FluidData> dicContainerWhat)
    {
        this.containerTra = tra;
        this.ContainerMaxVolume = maxVolume;
        this.DicContainerWhat = dicContainerWhat;
    }


    #region 液体量的处理 帧处理

    /// <summary>
    /// 倒出液体计算每一帧的流体值
    /// </summary>
    /// <param name="interactionPourWater">倒水杯那一边的倒水距离检测点</param>
    public void PourOut(InteractionPourWater interactionPourWater)
    {
        //这个地方的返回值是否有问题？？？？//TODO---
        float outValume = RotateAngleToReduce(interactionPourWater);

        //Dictionary<string, FluidData> temp = null;
        dicContainerPreFrameWhat.Clear();

        foreach (var item in dicContainerWhat)
        {
            //计算每个液体占每一帧的量
            float x = (item.Value.FluidVolume / containerCurrentVolume) * outValume;
            //添加到临时字典中
            dicContainerPreFrameWhat.Add(item.Key, new FluidData(item.Key, x, item.Value.FluidDensity));
        }
    }


    /// <summary>
    /// 传入的是倒水杯子的容器类
    /// </summary>
    /// <param name="pourContainer">倒水杯子的容器对象</param>
    public void ReceiveIn(PourContainer pourContainer)
    {
        //做流体的接受处理 有相同的液体的合并处理 不相同的做增加处理
        foreach (var item in pourContainer.DicContainerPreFrameWhat)
        {
            if (DicContainerWhat.ContainsKey(item.Key))
            {
                //有则在原对应流体加上这一帧的量
                DicContainerWhat[item.Key].FluidVolume += item.Value.FluidVolume;
            }
            else
            {
                //若没有则再新加一种流体
                DicContainerWhat.Add(item.Key, item.Value);
            }
        }
    }

    #endregion


    #region 物体的旋转和液量对应处理

    float currentAngel = 0;
    float lastAngel = 0;
    float offsetAngel = 0;
    float angelToVolumeRate = 1;//角度转流体量量的转化率

    /// <summary>
    /// 返回的是 在满足可倒出液体的角度下 当每次角度提高一点就将这一点转化为浮点数用于去计算倒出的液体总量
    /// </summary>
    /// <returns></returns>
    float RotateAngleToReduce(InteractionPourWater interactionPourWater)
    {
        if (!ISCanPourOut(interactionPourWater)) return 0;
        currentAngel = containerTra.localRotation.eulerAngles.z;
        offsetAngel = currentAngel - lastAngel;
        lastAngel = currentAngel;
        return Mathf.Abs(offsetAngel);
    }

    /// <summary>
    /// 能否再倒出来 依据现有的量去计算 可以倾倒的最小角度 在与现在的物体的倾斜角对比 
    /// </summary>
    /// <param name="obj">杯子容器模型</param>
    /// <param name="interactionPourWater">倒水杯那一边的倒水距离检测点</param>
    /// <returns></returns>
    bool ISCanPourOut(InteractionPourWater interactionPourWater)
    {
        switch (interactionPourWater.pointSide)
        {
            case PourPointSide.Left://采用(0,120)度限制杯子可倾斜角度
                float minangleLeft = EquationsOfTwoUnknowns(ContainerCurrentVolume, new Vector2(0, leftRotateLimits.y), new Vector2(containerMaxVolume, 0));

                if (containerTra.localRotation.eulerAngles.z > minangleLeft)
                    return true;
                else
                    return false;
            case PourPointSide.Right://采用(-120,0)度限制
                float minangleRight = EquationsOfTwoUnknowns(ContainerCurrentVolume, new Vector2(0, rightRotateLimits.y), new Vector2(containerMaxVolume, 0));

                if (containerTra.localRotation.eulerAngles.z < minangleRight)

                    return true;
                else
                    return false;
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
