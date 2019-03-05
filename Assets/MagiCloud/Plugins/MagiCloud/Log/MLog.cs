using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace MagiCloud
{
    public static class MLog
    {
        private static string currentLogPath = string.Empty;

        private static List<string> logs = new List<string>();

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

            logs.Add(info);
        }

        public static void WriteLogs()
        {
            if (string.IsNullOrEmpty(currentLogPath)) return;

            Json.JsonHelper.SaveJson(logs.ToString(), currentLogPath);
        }

    }
}

