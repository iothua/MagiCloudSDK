using System;
using UnityEngine;
using UnityEngine.U2D;

namespace MagiCloud.UIFrame
{
    /// <summary>
    /// Set
    /// </summary>
    public class UI_Set : UI_Base
    {
       
        //public SpriteAtlas setAtlas;//设置集合

        public override void OnInitialize()
        {
            base.OnInitialize();
            Operate = UIOperate.Set;
            UI_SettingDAL.OnInitialize();

            //读取信息
        }

        /// <summary>
        /// 显示设置界面
        /// </summary>
        public override void OnOpen()
        {
            base.OnOpen();

            UIManager.AddCurrentUIToStack();

            //切换手势、鼠标等操作
            UIManager.Instance.SetUI(this);
        }

        /// <summary>
        /// 隐藏设置界面
        /// </summary>
        public override void OnClose()
        {
            base.OnClose();
            //隐藏手势、鼠标等操作
            var mainInterface = UIManager.GetUIStackPop();

            if (mainInterface != null)
                mainInterface.OnOpen();
        }

        /// <summary>
        /// 页显示
        /// </summary>
        /// <param name="page"></param>
        public void PageShow(GameObject page)
        {
            page.SetActive(true);
        }

        /// <summary>
        /// 页隐藏
        /// </summary>
        /// <param name="page"></param>
        public void PageHide(GameObject page)
        {
            page.SetActive(false);
        }


        /// <summary>
        /// 退出
        /// </summary>
        public void OnExit()
        {
            Application.Quit();
        }
    }

}
