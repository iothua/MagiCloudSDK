using Chemistry.Equipments.Actions;
using MagiCloud.Equipments;
using System;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 仪器扩展方法
    /// </summary>
    public static class EA_Expansion
    {
        /// <summary>
        /// 处理仪器动作
        /// </summary>
        /// <param name="equipmentBase"></param>
        /// <param name="onStart"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static EA_Handle DoEquipmentHandle(this EquipmentBase equipmentBase, Action onStart = default(Action), float duration = 1.0f)
        {
            EA_Handle handle = new EA_Handle(equipmentBase, onStart, duration);
            return handle;
        }
    }
}
