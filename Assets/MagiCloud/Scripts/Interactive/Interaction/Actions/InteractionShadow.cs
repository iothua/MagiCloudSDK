using UnityEngine;
using MagiCloud.Interactive.Distance;

namespace MagiCloud.Interactive.Actions
{
    /// <summary>
    /// 虚影
    /// </summary>
    [System.Serializable]
    public class InteractionShadow : InteractionAction
    {
        public Vector3 localPosition = Vector3.zero;
        public Vector3 localRotation = Vector3.zero;
        public Vector3 localScale = Vector3.one;

        public override void OnOpen(DistanceInteraction InteractionSelf, DistanceInteraction interaction)
        {
            base.OnOpen(InteractionSelf, interaction);

            if (interaction == null) return;
            if (InteractionSelf == null) return;

            if (InteractionSelf.IsGrab && !IsSelf) return;

            if (!IsOpen && !IsLimit)
            {
                if (InteractionSelf.FeaturesObjectController.ActiveShadow)
                {
                    InteractionSelf.FeaturesObjectController.ShadowController.OpenGhost(interaction.FeaturesObjectController.transform,
                        localPosition, localScale, Quaternion.Euler(localRotation));

                    IsOpen = true;
                }
            }
        }

        public override void OnClose(DistanceInteraction InteractionSelf, DistanceInteraction interaction)
        {
            base.OnClose(InteractionSelf, interaction);

            if (InteractionSelf == null) return;
            if (InteractionSelf.IsGrab && !IsSelf) return;

            if (IsOpen)
            {
                if (InteractionSelf.FeaturesObjectController.ActiveShadow)
                {
                    InteractionSelf.FeaturesObjectController.ShadowController.CloseGhost();
                    IsOpen = false;
                }
            }
        }
    }
}
