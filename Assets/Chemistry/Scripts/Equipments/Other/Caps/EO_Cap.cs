using UnityEngine;
using MagiCloud.Equipments;
using DG.Tweening;
using MagiCloud.Interactive;

namespace Chemistry.Equipments
{
    /*
    1、盖子分为瓶盖
    2、分为胶头滴管盖子
    3、分为玻璃杯盖子
    4、其他连接设备盖子   
    5、一种是默认就有的，唯一性，另一种是多样性。可靠近多个。   

    1）如果仪器是唯一型，判断交互后的距离检测是否一致，如果不一致，则不允许交互。
    2）如果仪器是交互型，那么其他交互型的盖子可以过来交互，唯一型的不允许交互。
       
    */

    /// <summary>
    /// 盖子
    /// </summary>
    public class EO_Cap : EO_Cover
    {
        public bool IsRotate;

        private Tween _tweenRotate;
        public Vector3 rotateValue;

        public override void OnInitializeEquipment()
        {
            base.OnInitializeEquipment();
            if (IsRotate)
                _tweenRotate = transform.DORotate(rotateValue, 0.3f).SetAutoKill(false).Pause();

        }

        public override void OnDistanceEnter(InteractionEquipment interaction)
        {
            base.OnDistanceEnter(interaction);

            if (_tweenRotate != null)
                _tweenRotate.PlayBackwards();
        }

        /// <summary>
        /// 打开盖子(第一次离开)
        /// </summary>
        public override void OpenCover()
        {
            transform.SetParent(null);
            if (_tweenRotate != null)
                _tweenRotate.PlayForward();
        }

        /// <summary>
        /// 关闭盖子（第一次进入）
        /// </summary>
        public override void CloseCover()
        {

        }
    }
}

