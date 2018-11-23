using System;
using UnityEngine;
using System.Collections;
using MagiCloud.Interactive.Distance;

namespace MagiCloud.Interactive.Actions
{
    /// <summary>
    /// 加入父物体操作
    /// </summary>
    
    public class Interaction_AddParent : MonoBehaviour,IInteraction_Limit
    {
        [Header("父对象")]
        public Transform Parent;
        [Header("局部坐标")]
        public Vector3 localPosition = Vector3.zero;
        [Header("局部旋转值")]
        public Vector3 localRotation = Vector3.zero;

        public DistanceInteraction InteractionSelf;

        /// <summary>
        /// 本身
        /// </summary>
        [Header("抓取本身时，是加入到自己身上(True)，还是在对方身上(False)")]
        public bool IsSelf = false;

        public bool IsLimit { get; set; }

        private void Start()
        {
            if (InteractionSelf == null)
                InteractionSelf = GetComponent<DistanceInteraction>();

            if (InteractionSelf == null) return;

            InteractionSelf.OnRelease.AddListener(OnDistanceRelesae);
            InteractionSelf.OnExit.AddListener(OnDistanceExit);
        }

        private void OnDestroy()
        {
            if (InteractionSelf == null)
                return;

            InteractionSelf.OnRelease.RemoveListener(OnDistanceRelesae);
            InteractionSelf.OnExit.RemoveListener(OnDistanceExit);
        }

        void OnDistanceExit(DistanceInteraction interaction)
        {
            if (interaction == null) return;
            if (InteractionSelf.IsGrab && !IsSelf) return;

            interaction.FeaturesObjectController.SetParent(null);
        }

        void OnDistanceRelesae(DistanceInteraction interaction)
        {
            if (interaction == null) return;

            if (InteractionSelf.IsGrab && !IsSelf) return;

            if (IsLimit) return;

            //Debug.Log("抓取：" + interaction.IsGrab);

            //Debug.Log("本身：" + interaction.FeaturesObjectController.name);
            //Debug.Log("对方：" + interaction.FeaturesObjectController.name);

            interaction.FeaturesObjectController.SetParent(Parent, localPosition, localRotation);
        }
    }
}
