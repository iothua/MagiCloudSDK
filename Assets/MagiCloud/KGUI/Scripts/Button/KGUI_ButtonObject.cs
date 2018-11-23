using System;
using UnityEngine;
using System.Collections.Generic;
using MagiCloud.Operate;
using MagiCloud.Core.Events;

namespace MagiCloud.KGUI
{
    public class PanelRange
    {
        public int handIndex;
        public bool IsRangeUI;
    }

    /// <summary>
    /// Button物体
    /// </summary>
    public class KGUI_ButtonObject : KGUI_ButtonBase
    {
        public GameObject bindObject;//绑定的物体

        [Header("当值为-1时为无穷。当值为0时，则自动禁用")]
        public int maxCount = -1;

        private List<GameObject> targetObjects = new List<GameObject>();

        private List<PanelRange> ranges = new List<PanelRange>();

        public KGUI_Panel panel;

        public AudioClip audioClip;//音频
        public bool IsStartAudio = true;
        public AudioSource audioSource;
        public float zValue = 5f;

        public override void OnDown(int handIndex)
        {
            base.OnDown(handIndex);

            if (maxCount == 0) return;

            var targetObject = Instantiate(bindObject) as GameObject;
            var frontUI = targetObject.GetComponent<KGUI_ObjectFrontUI>() ?? targetObject.AddComponent<KGUI_ObjectFrontUI>();
            frontUI.OnSet();

            MOperateManager.SetObjectGrab(targetObject, zValue, handIndex);
            //KinectTransfer.SetObjectGrab(targetObject, zValue, handIndex: handIndex);

            targetObjects.Add(targetObject);

            if (maxCount != -1)
                maxCount--;

            if (maxCount == 0)
            {
                IsEnable = false;
            }
        }

        public override void OnEnter(int handIndex)
        {
            if (IsStartAudio && audioSource != null)
            {
                audioSource.Play();
            }

            base.OnEnter(handIndex);
        }

        void Register()
        {
            //Events.EventHandReleaseObject.AddListener(Events.EventLevel.B, OnReleaseObject);
            //MCKinect.Events.KinectEventHandReleaseObject.AddListener(MCKinect.Events.EventLevel.B, OnReleaseObject);
            //Events.EventHandGrabObject.AddListener(Events.EventLevel.B, OnGrabObject);

            EventHandReleaseObject.AddListener(OnReleaseObject);
            EventHandGrabObject.AddListener(OnGrabObject);

            if (panel != null)
            {
                panel.onEnter.AddListener(OnPanelEnter);
                panel.onExit.AddListener(OnPanelExit);
            }
        }

        void Logout()
        {
            EventHandReleaseObject.RemoveListener(OnReleaseObject);
            EventHandGrabObject.RemoveListener(OnGrabObject);


            //Events.EventHandReleaseObject.RemoveListener(OnReleaseObject);
            //MCKinect.Events.KinectEventHandReleaseObject.RemoveListener(OnReleaseObject);

            //Events.EventHandGrabObject.RemoveListener(OnGrabObject);

            if (panel != null)
            {
                panel.onEnter.RemoveListener(OnPanelEnter);
                panel.onExit.RemoveListener(OnPanelExit);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Register();
        }

        void OnPanelEnter(int handIndex)
        {
            PanelRange panelRange = ranges.Find(obj => obj.handIndex.Equals(handIndex));

            if (panelRange == null)
            {
                panelRange = new PanelRange() { handIndex = handIndex, IsRangeUI = true };
                ranges.Add(panelRange);
            }
            else
            {
                panelRange.IsRangeUI = true;
            }

        }

        void OnPanelExit(int handIndex)
        {
            PanelRange panelRange = ranges.Find(obj => obj.handIndex.Equals(handIndex));
            if (panelRange != null)
            {
                panelRange.IsRangeUI = false;
            }
        }

        void OnGrabObject(GameObject target, int handIndex)
        {
            if (!targetObjects.Contains(target)) return;

            target.GetComponent<KGUI_ObjectFrontUI>().OnSet();
        }

        void OnReleaseObject(GameObject target, int handIndex)
        {
            //释放时，发送一次事件?

            if (!targetObjects.Contains(target))
                return;

            PanelRange panelRange = ranges.Find(obj => obj.handIndex.Equals(handIndex));

            if (panelRange == null)
                return;

            if (panelRange.IsRangeUI)
            {
                targetObjects.Remove(target);

                Destroy(target);

                if (maxCount != -1)
                {
                    maxCount++;
                    IsEnable = true;
                }
            }
            else
            {
                target.GetComponent<KGUI_ObjectFrontUI>().OnReset();
            }

        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Logout();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Logout();
        }

        public void AddAudio()
        {
            if (audioClip == null)
            {
                audioClip = Resources.Load<AudioClip>("Audios\\手势划过-2");
            }

            if (audioSource == null)
            {
                if (audioSource == null)
                {
                    audioSource = gameObject.GetComponent<AudioSource>();
                }

                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
                if (audioSource.playOnAwake)
                    audioSource.playOnAwake = false;

                if (audioClip != audioSource.clip)
                    audioSource.clip = audioClip;
            }
        }

        public void DestroyAudio()
        {
            if (audioSource != null)
            {
                DestroyImmediate(audioSource);
            }
        }
    }
}
