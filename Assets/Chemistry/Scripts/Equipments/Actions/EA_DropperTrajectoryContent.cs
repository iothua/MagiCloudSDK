using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chemistry.Data;
using MagiCloud.Equipments;
using DG.Tweening;
using Chemistry.Equipments;
using System;

namespace Chemistry.Equipments.Actions
{
    public class EA_DropperTrajectoryContent
    {
        /// <summary>
        /// 滴管吸药动画合集
        /// </summary>
        public EA_DropperTrajectoryContent(EquipmentBase equipmentBase, I_ET_D_BreatheIn i_ET_D_BreatheIn, Action<I_ET_D_BreatheIn> onCompleteAction)
        {
            switch (i_ET_D_BreatheIn.InteractionEquipment)
            {
                case DropperInteractionType.细口瓶:
                    equipmentBase.transform.DOLocalMoveY(equipmentBase.transform.localPosition.y - i_ET_D_BreatheIn.Height, 0.5f).OnComplete(()=> onCompleteAction.Invoke(i_ET_D_BreatheIn));
                    break;
                case DropperInteractionType.锥形瓶:
                    break;
                case DropperInteractionType.集气瓶:
                    break;
                case DropperInteractionType.烧杯:
                    break;
                case DropperInteractionType.试管:
                    break;
                case DropperInteractionType.蒸发皿:
                    break;
                case DropperInteractionType.量筒:
                    break;
                case DropperInteractionType.玻璃杯:
                    break;
                case DropperInteractionType.培养皿:
                    break;
                case DropperInteractionType.广口瓶:
                    break;
                default:
                    equipmentBase.transform.DOLocalMoveY(equipmentBase.transform.localPosition.y - i_ET_D_BreatheIn.Height, 0.5f).OnComplete(() => onCompleteAction.Invoke(i_ET_D_BreatheIn));
                    break;
            }
        }
        /// <summary>
        /// 滴管滴药动画合集
        /// </summary>
        public EA_DropperTrajectoryContent(EquipmentBase equipmentBase, I_ET_D_Drip i_ET_D_Drip, Action<I_ET_D_Drip> onCompleteAction)
        {
            switch (i_ET_D_Drip.InteractionEquipment)
            {
                case DropperInteractionType.细口瓶:
                    break;
                case DropperInteractionType.锥形瓶:
                    break;
                case DropperInteractionType.集气瓶:
                    break;
                case DropperInteractionType.烧杯:
                    equipmentBase.transform.DOLocalMoveY(equipmentBase.transform.localPosition.y - i_ET_D_Drip.ClampPutHeight, 0.5f).OnComplete(() => onCompleteAction.Invoke(i_ET_D_Drip));
                    break;
                case DropperInteractionType.试管:
                    break;
                case DropperInteractionType.蒸发皿:
                    break;
                case DropperInteractionType.量筒:
                    break;
                case DropperInteractionType.玻璃杯:
                    break;
                case DropperInteractionType.培养皿:
                    break;
                case DropperInteractionType.广口瓶:
                    break;
                default:
                    equipmentBase.transform.DOLocalMoveY(equipmentBase.transform.localPosition.y - i_ET_D_Drip.ClampPutHeight, 0.5f).OnComplete(() => onCompleteAction.Invoke(i_ET_D_Drip));
                    break;
            }
        }
    } 
}
