using System;
using MagiCloud.Interactive.Distance;
using UnityEngine;

namespace MagiCloud.Interactive.Actions
{
    public class InteractionParent : InteractionAction
    {
        [Header("父对象")]
        public Transform Parent;
        [Header("局部坐标")]
        public Vector3 localPosition = Vector3.zero;
        [Header("布局旋转值")]
        public Vector3 localRotation = Vector3.zero;

        public override void OnClose(DistanceInteraction interaction)
        {
            base.OnClose(interaction);

            if (interaction == null) return;
            if (Interaction.IsGrab && !IsSelf) return;

            interaction.FeaturesObjectController.SetParent(null);

            IsOpen = false;
        }

        public override void OnOpen(DistanceInteraction interaction)
        {
            base.OnOpen(interaction);

            if (interaction == null) return;

            if (Interaction.IsGrab && !IsSelf) return;

            if (IsLimit) return;

            interaction.FeaturesObjectController.SetParent(Parent, localPosition, localRotation);
            IsOpen = true;
        }
    }
}
