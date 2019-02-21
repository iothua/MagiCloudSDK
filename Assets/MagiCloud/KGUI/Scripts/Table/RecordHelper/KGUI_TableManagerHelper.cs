using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
namespace MagiCloud.KGUI
{
    /// <summary>
    /// KGUI表格辅助
    /// </summary>
    public class KGUI_TableManagerHelper :MonoBehaviour
    {
        public KGUI_TableManager tableManager;
        [Header("表格首行内容")]
        public string[] heads;
        public Transform tableUIObj;
        protected bool isShow = true;
        protected int index = -1;
        public KGUI_ScrollView scrollView;

        protected virtual void Start()
        {
            SetTable();
            ShowTable(false);
        }
        /// <summary>
        /// 设置表格
        /// </summary>
        protected virtual void SetTable(int i = 0)
        {
            KGUI_Table table = tableManager.Tables[i];
            Dictionary<Vector2Int,string> dict = new Dictionary<Vector2Int,string>();
            for (int index = 0; index < heads.Length; index++)
            {
                dict.Add(new Vector2Int(0,index),heads[index]);
            }
            tableManager.AddData(tableManager.Tables[i],dict);
        }
        /// <summary>
        /// 表格显示
        /// </summary>
        public virtual void ShowTable(bool show)
        {
            tableUIObj.DOLocalMoveY(show ? 0 : 1500,0.5f);
            isShow = show;
        }

        /// <summary>
        /// 写入数据到表格中
        /// </summary>
        /// <param name="datas">数据</param>
        /// <param name="i">默认为第一个表格（暂时只记录第一个表格数）</param>
        /// <param name="isCover">是否覆盖</param>
        public void SetDataToTable(string[] datas,int i = 1,bool isCover = false,bool isColumn = false)
        {
            if (datas == null) return;
            if (!isColumn)
            {
                if (isCover)
                {
                    if (index >= tableManager.Tables[i].Rows.Count-1)
                        index=-1;
                }
                else
                {
                    if (index >= tableManager.Tables[i].Rows.Count-1)
                    {
                        tableManager.AddRow(tableManager.Tables[i]);
                        scrollView.SetRectData();
                        scrollView.vertical.SetRectData();
                    }
                }
            }
            else
            {
                if (isCover)
                {
                    if (index >= tableManager.Tables[i].Ranks.y-1)
                        index=-1;
                }
            }
            index++;
            Dictionary<Vector2Int,string> dict = new Dictionary<Vector2Int,string>();

            for (int j = 0; j < datas.Length; j++)
            {
                if (isColumn)
                    dict.Add(new Vector2Int(j,index),datas[j]);
                else
                    dict.Add(new Vector2Int(index,j),datas[j]);
            }
            tableManager.AddData(tableManager.Tables[i],dict);

            //}
        }
    }
}
