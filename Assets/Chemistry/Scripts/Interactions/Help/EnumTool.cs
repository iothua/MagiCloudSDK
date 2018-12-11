using System;
using System.Collections.Generic;
namespace Chemistry.Help
{
    /// <summary>
    /// 枚举工具
    /// </summary>
    public static class EnumTool
    {
        public static string[] EnumToEnum(Type from,Type to)
        {
            List<string> temp = new List<string>();
            foreach (var item in Enum.GetValues(from))
            {
                string name = Enum.GetName(to,item);
                temp.Add(name);
                if (name==string.Empty)
                    break;
            }
            return temp.ToArray();
        }

        /// <summary>
        /// 将枚举的int值转成string
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string IntToString(int index,Type type)
        {
            return Enum.ToObject(type,index).ToString();
        }

    }

}


