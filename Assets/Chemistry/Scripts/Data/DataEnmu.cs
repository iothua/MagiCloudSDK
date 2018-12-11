namespace Chemistry.Data
{
    /// <summary>
    /// 火焰状态
    /// </summary>
    public enum TriggerFireState
    {
        初始,
        灭,
        火星,
        燃烧,
        旺
    }

    /// <summary>
    /// 距离检测类型
    /// </summary>
    public enum DistanceType
    {
        D3D = 0,
        DScreen = 1
    }

    /// <summary>
    /// 计量单位
    /// </summary>
    public enum EMeasureUnit
    {
        /// <summary>
        /// 毫升
        /// </summary>
        ml = 0,

        /// <summary>
        /// 克
        /// </summary>
        g = 1
    }

    /// <summary>
    /// 药品类型
    /// </summary>
    public enum EDrugType
    {
        /// <summary>
        /// 无意义的
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 液体
        /// </summary>
        Liquid = 1,

        /// <summary>
        /// 固体（块状，不会消失，催化剂、铁钉...）
        /// </summary>
        Solid = 2,

        /// <summary>
        /// 固体粉末（粉末，可以溶解的，盐，碳酸钠粉末...）
        /// </summary>
        Solid_Powder = 3,

        /// <summary>
        /// 气体
        /// </summary>
        Gas = 4,

        /// <summary>
        /// 溶液（氯化钠溶液等，只有一种溶质的溶液，多种的不算）
        /// </summary>
        Solution = 5
    }

    /// <summary>
    /// 容器类型
    /// </summary>
    public enum EContainerType
    {
        
        细口瓶,
        锥形瓶,
        集气瓶,
        烧杯,
        试管,
        蒸发皿,
        量筒,
        玻璃杯,
        培养皿,
        广口瓶,
        酒精灯,
        火柴,
        火柴盒,
        木条,
        玻璃棒,
        铁架台,
        三脚架,
        漏斗,
        滤纸,
        药匙,
        镊子,
        坩埚钳,
    }

    /// <summary>
    /// 反应条件
    /// </summary>
    public enum EReactionCondition
    {
        /// <summary>
        /// 加热
        /// </summary>
        Warm = 0,

        /// <summary>
        /// 高温
        /// </summary>
        Heat = 1
    }

    /// <summary>
    /// 触发检测触发器类型
    /// </summary>
    public enum ETriggerType
    {
        /// <summary>
        /// 火
        /// </summary>
        Fire = 0,

        /// <summary>
        /// 氧气
        /// </summary>
        Oxygen,

        /// <summary>
        /// 二氧化碳
        /// </summary>
        CarbonDioxide,

        /// <summary>
        /// 水
        /// </summary>
        Water,

        /// <summary>
        /// 氢气
        /// </summary>
        Hydrogen

    }
}