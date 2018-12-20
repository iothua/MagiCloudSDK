using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// 表格控制端
    /// </summary>
    public class KGUI_TableManager :MonoBehaviour
    {
        public List<KGUI_Table> Tables;

        public KGUI_Slider slider;

        private void Start()
        {
            //Dictionary<Vector2Int, string> datas = new Dictionary<Vector2Int, string>();

            //datas.Add(new Vector2Int(0, 0), "哈哈");
            //datas.Add(new Vector2Int(0, 1), "嘻嘻");
            //datas.Add(new Vector2Int(0, 2), "乐乐");
            //AddData(GetTable(0), datas);
        }

        public KGUI_Table GetTable(int index)
        {
            if (index >= Tables.Count) return null;

            return Tables[index];
        }

        public KGUI_Table GetTable(string name)
        {
            return Tables.Find(obj => obj.TableName.Equals(name));
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="datas"></param>
        public void AddData(KGUI_Table table,Dictionary<Vector2Int,string> datas)
        {
            if (table == null)
            {
                Debug.Log("添加数据：表格对象为Null");
                return;
            }

            table.AddData(datas);
        }

        public void AddData(KGUI_Table table,Vector2Int vector,string data)
        {
            if (table == null)
            {
                Debug.Log("添加数据：表格对象为Null");
                return;
            }

            table.AddData(vector,data);
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="keys"></param>
        public void ClearData(KGUI_Table table,List<Vector2Int> keys)
        {
            if (table == null)
            {
                Debug.Log("移除数据：表格对象为Null");
                return;
            }

            table.ClearData(keys);
        }

        public void AddRow(KGUI_Table table)
        {
            int i = table.Rows.Count;
            table.AddRow(i);
        }

        public void AddCloumn(KGUI_Table table)
        { }
    }
}
