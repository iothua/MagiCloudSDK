using MySql.Data.MySqlClient;
using System;
using System.Data;
using UnityEngine;

namespace MagiCloud
{
    /// <summary>
    /// Sql访问方法
    /// </summary>
    public class SqlAccess
    {
        public MySqlConnection dbConnection;

        public SqlAccess(string host, string id, string pwd, string database)
        {
            ContentSql(host, id, pwd, database);
        }

        private void ContentSql(string host, string id, string pwd, string database)
        {
            try
            {
                string connectionString = string.Format("Server = {0};port={4};Database = {1}; User ID = {2}; Password = {3};", host, database, id, pwd, "3306");
                dbConnection = new MySqlConnection(connectionString);
                dbConnection.Open();
            }
            catch (Exception e)
            {

                throw new Exception("服务器连接失败，请重新检查是否打开mySql服务。" + e.Message);
            }
        }

        public void Close()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
                dbConnection = null;
            }
        }

        /// <summary>  
        /// 查询表数据 
        /// </summary>  
        /// <param name="tableName">表名</param>  
        /// <param name="items">需要查询的列</param>  
        /// <param name="whereColName">查询的条件列</param>  
        /// <param name="operation">条件操作符</param>  
        /// <param name="value">条件的值</param>  
        /// <returns></returns>  
        public DataSet Select(string tableName, string[] items, string[] whereColName, string[] operation, string[] value)
        {
            if (whereColName.Length != operation.Length || operation.Length != value.Length)
            {
                throw new Exception("输入不正确：" + "col.Length != operation.Length != values.Length");
            }
            string query = "SELECT " + items[0];
            for (int i = 1; i < items.Length; i++)
            {
                query += "," + items[i];
            }
            query += "  FROM  " + tableName + "  WHERE " + " " + whereColName[0] + operation[0] + " '" + value[0] + "'";
            for (int i = 1; i < whereColName.Length; i++)
            {
                query += " AND " + whereColName[i] + operation[i] + "' " + value[i] + "'";
            }
            return ExecuteQuery(query);
        }

        /// <summary>  
        /// 创建具有id自增的表  
        /// </summary>  
        /// <param name="name">表名</param>  
        /// <param name="col">属性列</param>  
        /// <param name="colType">属性列类型</param>  
        /// <returns></returns>  
        public DataSet CreateTableAutoID(string name, string[] col, string[] colType)
        {
            if (col.Length != colType.Length)
            {
                throw new Exception("columns.Length != colType.Length");
            }
            string query = "CREATE TABLE  " + name + " (" + col[0] + " " + colType[0] + " NOT NULL AUTO_INCREMENT";
            for (int i = 1; i < col.Length; ++i)
            {
                query += ", " + col[i] + " " + colType[i];
            }
            query += ", PRIMARY KEY (" + col[0] + ")" + ")";
            return ExecuteQuery(query);
        }

        /// <summary>  
        /// 插入一条数据，包括所有，不适用自动累加ID。  
        /// </summary>  
        /// <param name="tableName">表名</param>  
        /// <param name="values">插入值</param>  
        /// <returns></returns>  
        public DataSet InsertInto(string tableName, string[] values)
        {
            string query = "INSERT INTO " + tableName + " VALUES (" + "'" + values[0] + "'";
            for (int i = 1; i < values.Length; ++i)
            {
                query += ", " + "'" + values[i] + "'";
            }
            query += ")";
            return ExecuteQuery(query);
        }

        /// <summary>  
        /// 插入部分ID  
        /// </summary>  
        /// <param name="tableName">表名</param>  
        /// <param name="col">属性列</param>  
        /// <param name="values">属性值</param>  
        /// <returns></returns>  
        public DataSet InsertInto(string tableName, string[] col, string[] values)
        {
            if (col.Length != values.Length)
            {
                throw new Exception("columns.Length != colType.Length");
            }
            string query = "INSERT INTO " + tableName + " (" + col[0];
            for (int i = 1; i < col.Length; ++i)
            {
                query += ", " + col[i];
            }
            query += ") VALUES (" + "'" + values[0] + "'";
            for (int i = 1; i < values.Length; ++i)
            {
                query += ", " + "'" + values[i] + "'";
            }
            query += ")";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 获取到指定表数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public DataSet GetAllData(string tableName, string[] items)
        {
            string query = "select " + items[0];
            for (int i = 1; i < items.Length; ++i)
            {

                query += ", " + items[i];

            }
            query += " From " + tableName;

            return ExecuteQuery(query);
        }

        /// <summary>  
        /// 更新表数据 
        /// </summary>  
        /// <param name="tableName">表名</param>  
        /// <param name="cols">更新列</param>  
        /// <param name="colsvalues">更新的值</param>  
        /// <param name="selectkey">条件：列</param>  
        /// <param name="selectvalue">条件：值</param>  
        /// <returns></returns>  
        public DataSet UpdateInto(string tableName, string[] cols, string[] colsvalues, string selectkey, string selectvalue)
        {
            string query = "UPDATE " + tableName + " SET " + cols[0] + " = " + colsvalues[0];
            for (int i = 1; i < colsvalues.Length; ++i)
            {
                query += ", " + cols[i] + " =" + colsvalues[i];
            }
            query += " WHERE " + selectkey + " = " + selectvalue + " ";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 获取数据库表行数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public DataSet Count(string tableName)
        {
            string query = "select count(*) from " + tableName;
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tableName">表明</param>
        /// <param name="cols">列数组</param>
        /// <param name="colsvalues">列数组对应的值</param>
        /// <returns></returns>
        public DataSet Delete(string tableName, string[] cols, string[] colsvalues)
        {
            string query = "DELETE FROM " + tableName + " WHERE " + cols[0] + " = " + colsvalues[0];

            for (int i = 1; i < colsvalues.Length; ++i)
            {

                query += " or " + cols[i] + " = " + colsvalues[i];
            }
            return ExecuteQuery(query);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="col">列名</param>
        /// <param name="colvalue">列值</param>
        /// <returns></returns>
        public DataSet Delete(string tableName, string col, string colvalue)
        {
            string query = "delete from " + tableName + " where " + col + " = '" + colvalue + "' ";

            return ExecuteQuery(query);
        }

        private DataSet ExecuteQuery(string sqlString)
        {
            if (dbConnection.State == ConnectionState.Open)
            {
                DataSet ds = new DataSet();
                try
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(sqlString, dbConnection);
                    da.Fill(ds);
                }
                catch (Exception ee)
                {
                    throw new Exception("SQL:" + sqlString + "/n" + ee.Message.ToString());
                }
            }

            return null;
        }
    }
}

