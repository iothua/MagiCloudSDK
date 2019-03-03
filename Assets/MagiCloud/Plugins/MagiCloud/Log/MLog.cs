using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

namespace MagiCloud
{
    public static class MLog
    {
        private static string currentLogPath = string.Empty;

        private static StringBuilder logs = new StringBuilder();

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="content">Content.</param>
        public static void WriteLog(string content)
        {
            if (string.IsNullOrEmpty(content)) return;

            if (string.IsNullOrEmpty(currentLogPath))
            {
                currentLogPath = Application.streamingAssetsPath + "/Logs/" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            }

            string info = System.DateTime.Now.ToString("HH:mm:ss:ffff") + "-" + content + "\n";

            logs.Append(info);
        }

        public static void WriteLogs()
        {
            Json.JsonHelper.SaveJson(logs.ToString(), currentLogPath);
        }

    }
}

