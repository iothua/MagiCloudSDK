using MagiCloud.KGUI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MagiCloud.TextToAudio
{
    public class AudioTest_Heat : MonoBehaviour
    {
        public KGUI_Button button;
        public KGUI_Toggle toggle;
        public string context;

        public DOTweenAnimation tweenAnimation;
        public void Start()
        {

            Invoke("PlayAudio", 0);
            if (toggle)
                toggle.IsValue = true;
            if (button)
                button.onClick.AddListener((i) =>
                {
                    AudioMainSingle.Instance.PlayAudio(context);
                    if (toggle)
                        toggle.IsValue = true;
                });
            if (toggle)
                toggle.OnValueChanged.AddListener((x) =>
            {
                x = !x;
                if (x)
                {
                    //AudioMainSingle.Instance.TogglePause(x);
                    //StopAudio();
                    //tweenAnimation.tween.Pause();
                   //GameObject.Find("AudioMainSingle") this.GetComponent<AudioSource>().volume = 0;

                    GameObject.FindObjectOfType<AudioMainSingle>().GetComponent<AudioSource>().volume = 0;
                }
                else
                {
                    //PlayAudio();
                    //tweenAnimation.tween.Play();
                    //this.GetComponent<AudioSource>().volume = 1;
                    GameObject.FindObjectOfType<AudioMainSingle>().GetComponent<AudioSource>().volume = 1;

                }

            });
        }

        void PlayAudio()
        {
            AudioMainSingle.Instance.PlayAudio(context);
            //Debug.Log(context);

        }

        void StopAudio()
        {
            //AudioMainSingle.Instance.TogglePause(toggle.IsValue);
            AudioMainSingle.Instance.StopCurrent();
        }

    }
}