using UnityEngine;
using MagiCloud.Interactive.Distance;

namespace MagiCloud.Interactive.Actions
{
    /// <summary>
    /// 虚影
    /// </summary>
    public class InteractionShadow : InteractionAction
    {
        public Vector3 localPosition = Vector3.zero;
        public Vector3 localRotation = Vector3.zero;
        public Vector3 localScale = Vector3.one;

        public override void OnOpen(DistanceInteraction interaction)
        {
            base.OnOpen(interaction);

            if (Interaction == null) return;

            if (Interaction.IsGrab && !IsSelf) return;

            if (!IsOpen && !IsLimit)
            {
                if (Interaction.FeaturesObjectController.ActiveShadow)
                {
                    Interaction.FeaturesObjectController.ShadowController.OpenGhost(interaction.FeaturesObjectController.transform,
                        localPosition, localScale, Quaternion.Euler(localRotation));

                    IsOpen = true;
                }
            }
        }

        public override void OnClose(DistanceInteraction interaction)
        {
            base.OnClose(interaction);

            if (Interaction == null) return;
            if (Interaction.IsGrab && !IsSelf) return;

            if (IsOpen)
            {
                if (Interaction.FeaturesObjectController.ActiveShadow)
                {
                    Interaction.FeaturesObjectController.ShadowController.CloseGhost();
                    IsOpen = false;
                }
            }
        }
    }
}
