using System;
using UnityEngine;
using MagiCloud.Interactive.Distance;

namespace MagiCloud.Interactive.Actions
{
    /// <summary>
    /// 虚影交互
    /// </summary>
    public class Interaction_Shadow : MonoBehaviour,IInteraction_Limit
    {
        public DistanceInteraction Interaction;

        private bool IsOpen = false;

        public Vector3 localPosition = Vector3.zero;
        public Vector3 localRotation = Vector3.zero;
        public Vector3 localScale = Vector3.one;

        /// <summary>
        /// 本身
        /// </summary>
        [Header("抓取本身时，是加入到自己身上(True)，还是在对方身上(False)")]
        public bool IsSelf = false;

        public bool IsLimit { get; set; }

        private void Start()
        {
            if (Interaction == null)
                Interaction = GetComponent<DistanceInteraction>();

            if (Interaction == null) return;

            Interaction.OnStay.AddListener(OnDistanceStay);
            Interaction.OnExit.AddListener(OnDistanceExit);
            Interaction.OnRelease.AddListener(OnDistanceRelesae);
            //Interaction.OnRelesaeReset.AddListener(OnDistanceRelesae);
        }

        private void OnDestroy()
        {
            if (Interaction == null) return;

            Interaction.OnStay.RemoveListener(OnDistanceStay);
            Interaction.OnExit.RemoveListener(OnDistanceExit);
            Interaction.OnRelease.RemoveListener(OnDistanceRelesae);
            //Interaction.OnRelesaeReset.RemoveListener(OnDistanceRelesae);
        }

        void OnDistanceStay(DistanceInteraction interaction)
        {
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

        void OnDistanceExit(DistanceInteraction interaction)
        {
            if (Interaction == null) return;
            if (Interaction.IsGrab && !IsSelf) return;
            if (IsOpen && !IsLimit)
            {
                if (Interaction.FeaturesObjectController.ActiveShadow)
                {
                    Interaction.FeaturesObjectController.ShadowController.CloseGhost();
                    IsOpen = false;
                }
            }
        }

        void OnDistanceRelesae(DistanceInteraction interaction)
        {
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
