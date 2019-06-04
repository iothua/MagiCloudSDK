using System;
using MagiCloud.Interactive.Distance;
using UnityEngine;

namespace MagiCloud.Interactive.Actions
{
    [Serializable]
    public class InteractionParent : InteractionAction
    {
        [Header("父对象")]
        public Transform Parent;
        [Header("局部坐标")]
        public Vector3 localPosition = Vector3.zero;
        [Header("布局旋转值")]
        public Vector3 localRotation = Vector3.zero;
        public override void OnClose(DistanceInteraction InteractionSelf, DistanceInteraction interaction)
        {
            base.OnClose(InteractionSelf,interaction);

            if (interaction == null) return;
            if (InteractionSelf == null) return;
            if (InteractionSelf.IsGrab && !IsSelf) return;

            interaction.FeaturesObjectController.SetParent(null);

            IsOpen = false;
        }

        public override void OnOpen(DistanceInteraction InteractionSelf, DistanceInteraction interaction)
        {
            base.OnOpen(InteractionSelf, interaction);

            if (interaction == null) return;
            if (InteractionSelf == null) return;
            if (InteractionSelf.IsGrab && !IsSelf) return;

            if (IsLimit) return;

            interaction.FeaturesObjectController.SetParent(Parent, localPosition, localRotation);
            IsOpen = true;
        }
    }
}
