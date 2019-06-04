using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Utility
{
    public static partial class Utility
    {
        public static class Text
        {
            [ThreadStatic]
            private static StringBuilder _cachedStringBuilder = new StringBuilder(1024);
            /// <summary>
            /// 获取格式化字符串
            /// </summary>
            /// <param name="format">字符串格式</param>
            /// <param name="arg">字符串参数</param>
            /// <returns></returns>
            public static string Format(string format,object arg)
            {
                if (format==null)
                    throw new ArgumentNullException(format);
                _cachedStringBuilder.Length=0;
                _cachedStringBuilder.AppendFormat(format,arg);
                return _cachedStringBuilder.ToString();
            }
            public static string Format(string format,object arg0,object arg1)
            {
                if (format==null)
                    throw new ArgumentNullException(format);
                _cachedStringBuilder.Length=0;
                _cachedStringBuilder.AppendFormat(format,arg0,arg1);
                return _cachedStringBuilder.ToString();
            }
            public static string Format(string format,object arg0,object arg1,object arg2)
            {
                if (format==null)
                    throw new ArgumentNullException(format);
                _cachedStringBuilder.Length=0;
                _cachedStringBuilder.AppendFormat(format,arg0,arg1,arg2);
                return _cachedStringBuilder.ToString();
            }
            public static string Format(string format,params object[] args)
            {
                if (format==null)
                    throw new ArgumentNullException(format);
                if (args==null)
                    throw new ArgumentNullException("args");

                _cachedStringBuilder.Length=0;
                _cachedStringBuilder.AppendFormat(format,args);
                return _cachedStringBuilder.ToString();
            }

            public static string[] SplitToLines(string text)
            {
                List<string> texts = new List<string>();
                int index = 0;
                string rowText = null;
                while ((rowText=ReadLine(text,ref index))!=null)
                {
                    texts.Add(rowText);
                }
                return texts.ToArray();
            }

            public static string GetFullName<T>(string name)
            {
                return GetFullName(typeof(T),name);
            }

            public static string GetFullName(Type type,string name)
            {
                if (type==null)
                    throw new ArgumentNullException(type.FullName);
                string fullName = type.FullName;
                return string.IsNullOrEmpty(name) ? fullName : Format("{0}.{1}",fullName,name);
            }

            public static string FieldNameForDisplay(string fieldName)
            {
                if (string.IsNullOrEmpty(fieldName))
                    return string.Empty;
                string str = Regex.Replace(fieldName,@"^m_",string.Empty);
                str =Regex.Replace(str,@"((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))",@" $1").TrimStart();
                return str;
            }


            private static string ReadLine(string text,ref int index)
            {
                if (text==null)
                    return null;
                int length = text.Length;
                int temp = index;
                while (temp<length)
                {
                    char ch = text[temp];
                    switch (ch)
                    {
                        case '\r':
                        case '\n':
                            string str = text.Substring(index,temp-index);
                            index=temp+1;
                            if (((ch=='\r')&&index<length&&text[index]=='\n'))
                                index++;
                            return str;
                        default:
                            temp++;
                            break;
                    }
                }
                if (temp>index)
                {
                    string str = text.Substring(index,temp-index);
                    index=temp;
                    return str;
                }
                return null;
            }
        }
    }
}
