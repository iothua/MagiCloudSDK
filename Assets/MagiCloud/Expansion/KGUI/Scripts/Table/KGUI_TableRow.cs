using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 表格行
    /// </summary>
    public class KGUI_TableRow : MonoBehaviour
    {
        /// <summary>
        /// 列集合
        /// </summary>
        public List<KGUI_TableCell> Cells;

        private RectTransform _rectTransform;

        public RectTransform Rect {
            get {

                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public Vector2 Size {
            get {
                return Rect.sizeDelta;
            }
        }

        public void Add(KGUI_TableCell cell)
        {
            if (Cells.Contains(cell)) return;

            Cells.Add(cell);
        }

        public void Remove(KGUI_TableCell cell)
        {
            if (!Cells.Contains(cell))
                return;

            Cells.Remove(cell);
        }
    }
}
