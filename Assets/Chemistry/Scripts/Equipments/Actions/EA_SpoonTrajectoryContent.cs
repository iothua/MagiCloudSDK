using Chemistry.Data;
using DG.Tweening;
using MagiCloud.Equipments;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chemistry.Equipments.Actions
{
    public class EA_SpoonTrajectoryContent
    {
        /// <summary>
        /// 药匙取药动画合集
        /// </summary>
        public EA_SpoonTrajectoryContent(EquipmentBase equipmentBase, I_ET_S_SpoonTake i_ET_S_SpoonTake, Transform receive_point, Action<I_ET_S_SpoonTake> onCompleteAction)
        {
            Vector3 initPos = equipmentBase.transform.position;
            Vector3 initRot = equipmentBase.transform.eulerAngles;
            Sequence sequence = DOTween.Sequence();
            switch (i_ET_S_SpoonTake.InteractionEquipment)
            {
                case DropperInteractionType.细口瓶:
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
                    sequence.Append(equipmentBase.transform.DOLocalMoveY(receive_point.localPosition.y - i_ET_S_SpoonTake.Height, 0.5f));
                    sequence.Append(equipmentBase.transform.DOLocalRotate(new Vector3(20, 90, 0), 1).OnComplete(() => onCompleteAction.Invoke(i_ET_S_SpoonTake)));
                    sequence.AppendInterval(0.5f);

                    sequence.Append(equipmentBase.transform.DOLocalMoveY(receive_point.localPosition.y + 1, 0.5f));
                    sequence.Join(equipmentBase.transform.DOLocalRotate(new Vector3(90, 90, 0), 1));

                    break;
                default:
                    sequence.Append(equipmentBase.transform.DOLocalMoveY(receive_point.localPosition.y - i_ET_S_SpoonTake.Height, 0.5f).OnComplete(() => onCompleteAction.Invoke(i_ET_S_SpoonTake)));
                    sequence.AppendInterval(0.5f);
                    sequence.Append(equipmentBase.transform.DOLocalMoveY(equipmentBase.transform.localPosition.y - i_ET_S_SpoonTake.Height, 0.5f));
                    break;
            }
        }
        /// <summary>
        /// 药匙放药动画合集
        /// </summary>
        public EA_SpoonTrajectoryContent(EquipmentBase equipmentBase, I_ET_S_SpoonPut i_ET_S_SpoonPut, Transform receive_point, Action<I_ET_S_SpoonPut> onCompleteAction)
        {
            Sequence sequence = DOTween.Sequence();
            switch (i_ET_S_SpoonPut.InteractionEquipment)
            {
                case DropperInteractionType.细口瓶:
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
                    sequence.Append(equipmentBase.transform.DOLocalMoveY(receive_point.localPosition.y - i_ET_S_SpoonPut.SpoonPutHeight, 0.5f));
                    sequence.Join(equipmentBase.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.5f).OnComplete(() => onCompleteAction.Invoke(i_ET_S_SpoonPut)));
                    sequence.AppendInterval(0.5f);

                    sequence.Append(equipmentBase.transform.DOLocalMoveY(receive_point.localPosition.y + 1, 0.5f));
                    sequence.Join(equipmentBase.transform.DOLocalRotate(new Vector3(90, 90, 0), 1));
                    break;
                case DropperInteractionType.培养皿:
                    break;
                case DropperInteractionType.广口瓶:
                    break;
                default:
                    sequence.Append(equipmentBase.transform.DOLocalMoveY(receive_point.localPosition.y - i_ET_S_SpoonPut.SpoonPutHeight, 0.5f).OnComplete(() => onCompleteAction.Invoke(i_ET_S_SpoonPut)));
                    sequence.AppendInterval(0.5f);
                    sequence.Append(equipmentBase.transform.DOLocalMoveY(equipmentBase.transform.localPosition.y - i_ET_S_SpoonPut.SpoonPutHeight, 0.5f));
                    break;
            }
        }
    } 
}
