using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace MagiCloud.Json
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 将对象序列化为Json字符串
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ObjectToJsonString(object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        /// <summary>
        /// 根据路径读取Json数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadJsonString(string path)
        {
            if (!File.Exists(path)) return string.Empty;

            string jsonData;
            StreamReader sr = new StreamReader(path);
            jsonData = sr.ReadToEnd();
            sr.Close();
            return jsonData;
        }

        /// <summary>
        /// 解析Json字符串生成对象实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string json) where T : class
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        /// <summary>
        /// 解析Json字符串生成对象实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> JsonToList<T>(string json) where T : class
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }

        /// <summary>
        /// 保存为Json文件
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path"></param>
        public static void SaveJson(string json, string path)
        {
            string strPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }

            FileStream file = new FileStream(path, FileMode.Create);
            byte[] bts = System.Text.Encoding.UTF8.GetBytes(json);
            file.Write(bts, 0, bts.Length);
            if (file != null)
            {
                file.Close();
            }
        }
        /// <summary>
        /// Json数据格式化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string JsonFormat(string json)
        {
            int level = 0;
            var jsonArr = json.ToArray();   //ToArray()转为char[];
            string jsonTree = string.Empty; //格式化后的字符串
            for (int i = 0; i < json.Length; i++)
            {
                char c = jsonArr[i];
                if (level > 0 && '\n' == jsonTree.ToArray()[jsonTree.Length - 1])   //最后一个char是'\n'
                {
                    jsonTree += TreeLevel(level);       //与上一行垂直对齐
                }
                switch (c)
                {
                    case '[':
                    case '{':
                        jsonTree += c + "\n";
                        level++;
                        break;
                    case ']':
                    case '}':
                        jsonTree += "\n";
                        level--;
                        jsonTree += TreeLevel(level);
                        jsonTree += c;
                        break;
                    case ',':
                        jsonTree += c + "\n";
                        break;
                    default:
                        jsonTree += c;
                        break;
                }
            }
            return jsonTree;
        }
        /// <summary>
        /// 树等级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static string TreeLevel(int level)
        {
            string leaf = string.Empty;
            for (int t = 0; t < level; t++)
            {
                leaf += "\t";
            }
            return leaf;
        }
    }
}
