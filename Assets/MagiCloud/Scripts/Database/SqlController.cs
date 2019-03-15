using System;
using UnityEngine;
using System.Data;
using System.Reflection;
using System.Collections.Generic;

namespace MagiCloud.Database
{
    public class SqlController : MonoBehaviour 
    {
        private SqlAccess sqlAccess;

        private void Awake()
        {
            AssetBundleManager.LoadAsset<TextAsset>(new string[1] { "MagiCloudPlatform/mysql.txt" }, (texts) =>
            {
                string[] sqls = texts[0].text.Split(';');
                sqlAccess = new SqlAccess(sqls[0], sqls[1], sqls[2], sqls[3]);
            });
        }

        /// <summary>
        /// 将DataSet转化为实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="p_DataSet">DataSet</param>
        /// <param name="p_TableIndex">要转换数据表索引</param>
        /// <returns>实体类</returns>
        public static T DataSetToEntity<T>(DataSet p_DataSet, int p_TableIndex = 0)
        {
            if (p_DataSet == null || p_DataSet.Tables.Count < 0) return default(T);

            if (p_TableIndex > p_DataSet.Tables.Count - 1) return default(T);
            if (p_TableIndex < 0) p_TableIndex = 0;

            if (p_DataSet.Tables[p_TableIndex].Rows.Count <= 0) return default(T);

            DataRow p_Data = p_DataSet.Tables[p_TableIndex].Rows[0];

            T t = (T)Activator.CreateInstance(typeof(T));

            PropertyInfo[] properties = t.GetType().GetProperties();

            foreach (PropertyInfo pi in properties)
            {
                if (p_DataSet.Tables[p_TableIndex].Columns.IndexOf(pi.Name.ToUpper()) != -1 && p_Data[pi.Name.ToUpper()] != DBNull.Value)
                {
                    pi.SetValue(t, p_Data[pi.Name.ToUpper()], null);
                }
                else
                {
                    pi.SetValue(t, null, null);
                }
            }

            return t;
        }

        public static IList<T> DataSetToEntityList<T>(DataSet p_DataSet, int p_TableIndex)
        {
            if (p_DataSet == null || p_DataSet.Tables.Count < 0)
                return default(IList<T>);

            if (p_TableIndex > p_DataSet.Tables.Count - 1)
                return default(IList<T>);

            if (p_TableIndex < 0)
                p_TableIndex = 0;

            if (p_DataSet.Tables[p_TableIndex].Rows.Count <= 0)
                return default(IList<T>);

            DataTable p_Data = p_DataSet.Tables[p_TableIndex];
            IList<T> result = new List<T>();

            for (int i = 0; i < p_Data.Rows.Count; i++)
            {
                T t = (T)Activator.CreateInstance(typeof(T));

                PropertyInfo[] properties = t.GetType().GetProperties();
                foreach (PropertyInfo pi in properties)
                {
                    if (p_Data.Columns.IndexOf(pi.Name.ToUpper()) != -1 && p_Data.Rows[i][pi.Name.ToUpper()] != DBNull.Value)
                    {
                        pi.SetValue(t, p_Data.Rows[i][pi.Name.ToUpper()], null);
                    }
                    else
                    {
                        pi.SetValue(t, null, null);
                    }
                }

                result.Add(t);
            }

            return result;
        }

    }
}
