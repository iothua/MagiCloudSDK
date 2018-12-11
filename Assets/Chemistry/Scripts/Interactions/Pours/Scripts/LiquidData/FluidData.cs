using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 流体数据类
/// </summary>
[System.Serializable]
public class FluidData
{
    #region 字段
    /// <summary>
    /// 流体的名字
    /// </summary>
    public string fluidName;

    /// <summary>
    /// 流体的体积
    /// </summary>
    public float fluidVolume;

    /// <summary>
    /// 流体的密度  会参与到倒水时的流动速度计算
    /// </summary>
    public float fluidDensity;

    #endregion

    #region 属性
    public string FluidName
    {
        get
        {
            return fluidName;
        }

        set
        {
            fluidName = value;
        }
    }

    public float FluidVolume
    {
        get
        {
            return fluidVolume;
        }

        set
        {
            fluidVolume = value;
        }
    }

    public float FluidDensity
    {
        get
        {
            return fluidDensity;
        }

        set
        {
            fluidDensity = value;
        }
    }
    #endregion


    /// <summary>
    /// 流体构造函数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="volume"></param>
    /// <param name="density"></param>
    public FluidData(string name, float volume, float density)
    {
        this.fluidName = name;
        this.fluidVolume = volume;
        this.fluidDensity = density;
    }
}
