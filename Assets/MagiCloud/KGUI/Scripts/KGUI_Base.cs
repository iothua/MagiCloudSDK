using UnityEngine;

namespace MagiCloud.KGUI
{
    public class KGUI_Base : MonoBehaviour
    {
        public virtual void OnDown(int handIndex)
        {
            //MagiCloud.Events.EventHandPressedButton.SendListener(this, handIndex);
        }

        public virtual void OnUp(int handIndex)
        {
            //MagiCloud.Events.EventHandReleaseButton.SendListener(this, handIndex);

        }

    }
}
