using DG.Tweening;
using System.Collections;
using UnityEngine;
namespace MagiCloud.KGUI
{
    public class KGUI_TurnPage : MonoBehaviour
    {
        public RectTransform content;

        public KGUI_Backpack kGUI_Backpack;

        public KGUI_Button buttonNext;
        public KGUI_Button buttonPre;

        private float min = 0;
        private float max;

        private bool isPreComplete = true;
        private bool isNextComplete = true;

        private int cellsNum;

        void Start()
        {
            kGUI_Backpack.action.AddListener(OnReset);

            StartCoroutine(EndAction());
        }

        private void OnDestroy()
        {
            kGUI_Backpack.action.RemoveListener(OnReset);
        }

        void Update()
        {

        }

        IEnumerator EndAction()
        {
            yield return new WaitForEndOfFrame();
            OnReset();
        }

        void OnReset()
        {
            cellsNum = content.GetComponentsInChildren<KGUI_BackpackItem>().Length;
            max = Mathf.Ceil(cellsNum / 4 - 1) * 480;
            content.localPosition = Vector3.zero;

            if (cellsNum <= 4)
            {
                buttonPre.IsEnable = false;
                buttonNext.IsEnable = false;
                buttonPre.gameObject.SetActive(false);
                buttonNext.gameObject.SetActive(false);
            }
            else
            {
                buttonPre.IsEnable = false;
                buttonNext.IsEnable = true;
                buttonNext.gameObject.SetActive(true);
                buttonPre.gameObject.SetActive(true);

            }
        }

        public void MovePrePage()
        {
            if (content.localPosition.x < min)
            {
                if (isPreComplete == true && isNextComplete == true)
                {
                    isPreComplete = false;
                    content.DOLocalMoveX(480, 1).SetRelative().OnComplete(() =>
                    {
                        isPreComplete = true;
                        if (content.localPosition.x >= min)
                        {
                            buttonPre.IsEnable = false;
                            buttonNext.IsEnable = true;
                        }
                        else
                        {
                            buttonPre.IsEnable = true;
                            buttonNext.IsEnable = true;
                        }
                    });

                }
            }

        }
        public void MoveNextPage()
        {
            if (content.localPosition.x >= -max)
            {
                if (isPreComplete == true && isNextComplete == true)
                {

                    isNextComplete = false;
                    content.DOLocalMoveX(-480, 1).SetRelative().OnComplete(() =>
                    {
                        isNextComplete = true;
                        if (content.localPosition.x <= -max)
                        {
                            buttonNext.IsEnable = false;
                            buttonPre.IsEnable = true;
                        }
                        else
                        {
                            buttonNext.IsEnable = true;
                            buttonPre.IsEnable = true;
                        }
                    });

                }
            }
        }
    }
}
