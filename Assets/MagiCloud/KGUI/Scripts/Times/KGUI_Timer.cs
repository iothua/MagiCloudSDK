using UnityEngine;
using MagiCloud.Common;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{
    public class KGUI_Timer : MonoBehaviour
    {
        public Text txtTime;

        private string format;
        private MTimer timer;

        public float timeSpeed = 1;

        private void Start()
        {
            StartClock();
        }

        public void StartClock()
        {
            timer = new MTimer(timerValue: (timerValue, deleta) =>
            {
                if (txtTime != null)
                    txtTime.text = timerValue.ToFormat(format);

            }, timeSpeed: timeSpeed);
        }
    }
}
