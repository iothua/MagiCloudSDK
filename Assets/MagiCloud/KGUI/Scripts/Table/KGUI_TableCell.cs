using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 单元格
    /// </summary>
    public class KGUI_TableCell : MonoBehaviour
    {
        //信息
        public Text textInfo;

        private RectTransform _rectTransform;
        private Image image;

        /// <summary>
        /// 坐标（X为X轴，Y为Y轴）
        /// </summary>
        public Vector2Int Position;

        //单元格合并对象
        public List<KGUI_TableCell> Cells;



        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHide;

        /// <summary>
        /// 大小（X为宽度，Y为长度）
        /// </summary>
        public Vector2 Size {
            get {
                return rectTransform.sizeDelta;
            }
            set {
                rectTransform.sizeDelta = value;
            }
        }

        public RectTransform rectTransform {
            get {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        public Image Background {
            get {
                if (image == null)
                    image = GetComponent<Image>();

                return image;
            }
        }

        /// <summary>
        /// 设置单元格信息
        /// </summary>
        /// <param name="str"></param>
        /// <param name="alignment"></param>
        public void SetCell(string str, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            textInfo.text = str;

            textInfo.alignment = alignment;
        }

        /// <summary>
        /// 获取到单元格信息
        /// </summary>
        /// <returns></returns>
        public string GetCell()
        {
            return textInfo.text;
        }

        /// <summary>
        /// 大小
        /// </summary>
        /// <param name="vector"></param>
        public void SetSize(Vector2 vector)
        {
            Size = vector;
            rectTransform.sizeDelta = Size;
        }

        public void OnHide()
        {
            gameObject.SetActive(false);
            IsHide = true;
        }

        public void OnShow()
        {
            gameObject.SetActive(true);
            IsHide = false;
        }

        #region 注释

        ///// <summary>
        ///// 合并
        ///// </summary>
        ///// <param name="cell"></param>
        //public void OnMerge(KGUI_TableCell cell, float spaceing)
        //{
        //    //是否行相邻
        //    OnRowMerge(cell, spaceing);

        //    //是否列相邻
        //    OnCloumMerge(cell, spaceing);
        //}

        ///// <summary>
        ///// 是否行相邻
        ///// </summary>
        ///// <param name="cell"></param>
        ///// <returns></returns>
        //public bool OnRowNeighbor(KGUI_TableCell cell)
        //{
        //    //如果数组的最后一个，不跟他相邻的话，则直接跳过
        //    float row = 0;

        //    foreach (var item in Cells)
        //    {
        //        //找到最大的行值
        //        if (item.Position.x > row)
        //            row = item.Position.x;
        //    }

        //    //判断是否相邻
        //    if (Mathf.Abs(cell.Position.x - row) != 1 || Mathf.Abs(cell.Position.x - Position.x) != 1) return false;

        //    return true;
        //}

        //public bool OnCloumNeighbor(KGUI_TableCell cell)
        //{
        //    //if (cell.Position.y - Position.y != 1) return false;

        //    float cloum = 0;

        //    foreach (var item in Cells)
        //    {
        //        if (item.Position.y > cloum)
        //            cloum = item.Position.y;
        //    }

        //    //不相邻，则直接跳过
        //    if (Mathf.Abs(cell.Position.y - cloum) != 1 || cell.Position.y - Position.y != 1) return false;

        //    return true;
        //}
        #endregion

        /// <summary>
        /// 行合并
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="spaceing"></param>
        /// <returns></returns>
        public bool OnRowMerge(KGUI_TableCell cell,float spaceing)
        {
            ////如果不在同一列的话，则直接跳过
            //if (Position.y != cell.Position.y|| !OnRowNeighbor(cell))
            //    //判断是否行相邻
            //    return false;

            //如果不相邻，则跳过
            //if (!OnRowNeighbor(cell)) return false;
            if (cell == null) return false;

            if (!Cells.Contains(cell))
            {
                //进行合并
                Cells.Add(cell);
                cell.OnHide();
            }

            //重新计算高度
            Size = new Vector2(Size.x, cell.Size.y + Size.y+spaceing);

            return true;
        }

        /// <summary>
        /// 列合并
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="spaceing"></param>
        /// <returns></returns>
        public bool OnCloumMerge(KGUI_TableCell cell, float spaceing)
        {
            if (cell == null) return false;

            if (!Cells.Contains(cell))
            {
                Cells.Add(cell);
                cell.OnHide();
            }

            Size = new Vector2(cell.Size.x + Size.x + spaceing, Size.y);

            return true;
        }

        /// <summary>
        /// 添加单元格
        /// </summary>
        /// <param name="cell"></param>
        public void AddCell(KGUI_TableCell cell)
        {
            if (!Cells.Contains(cell))
            {
                Cells.Add(cell);
                cell.OnHide();
            }
        }

        public void RemoveCell(KGUI_TableCell cell)
        {

        }

        /// <summary>
        /// 存在其他行
        /// </summary>
        /// <param name="row"></param>
        public bool IsExistRow(out int row)
        {
            row = 0;

            //判断是否还存在其他的行
            if (Cells.Count == 0)
            {
                return false;
            }

            //获取到最大的行值
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Position.x > row)
                    row = Cells[i].Position.x;
            }

            return row > 0 ? true : false;
        }

        /// <summary>
        /// 是否存在其他列
        /// </summary>
        /// <param name="cloumn"></param>
        /// <returns></returns>
        public bool IsExistCloumn(out int cloumn)
        {
            cloumn = 0;
            if (Cells.Count == 0) return false;

            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Position.y > cloumn)
                    cloumn = Cells[i].Position.y;
            }

            return cloumn > 0 ? true : false;
        }

        public void UpdateCell(Color color, Sprite sprite,int fontSize)
        {
            textInfo.color = color;
            if (sprite == null)
            {
                Background.enabled = false;
            }
            else
            {
                Background.enabled = true;
                Background.sprite = sprite;
            }

            textInfo.fontSize = fontSize;
        }
    }
}
