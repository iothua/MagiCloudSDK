using MagiCloud.Features;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 滑动的仪器接口
    /// </summary>
    public interface ISlideEquipment
    {
        MCLimitMove LimitMove { get; }
       
    }
}
