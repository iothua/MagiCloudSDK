using System;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.Interactive.Distance;
using MagiCloud.Interactive.Actions;

namespace MagiCloud.Interactive
{
    /// <summary>
    /// 交互功能控制端（内部集成功能）
    /// </summary>
    [RequireComponent(typeof(DistanceInteraction))]
    public class InteractionController : MonoBehaviour
    {
        public bool StartShadow = false;//启动加入虚影功能
        public bool StartAddParent = false;//启动加入子父功能

        public Interaction_AddParent addParent;
        public Interaction_Shadow Shadow;

        private GameObject interactionObject;
        public GameObject InteractionObject {
            get {

                if (interactionObject == null)
                {
                    var obj = transform.Find("interactionObject");

                    if (obj == null)
                    {
                        interactionObject = null;
                    }
                    else
                    {
                        interactionObject = obj.gameObject;
                    }

                    if (interactionObject == null)
                    {
                        interactionObject = new GameObject("interactionObject");
                        interactionObject.transform.SetParent(transform);
                        interactionObject.transform.localPosition = Vector3.zero;
                        interactionObject.hideFlags = HideFlags.HideInHierarchy;
                    }
                }

                return interactionObject.gameObject;
            }
        }

        private void Start()
        {
            if (StartShadow)
            {
                AddShadow();
            }
            else
            {
                RemoveShadow();
            }

            if (StartAddParent)
            {
                AddParent();
            }
            else
            {
                RemoveParent();
            }
        }

        public Interaction_Shadow AddShadow()
        {
            if (Shadow == null)
            {
                Shadow = InteractionObject.GetComponent<Interaction_Shadow>() ?? InteractionObject.AddComponent<Interaction_Shadow>();
                Shadow.Interaction = gameObject.GetComponent<DistanceInteraction>();
            }

            Shadow.hideFlags = HideFlags.HideInInspector;

            return Shadow;
        }

        public void RemoveShadow()
        {
            if (Shadow == null) return;

            DestroyImmediate(Shadow);
        }

        public Interaction_AddParent AddParent()
        {
            if (addParent == null)
            {
                addParent = InteractionObject.GetComponent<Interaction_AddParent>() ?? InteractionObject.AddComponent<Interaction_AddParent>();
                addParent.InteractionSelf = gameObject.GetComponent<DistanceInteraction>();
            }

            addParent.hideFlags = HideFlags.HideInInspector;

            return addParent;
        }

        public void RemoveParent()
        {
            if (addParent == null) return;

            DestroyImmediate(addParent);
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            DestroyImmediate(InteractionObject);
#endif
        }
    }
}
