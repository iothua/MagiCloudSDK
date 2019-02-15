using UnityEngine;
using MagiCloud.Interactive.Distance;
using MagiCloud.Features;

namespace MagiCloud.Interactive.Actions
{
    /// <summary>
    /// 虚影
    /// </summary>
    [System.Serializable]
    public class InteractionShadow :InteractionAction
    {
        public bool isLocal = true;
        public Vector3 localPosition = Vector3.zero;
        public Vector3 localRotation = Vector3.zero;
        public Vector3 localScale = Vector3.one;
        public string shaderName = "Legacy Shaders/Transparent/Diffuse";
        public ShadowType type = ShadowType.Auto;
        public int renderQueue = 3000;
        [Range(0.1f,0.9f)]
        public float intension = 0.25f;
        public Transform traModelNode;
        public ShadowController shadowController;
        public Color color = Color.yellow;
        public bool isReUpdate = false;
        //public InteractionShadow(Transform node)
        //{
        //    renderQueue=3000;
        //    intension=0.25f;
        //    shaderName = "Legacy Shaders/Transparent/Diffuse";
        //    color=Color.yellow;
        //    Init(node);
        //}

        public void Init(Transform node)
        {
            shadowController?.Destroy();
            shadowController=null;
            //if (shadowController==null)
            //    shadowController=node.gameObject.GetComponent<ShadowController>();
            //if (shadowController==null)
            shadowController=node.gameObject.AddComponent<ShadowController>();
            shadowController.Init(node.parent,traModelNode,Color.yellow,intension,renderQueue,type,shaderName);
        }

        public override void OnOpen(DistanceInteraction InteractionSelf,DistanceInteraction interaction)
        {
            base.OnOpen(InteractionSelf,interaction);

            if (interaction == null) return;
            if (InteractionSelf == null) return;

            if (InteractionSelf.IsGrab && !IsSelf) return;

            if (!IsOpen && !IsLimit)
            {
                if (shadowController==null||isReUpdate) Init(InteractionSelf.transform);
                shadowController.OpenGhost(interaction.FeaturesObjectController.transform,
                    localPosition,localScale,Quaternion.Euler(localRotation),isLocal);

                IsOpen = true;
            }
        }

        public override void OnClose(DistanceInteraction InteractionSelf,DistanceInteraction interaction)
        {
            base.OnClose(InteractionSelf,interaction);

            if (InteractionSelf == null) return;
            if (InteractionSelf.IsGrab && !IsSelf) return;

            if (IsOpen)
            {
                shadowController.CloseGhost();
                IsOpen = false;
            }
        }
        public void OnDestory()
        {
            shadowController?.Destroy();
        }
    }
}
