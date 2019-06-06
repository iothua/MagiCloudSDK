#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Loxodon.Framework.Bundles.Objects
{
    public static class ObjectExtensions
    {
        public static void ToString(this object obj, StringBuilder buf, int depth)
        {
            if (obj is DynamicObject)
            {
                ((DynamicObject)obj).ToString(buf, depth);
            }
            else if (obj is string)
            {
                buf.AppendFormat("\"{0}\"", obj);
            }
            else if (obj is Matrix4x4)
            {
                Matrix4x4 mat = (Matrix4x4)obj;
                mat.ToString(buf, depth);
            }
            else if (obj is StreamedResource)
            {
                StreamedResource resource = (StreamedResource)obj;
                resource.ToString(buf, depth);
            }
            //else if (obj is Collection)
            //{
            //    Collection list = obj as Collection;
            //    list.ToString(buf, depth);
            //}
            else if (obj is Map)
            {
                Map dict = obj as Map;
                dict.ToString(buf, depth);
            }
            else if (obj is IList)
            {
                IList list = obj as IList;
                list.ToString(buf, depth);
            }
            else if (obj is IDictionary)
            {
                IDictionary dict = obj as IDictionary;
                dict.ToString(buf, depth);
            }
            else
            {
                buf.Append(obj);
            }
        }

        public static void ToString(this StreamedResource resource, StringBuilder buf, int depth)
        {
            string tab = "\t";
            string indent = "";
            for (int i = 0; i < depth; i++)
                indent += tab;

            buf.Append("StreamedResource { ").Append(Environment.NewLine);
            buf.Append(indent).Append(tab).AppendFormat("Source : {0},", resource.Source).Append(Environment.NewLine);
            buf.Append(indent).Append(tab).AppendFormat("Offset : {0},", resource.Offset).Append(Environment.NewLine);
            buf.Append(indent).Append(tab).AppendFormat("Size : {0},", resource.Size).Append(Environment.NewLine);
            buf.Append(indent).Append("}");
        }

        public static void ToString(this Matrix4x4 mat, StringBuilder buf, int depth)
        {
            buf.Append("Matrix4x4(");
            buf.AppendFormat("{0}, ", mat.m00);
            buf.AppendFormat("{0}, ", mat.m01);
            buf.AppendFormat("{0}, ", mat.m02);
            buf.AppendFormat("{0}, ", mat.m03);
            buf.AppendFormat("{0}, ", mat.m10);
            buf.AppendFormat("{0}, ", mat.m11);
            buf.AppendFormat("{0}, ", mat.m12);
            buf.AppendFormat("{0}, ", mat.m13);
            buf.AppendFormat("{0}, ", mat.m20);
            buf.AppendFormat("{0}, ", mat.m21);
            buf.AppendFormat("{0}, ", mat.m22);
            buf.AppendFormat("{0}, ", mat.m23);
            buf.AppendFormat("{0}, ", mat.m30);
            buf.AppendFormat("{0}, ", mat.m31);
            buf.AppendFormat("{0}, ", mat.m32);
            buf.AppendFormat("{0}, ", mat.m33);
            buf.Append(")");
        }

        public static void ToString(this IList list, StringBuilder buf, int depth)
        {
            string tab = "\t";
            string indent = "";
            for (int i = 0; i < depth; i++)
                indent += tab;

            if (list.Count <= 0)
            {
                buf.Append("[]");
                return;
            }

            if (list[0].GetType().IsPrimitive)
            {
                buf.Append("[ ");
                for (int i = 0; i < list.Count; i++)
                {
                    buf.AppendFormat("{0}", list[i]);
                    buf.Append(i < list.Count - 1 ? ", " : " ");
                }
                buf.Append("]");
                return;
            }

            buf.Append("[").Append(Environment.NewLine);
            for (int i = 0; i < list.Count; i++)
            {
                buf.Append(indent).Append(tab);
                list[i].ToString(buf, depth + 1);
                if (i < list.Count - 1)
                    buf.Append(",");
                buf.Append(Environment.NewLine);
            }
            buf.Append(indent).Append("]");
        }

        //public static void ToString(this Collection list, StringBuilder buf, int depth)
        //{
        //    string tab = "\t";
        //    string indent = "";
        //    for (int i = 0; i < depth; i++)
        //        indent += tab;

        //    buf.AppendFormat("{0} ", list.TypeNode.TypeName);
        //    if (list.Count <= 0)
        //    {
        //        buf.Append("[]");
        //        return;
        //    }

        //    if (list[0].GetType().IsPrimitive)
        //    {
        //        buf.Append("[ ");
        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            buf.AppendFormat("{0}", list[i]);
        //            buf.Append(i < list.Count - 1 ? ", " : " ");
        //        }
        //        buf.Append("]");
        //        return;
        //    }

        //    buf.Append("[").Append(Environment.NewLine);
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        buf.Append(indent).Append(tab);
        //        list[i].ToString(buf, depth + 1);
        //        if (i < list.Count - 1)
        //            buf.Append(",");
        //        buf.Append(Environment.NewLine);
        //    }
        //    buf.Append(indent).Append("]");
        //}

        public static void ToString(this IDictionary dict, StringBuilder buf, int depth)
        {
            string tab = "\t";
            string indent = "";
            for (int i = 0; i < depth; i++)
                indent += tab;

            if (dict.Count <= 0)
            {
                buf.Append("{}");
                return;
            }

            buf.Append("{").Append(Environment.NewLine);

            int count = dict.Count;
            int n = 0;
            foreach (DictionaryEntry kv in dict)
            {
                buf.Append(indent).Append(tab);
                kv.Key.ToString(buf, depth + 1);
                buf.Append(" : ");
                kv.Value.ToString(buf, depth + 1);
                if (n++ < count - 1)
                    buf.Append(",");
                buf.Append(Environment.NewLine);
            }
            buf.Append(indent).Append("}");
        }

        public static void ToString(this Map dict, StringBuilder buf, int depth)
        {
            string tab = "\t";
            string indent = "";
            for (int i = 0; i < depth; i++)
                indent += tab;

            buf.AppendFormat("{0} ", dict.TypeNode.TypeName);
            if (dict.Count <= 0)
            {
                buf.Append("{}");
                return;
            }

            buf.Append("{").Append(Environment.NewLine);

            int count = dict.Count;
            int n = 0;
            //foreach (DictionaryEntry kv in dict)
            foreach (var kv in dict)
            {
                buf.Append(indent).Append(tab);
                kv.Key.ToString(buf, depth + 1);
                buf.Append(" : ");
                kv.Value.ToString(buf, depth + 1);
                if (n++ < count - 1)
                    buf.Append(",");
                buf.Append(Environment.NewLine);
            }
            buf.Append(indent).Append("}");
        }

        public static void ToString(this DynamicObject obj, StringBuilder buf, int depth)
        {
            string tab = "\t";
            string indent = "";
            for (int i = 0; i < depth; i++)
                indent += tab;


            if (obj.Count <= 0)
            {
                buf.Append(obj.TypeNode.TypeName).Append(" {}");
                return;
            }

            buf.Append(obj.TypeNode.TypeName).Append(" {").Append(Environment.NewLine);

            if (obj is UnityDynamicObject)
            {
                UnityDynamicObject unityObj = obj as UnityDynamicObject;
                buf.Append(indent).Append(tab).AppendFormat("ID : {0},", unityObj.ID).Append(Environment.NewLine);
                buf.Append(indent).Append(tab).AppendFormat("Size : {0},", unityObj.Size).Append(Environment.NewLine);
            }

            int count = obj.Count;
            int n = 0;
            foreach (KeyValuePair<string, object> kv in obj)
            {
                buf.Append(indent).Append(tab);
                buf.Append(kv.Key).Append(" : ");
                kv.Value.ToString(buf, depth + 1);
                if (n++ < count - 1)
                    buf.Append(",");
                buf.Append(Environment.NewLine);
            }
            buf.Append(indent).Append("}");
        }
    }
}
#endif
