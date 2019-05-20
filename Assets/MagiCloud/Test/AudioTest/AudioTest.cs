using MagiCloud.KGUI;
using UnityEngine;
using UnityEngine.UI;

namespace MagiCloud.TextToAudio
{
    public class AudioTest :MonoBehaviour
    {
        public Text text;
        public KGUI_Button button;
        public KGUI_Toggle toggle;
        public string context;
        public void Start()
        {
            button.onClick.AddListener((i) =>
            {
                AudioMainSingle.Instance.PlayAudio(text.text);
                toggle.IsValue=true;
            });
            toggle.OnValueChanged.AddListener((x) => AudioMainSingle.Instance.TogglePause(x));
        }
    }
}
