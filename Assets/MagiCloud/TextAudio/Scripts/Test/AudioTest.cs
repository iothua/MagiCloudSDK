using MagiCloud.KGUI;
using UnityEngine;

namespace MagiCloud.TextToAudio
{
    public class AudioTest :MonoBehaviour
    {
        public KGUI_Text text;
        public KGUI_Button button;
        public KGUI_Toggle toggle;
        public string context;
        public void Start()
        {
            button.onClick.AddListener((i) =>
            {
                AudioMainSingle.Instance.PlayAudio(text.Text);
                toggle.IsValue=true;
            });
            toggle.OnValueChanged.AddListener((x) => AudioMainSingle.Instance.TogglePause(x));
        }
    }
}
