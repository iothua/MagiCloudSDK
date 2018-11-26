using UnityEngine;
using MagiCloud.Interactive.Distance;
using MagiCloud.Equipments;
using MagiCloud.Interactive.Actions;

namespace MagiCloud.Interactive
{
    public class InteractionEquipment : DistanceInteraction,IExternalInteraction
    {
        public EquipmentBase Equipment;

        private IInteraction_Limit[] Limits;

        protected override void OnEnable()
        {
            base.OnEnable();

            Limits = gameObject.GetComponentsInChildren<IInteraction_Limit>();

            ExternalInteraction = this;
        }

        public override bool IsCanInteraction(DistanceInteraction distanceInteraction)
        {
            if (Equipment == null) return true;

            if (!(distanceInteraction.ExternalInteraction is InteractionEquipment)) return false;

            bool result = Equipment.IsCanInteraction((InteractionEquipment)distanceInteraction.ExternalInteraction);

            foreach (var item in Limits)
            {
                item.IsLimit = !result;
            }

            return result;
        }

        public override void OnDistanceEnter(DistanceInteraction distanceInteraction)
        {
            if (Equipment == null) return;

            //if (!IsCanInteraction(distanceInteraction)) return;

            base.OnDistanceEnter(distanceInteraction);

            Equipment.OnDistanceEnter((InteractionEquipment)distanceInteraction.ExternalInteraction);
        }

        public override void OnDistanceStay(DistanceInteraction distanceInteraction)
        {
            if (Equipment == null) return;

            //if (!IsCanInteraction(distanceInteraction)) return;

            base.OnDistanceStay(distanceInteraction);

            Equipment.OnDistanceStay((InteractionEquipment)distanceInteraction.ExternalInteraction);
        }

        public override void OnDistanceExit(DistanceInteraction distanceInteraction)
        {
            if (Equipment == null) return;

            if (!(distanceInteraction.ExternalInteraction is InteractionEquipment)) return;

            base.OnDistanceExit(distanceInteraction);

            Equipment.OnDistanceExit((InteractionEquipment)distanceInteraction.ExternalInteraction);
        }

        public override void OnDistanceRelesae(DistanceInteraction distanceInteraction)
        {
            if (Equipment == null) return;
            //if (!IsCanInteraction(distanceInteraction)) return;

            base.OnDistanceRelesae(distanceInteraction);

            Equipment.OnDistanceRelease((InteractionEquipment)distanceInteraction.ExternalInteraction);
        }

        public override void OnDistanceRelease(DistanceInteraction distanceInteraction, InteractionReleaseStatus status)
        {
            if (Equipment == null) return;
            base.OnDistanceRelease(distanceInteraction, status);

            Equipment.OnDistanceRelease((InteractionEquipment)distanceInteraction.ExternalInteraction, status);
        }

        public override void OnDistanceNotInteractionRelease()
        {
            if (Equipment == null)
                return;

            base.OnDistanceNotInteractionRelease();
            Equipment.OnDistanceNotRelease();

        }
    }
}
