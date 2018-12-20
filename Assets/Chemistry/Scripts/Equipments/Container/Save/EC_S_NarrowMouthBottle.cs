using System;
using UnityEngine;
using MagiCloud.Interactive;
using Chemistry.Data;
using MagiCloud.Equipments;
using Sirenix.OdinInspector;
using Chemistry.Equipments.Actions;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 容器-存储容器-细口瓶
    /// 功能：  I. 1.容积 2.液体
    ///         II.盖子，药品系统
    ///         III.倒水
    /// </summary>
    public class EC_S_NarrowMouthBottle : EC_Save, I_ET_D_BreatheIn
    {
        [SerializeField]
        [LabelText("胶头滴管下落高度")]
        private float breatheHeight = 0.75f;

        /// <summary>
        /// 胶头滴管下落的高度
        /// </summary>
        public float Height { get { return breatheHeight; } }

        public DropperInteractionType InteractionEquipment
        {
            get
            {
                return DropperInteractionType.细口瓶;
            }
        }

        protected override void Start()
        {
            base.Start();
            OnInitializeEquipment();
        }

        public override void OnInitializeEquipment()
        {
            containerType = EContainerType.细口瓶;

            base.OnInitializeEquipment();
        }

        public override bool IsCanInteraction(InteractionEquipment interaction)
        {

            if (!base.IsCanInteraction(interaction)) return false;

            if (!isOpen) return false;

            //滴管
            if (interaction.Equipment is ET_Dropper)
            {
                return true;
            }
            return true;
        }

        public override void OnDistanceRelease(InteractionEquipment interaction)
        {
            base.OnDistanceRelease(interaction);

            if (interaction.Equipment is EO_Cover)
                DropperJoin();
        }

        public override void OnDistanceExit(InteractionEquipment interaction)
        {
            base.OnDistanceExit(interaction);

            if (interaction.Equipment is EO_Cover)
                DropperLeave();
        }

        /// <summary>
        /// 盖子加入
        /// </summary>
        public void DropperJoin()
        {
            //滴管加入后，应该执行一段动画，缓慢的放回去
            CloseCap();
        }

        /// <summary>
        /// 盖子离开
        /// </summary>
        public void DropperLeave()
        {
            //滴灌离开时，应该限制一段距离，然后在离开
            OpenCap();
        }

        public override void OnInitializeEquipment_Editor(string equipmentName)
        {
            base.OnInitializeEquipment_Editor(equipmentName);

            var boxCollider = Collider as BoxCollider;
            boxCollider.center = new Vector3(0.00414f, 0.347f, -0.012f);
            boxCollider.size = new Vector3(0.537f, 0.708f, 0.457f);
        }

        public void OnBreatheIn(float volume)
        {
            ChangeLiquid(-volume);
        }
    }
}
