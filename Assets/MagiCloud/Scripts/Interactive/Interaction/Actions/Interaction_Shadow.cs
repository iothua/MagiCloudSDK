using System;
using UnityEngine;
using MagiCloud.Interactive.Distance;
using MagiCloud.Features;

namespace MagiCloud.Interactive.Actions
{
    /// <summary>
    /// 虚影交互
    /// </summary>
    public class Interaction_Shadow :MonoBehaviour, IInteraction_Limit
    {
        public DistanceInteraction Interaction;

        private bool IsOpen = false;
        public Vector3 localPosition = Vector3.zero;
        public Vector3 localRotation = Vector3.zero;
        public Vector3 localScale = Vector3.one;
        public Transform traModelNode;
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
            Init(Interaction.transform);
        }

        private void OnDestroy()
        {
            if (Interaction == null) return;

            Interaction.OnStay.RemoveListener(OnDistanceStay);
            Interaction.OnExit.RemoveListener(OnDistanceExit);
            Interaction.OnRelease.RemoveListener(OnDistanceRelesae);
            //Interaction.OnRelesaeReset.RemoveListener(OnDistanceRelesae);
        }
        public void Init(Transform node)
        {
            shadowController?.Destroy();
            shadowController=null;
            //if (shadowController==null)
            //    shadowController=node.gameObject.GetComponent<ShadowController>();
            //if (shadowController==null)
            shadowController=node.gameObject.AddComponent<ShadowController>();
            shadowController.Init(node.parent,traModelNode,Color.yellow);
        }
        ShadowController shadowController;
        void OnDistanceStay(DistanceInteraction interaction)
        {
            if (Interaction == null) return;
            if (!Interaction.HasDetected) return;
            if (Interaction.IsGrab && !IsSelf) return;

            if (!IsOpen && !IsLimit)
            {
                //if (Interaction.FeaturesObjectController.ActiveShadow)
                //{

                if (shadowController!=null)
                {
                    shadowController.Init(Interaction.transform.parent,traModelNode,Color.yellow,0.25f,3000,traModelNode ? ShadowType.Manual : ShadowType.Auto);
                    shadowController.OpenGhost(interaction.FeaturesObjectController.transform,
                       localPosition,localScale,Quaternion.Euler(localRotation));
                }
                IsOpen = true;
            }
        }

        void OnDistanceExit(DistanceInteraction interaction)
        {
            if (Interaction == null) return;
            if (Interaction.IsGrab && !IsSelf) return;
            if (IsOpen && !IsLimit)
            {
                shadowController?.CloseGhost();
                IsOpen=false;
                //if (Interaction.FeaturesObjectController.ActiveShadow)
                //{
                //    Interaction.FeaturesObjectController.ShadowController.CloseGhost();
                //    IsOpen = false;
                //}
            }
        }

        void OnDistanceRelesae(DistanceInteraction interaction)
        {
            if (Interaction == null) return;
            if (Interaction.IsGrab && !IsSelf) return;

            if (IsOpen)
            {
                shadowController?.CloseGhost();
                IsOpen = false;
                //    if (Interaction.FeaturesObjectController.ActiveShadow)
                //    {
                //        Interaction.FeaturesObjectController.ShadowController.CloseGhost();
                //        IsOpen = false;
                //    }
            }
        }
    }
}
