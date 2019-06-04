using UnityEngine;
using System.Collections;
using System;

namespace MagiCloud.UIFrame
{
    /// <summary>
    /// UI的动作
    /// </summary>
    public class UI_Action : MonoBehaviour
    {
        public ActionType actionType = ActionType.Hide;

        private bool isStart = false;

        /// <summary>
        /// 开始
        /// </summary>
        public bool IsStart {
            get {
                return isStart;
            }
            set {

                isStart = value;
                if (isStart && start != null)
                    start();
            }
        }

        private bool isComplete = false;
        /// <summary>
        /// 完成
        /// </summary>
        public bool IsComplete {
            get {
                return isComplete;
            }
            set {
                isComplete = value;

                if (isComplete && end != null)
                    end();
                    
            }
        }

        public float progress = 0;

        /// <summary>
        /// 开始和结束事件
        /// </summary>
        public Action start, end;

        public void OnShowExcute()
        {
            switch (actionType)
            {
                case ActionType.None:

                    IsStart = true;
                    IsComplete = true;

                    break;
                case ActionType.Hide:

                    gameObject.SetActive(true);
                    IsStart = true;
                    IsComplete = true;

                    //UIManager.Instance.StartCoroutine(HideHandle(true));
                    //gameObject.SetActive(true);
                    break;
                case ActionType.Facede:
                    break;
                case ActionType.Gradient:
                    break;
                case ActionType.Leave:
                    break;
            }
        }

        public void OnHideExcute()
        {
            switch (actionType)
            {
                case ActionType.None:
                    IsStart = true;
                    IsComplete = true;
                    break;
                case ActionType.Hide:

                    IsStart = true;
                    IsComplete = true;

                    gameObject.SetActive(false);
                    //UIManager.Instance.StartCoroutine(HideHandle(false));

                    //gameObject.SetActive(false);
                    break;
                case ActionType.Facede:
                    break;
                case ActionType.Gradient:
                    break;
                case ActionType.Leave:
                    break;
            }
        }

        IEnumerator HideHandle(bool isActive)
        {

            IsStart = true;
            IsComplete = false;
            gameObject.SetActive(isActive);

            yield return 0;
            //yield return new WaitForFixedUpdate();

            IsComplete = true;
            isStart = false;

        }
    }
}

