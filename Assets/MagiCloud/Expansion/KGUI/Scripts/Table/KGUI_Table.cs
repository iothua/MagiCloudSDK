using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MagiCloud.KGUI
{
    //根据输入的行与列，生成表格

    /// <summary>
    /// 表格
    /// </summary>
    [ExecuteInEditMode]
    public class KGUI_Table :MonoBehaviour
    {
        public string TableName = "表格1";

        public List<KGUI_TableRow> Rows;

        public GameObject cellObject;
        public GameObject rowObject;

        //合并单元格
        public List<Vector2Int> Merges;

        /// <summary>
        /// 行父物体
        /// </summary>
        public RectTransform rowParent;

        /// <summary>
        /// 单元格之间的间距
        /// </summary>
        public float spacing;

        /// <summary>
        /// 行列
        /// </summary>
        public Vector2Int Ranks;

        /// <summary>
        /// 行高，行宽
        /// </summary>
        public Vector2 cellSize;

        public Color textColor = Color.white;
        public Sprite cellBackground;
        public int FontSize = 15;
        private float parentY = 0; //初始Y值

        private void Awake()
        {
            if (cellObject == null)
                cellObject = Resources.Load<GameObject>("Prefabs\\Cell");

            if (rowObject == null)
                rowObject = Resources.Load<GameObject>("Prefabs\\Row");
        }

        /// <summary>
        /// 生成表
        /// </summary>
        public void CreateTable()
        {
            DeleteTable();

            //设置父对象的高度
            rowParent.anchorMin = new Vector2(0.5f,1);
            rowParent.anchorMax = new Vector2(0.5f,1);
            rowParent.pivot = new Vector2(0.5f,1);
            UpdateContentRect();
            parentY = 0;


            //行
            for (int i = 0; i < Ranks.x; i++)
            {
                CreatRow(i);
            }
            UpdateContentRect();
        }

        private void UpdateContentRect()
        {
            //根据行列宽与行列数，计算出高度，以及间隔
            Vector2 size = new Vector2(rowParent.sizeDelta.x,cellSize.y * Ranks.x + spacing * (Ranks.x - 1));

            rowParent.sizeDelta=size;
            GetComponent<RectTransform>().sizeDelta=size;
        }


        public void AddRow(int i)
        {
            CreatRow(i);
            Ranks.x++;
            UpdateContentRect();
        }

        public void CreatRow(int i)
        {
            //生成行
            var row = Instantiate(rowObject,rowParent);

            row.name = "row" + i;

            var rowRect = row.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0,1);
            rowRect.anchorMax = new Vector2(0,1);
            rowRect.pivot = new Vector2(0,1);

            //设置行高,列宽
            rowRect.sizeDelta = (Rows.Count<1) ? new Vector2(cellSize.x * Ranks.y,cellSize.y) : Rows[0].Rect.sizeDelta;
            parentY =-(i)*(cellSize.y + spacing);
            //设置坐标
            rowRect.localPosition = new Vector3(-rowParent.sizeDelta.x / 2,parentY,rowRect.localPosition.z);
            // parentY -= cellSize.y + spacing;//

            var kguiRow = row.GetComponent<KGUI_TableRow>();
            //算出单元格
            float rowX = 0;

            //列
            for (int j = 0; j < Ranks.y; j++)
            {
                CreatCell(i,kguiRow,ref rowX,j);
                //添加单元格信息
            }
            //添加行
            Rows.Add(kguiRow);
            // Ranks.x++;

        }

        private void CreatCell(int i,KGUI_TableRow row,ref float rowX,int j)
        {
            var cell = Instantiate(cellObject,row.transform);

            cell.name = "cell" + j;

            var kguiCell = cell.GetComponent<KGUI_TableCell>();

            //设置瞄点
            kguiCell.rectTransform.anchorMin = new Vector2(0,1);
            kguiCell.rectTransform.anchorMax = new Vector2(0,1);
            kguiCell.rectTransform.pivot = new Vector2(0,1);
            kguiCell.SetCell(" ");
            kguiCell.UpdateCell(textColor,cellBackground,FontSize);
            Vector2 size = cellSize;
            Vector2 pos = new Vector3(rowX,0,0);//ID坐标
            if (Rows.Count>0)
            {
                size=Rows[0].Cells[j].rectTransform.sizeDelta;
                //     pos=Rows[0].Cells[j].rectTransform.localPosition;
            }
            //计算坐标
            cell.transform.localPosition = pos;

            //计算下一个单元格的坐标
            rowX += size.x + spacing;

            //设置单元格大小
            kguiCell.SetSize(size);

            kguiCell.Position =new Vector2Int(i,j);

            row.Add(kguiCell);
        }

        /// <summary>
        /// 删除表格
        /// </summary>
        public void DeleteTable()
        {
            foreach (var row in Rows)
            {
                if (row == null) continue;

                foreach (var item in row.Cells)
                {
                    if (item == null) continue;
                    DestroyImmediate(item.gameObject);
                }

                DestroyImmediate(row.gameObject);
            }

            Rows.Clear();
        }

        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="tableCell"></param>
        public void SetColumnWidth(KGUI_TableCell tableCell,float width)
        {
            //获取到所有的行
            for (int i = 0; i < Rows.Count; i++)
            {
                //算出单元格
                float rowX = 0;

                //遍历所有的列
                for (int j = 0; j < Rows[i].Cells.Count; j++)
                {
                    KGUI_TableCell cell = Rows[i].Cells[j];
                    //计算坐标
                    cell.transform.localPosition = new Vector3(rowX,0,0);

                    //如果是同一列
                    if (cell.Position.y == tableCell.Position.y)
                    {
                        //设置宽度
                        cell.SetSize(new Vector2(width,cell.Size.y));
                        //并且重新计算排榜
                    }

                    rowX += cell.Size.x + spacing;
                }
            }
        }

        /// <summary>
        /// 设置行高
        /// </summary>
        /// <param name="tableCell"></param>
        public void SetRowHeight(KGUI_TableCell tableCell,float height)
        {
            //算出单元格
            float sumHeight = 0;//设置总高度

            //获取到所有的行
            for (int i = 0; i < Rows.Count; i++)
            {
                Rows[i].transform.localPosition = new Vector3(-rowParent.sizeDelta.x / 2,sumHeight,Rows[i].transform.localPosition.z);

                //如果在同一行,不用遍历全部的列，只需要遍历第一个元素即可
                if (Rows[i].Cells[0].Position.x == tableCell.Position.x)
                {
                    //则设置此行
                    Rows[i].Rect.sizeDelta = new Vector2(Rows[i].Size.x,height);//设置高度
                    //设置该列的所有列高
                    for (int j = 0; j < Rows[i].Cells.Count; j++)
                    {
                        KGUI_TableCell cell = Rows[i].Cells[j];

                        cell.SetSize(new Vector2(cell.Size.x,height));
                    }
                }

                sumHeight -= Rows[i].Size.y + spacing;//高度
            }

            //根据行列宽与行列数，计算出高度，以及间隔
            rowParent.sizeDelta = new Vector2(rowParent.sizeDelta.x,Mathf.Abs(sumHeight));//重新计算出总高度
        }

        /// <summary>
        /// 合并列
        /// </summary>
        /// <param name="row"></param>
        /// <param name="startCloumn"></param>
        /// <param name="endCloumn"></param>
        public void MergeCloumn(int row,int startCloumn,int endCloumn)
        {
            ////获取到这个范围内的所有列，判断是否存在已经隐藏的，如果存在，则直接跳过
            //for (int i = startCloumn; i < endCloumn; i++)
            //{
            //    if (Rows[row].Cells[i].IsHide) return;//如果存在隐藏的，则直接跳过。表示所选的已经有合并了
            //}

            KGUI_TableCell startCell = GetTableCell(new Vector2Int(row,startCloumn));
            //判断起始单元格，是否还存在其他的行

            int _row;

            if (startCell.IsExistRow(out _row))
            {
                for (int i = row + 1; i <= _row; i++)
                {
                    //将该行的所有列都注释掉
                    for (int j = startCloumn; j <= endCloumn; j++)
                    {
                        startCell.AddCell(GetTableCell(new Vector2Int(i,j)));
                    }
                }
            }

            //如果存在，则也将其他的行包含过去，执行合并单元格
            for (int i = startCloumn + 1; i <= endCloumn; i++)
            {
                //合并列
                startCell.OnCloumMerge(GetTableCell(new Vector2Int(row,i)),spacing);
            }
        }

        /// <summary>
        /// 合并行
        /// </summary>
        /// <param name="cloumn"></param>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        public void MergeRow(int cloumn,int startRow,int endRow)
        {
            //获取到这个范围内的所有列，判断是否存在已经隐藏的，如果存在，则直接跳过
            for (int i = startRow; i < startRow; i++)
            {
                //如果存在隐藏的，则直接跳过。表示所选的已经有合并了
                if (Rows[i].Cells[cloumn].IsHide)
                {
                    Debug.Log("已经存在合并的");
                    return;
                };
            }

            KGUI_TableCell startCell = GetTableCell(new Vector2Int(startRow,cloumn));

            int _cloumn;

            if (startCell.IsExistRow(out _cloumn))
            {
                for (int i = cloumn + 1; i <= _cloumn; i++)
                {
                    //将该列的所有行都注释掉
                    for (int j = startRow; j <= endRow; j++)
                    {
                        startCell.AddCell(GetTableCell(new Vector2Int(j,i)));
                    }
                }
            }

            //如果存在，则也将其他的行包含过去，执行合并单元格
            for (int i = startRow + 1; i <= endRow; i++)
            {
                //合并列
                startCell.OnRowMerge(GetTableCell(new Vector2Int(i,cloumn)),spacing);
            }
        }

        ///// <summary>
        ///// 合并
        ///// </summary>
        //public void OnMerge(KGUI_TableCell startCell, KGUI_TableCell endCell)
        //{
        //    //检查是否在相邻
        //    startCell.OnMerge(endCell, spacing);
        //}


        /// <summary>
        /// 根据坐标获取到单元格
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public KGUI_TableCell GetTableCell(Vector2Int position)
        {
            if (position.x >= Rows.Count) return null;

            //获取都行
            KGUI_TableRow row = Rows[position.x];

            if (position.y >= row.Cells.Count) return null;

            return row.Cells[position.y];
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="datas"></param>
        public void AddData(Dictionary<Vector2Int,string> datas)
        {
            if (datas == null) return;

            foreach (var data in datas)
            {
                KGUI_TableCell cell = GetTableCell(data.Key);
                if (cell == null || cell.IsHide)
                    continue;

                cell.SetCell(data.Value);
            }
        }

        public void AddData(Vector2Int vector,string data)
        {
            KGUI_TableCell cell = GetTableCell(vector);
            if (cell == null) return;

            cell.SetCell(data);
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="keys"></param>
        public void ClearData(List<Vector2Int> keys)
        {
            if (keys == null) return;

            foreach (var item in keys)
            {
                KGUI_TableCell cell = GetTableCell(item);
                if (cell == null || cell.IsHide)
                    continue;

                cell.SetCell("");
            }
        }

        /// <summary>
        /// Update Cell
        /// </summary>
        /// <param name="color"></param>
        /// <param name="sprite"></param>
        public void UpdateCell(Color color,Sprite sprite,int fontSize)
        {
            foreach (var row in Rows)
            {
                foreach (var cell in row.Cells)
                {
                    cell.UpdateCell(textColor,cellBackground,fontSize);
                }
            }
        }

        /// <summary>
        /// Update Cell
        /// </summary>
        /// <param name="color"></param>
        /// <param name="sprite"></param>
        public void UpdateCell()
        {
            UpdateCell(textColor,cellBackground,FontSize);
        }
    }
}
