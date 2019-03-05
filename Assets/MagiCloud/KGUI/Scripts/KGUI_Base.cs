using UnityEngine;

namespace MagiCloud.KGUI
{
    public class KGUI_Base :MonoBehaviour
    {
        [SerializeField]
        protected int order = 0;

        public int Order { get { return order; } set { order=value; } }

        public virtual bool Active { get; set; } = true;

        public virtual void OnDown(int handIndex)
        {
            //MagiCloud.Events.EventHandPressedButton.SendListener(this, handIndex);
        }

        public virtual void OnUp(int handIndex)
        {
            //MagiCloud.Events.EventHandReleaseButton.SendListener(this, handIndex);

        }



        /// <summary>
        /// 向下屏蔽
        /// </summary>
        public void ShieldDownward()
        {

        }

        /// <summary>
        /// 向上屏蔽
        /// </summary>
        public void ShileldUpward()
        {

        }

    }
}
