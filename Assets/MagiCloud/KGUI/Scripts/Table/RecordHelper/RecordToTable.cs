using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
namespace MagiCloud.KGUI
{
    /// <summary>
    /// 记录数据到表格
    /// </summary>
    public class RecordToTable :SerializedMonoBehaviour
    {
        [Header("数据记录者")]
        public IRecord record;
        public KGUI_Button recordButton;
        public Image trueImage;
        public Image errorImage;
        public KGUI_TableManagerHelper tableManager;
        public KGUI_Button showButtom;
        private Color color = Color.white;
        void Start()
        {
            if (recordButton!=null)
                recordButton.onClick.AddListener(OnClick);
        }

        private void OnClick(int arg)
        {
            if (record==null)
            {
#if UNITY_EDITOR
                Debug.Log("record为空");
#endif
                return;
            }
            //如果播放时不记录
            if (!record.CanReocrd)
            {
                errorImage.color=color;
                errorImage.DOFade(0,1f).SetEase(Ease.InQuint);
            }
            else
            {
                trueImage.color=color;
                trueImage.DOFade(0,1f).SetEase(Ease.InQuint);
                //记录数据到表格
                tableManager.SetDataToTable(record.GetData(),1);
            }
        }


        public void Show()
        {
            tableManager.transform.DOLocalMoveY(0,0.5f);
            showButtom.gameObject.SetActive(false);
        }



        public void Close(int i)
        {
            tableManager.transform.DOLocalMoveY(1000,0.5f);
            showButtom.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            if (recordButton!=null)
                recordButton.onClick.RemoveListener(OnClick);

        }
    }
}