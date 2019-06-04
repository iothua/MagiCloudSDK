using MagiCloud.Core.UI;
using UnityEngine.Events;

namespace MagiCloud.UIFrame
{


    /// <summary>
    /// UI公用Button嵌入
    /// </summary>
    public class UI_ButtonEmbed :UI_Base
    {
        private IButton button;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Operate = UIOperate.Embed;
            if (button == null)
                button = GetComponentInChildren<IButton>();
        }

        public void AddClick(UnityAction<int> unityAction,bool isOpen = true)
        {
            if (unityAction == null) return;

            if (isOpen)
            {
                OnOpen();
            }
            if (button != null)
            {
                button.Click.AddListener(unityAction);
            }
        }

        public void RemoveClick(UnityAction<int> unityAction,bool isClose = true)
        {
            if (isClose)
            {
                OnClose();
            }

            if (button != null)
            {
                button.Click.RemoveListener(unityAction);
            }
        }

        public void RemoveClickAll()
        {
            OnClose();

            if (button != null)
            {
                button.Click.RemoveAllListeners();
            }
        }
    }
}
