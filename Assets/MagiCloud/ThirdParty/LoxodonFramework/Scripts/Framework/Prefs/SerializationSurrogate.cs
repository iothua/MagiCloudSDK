//using UnityEngine;
//using System.Runtime.Serialization;

//namespace Loxodon.Framework.Prefs
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class Vector2SerializationSurrogate : ISerializationSurrogate
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
//        {
//            Vector2 v = (Vector2)obj;
//            info.AddValue("x", v.x);
//            info.AddValue("y", v.y);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        /// <param name="selector"></param>
//        /// <returns></returns>
//        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
//        {
//            Vector2 v = (Vector2)obj;
//            v.x = (float)info.GetValue("x", typeof(float));
//            v.y = (float)info.GetValue("y", typeof(float));
//            return v;
//        }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class Vector3SerializationSurrogate : ISerializationSurrogate
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
//        {
//            Vector3 v = (Vector3)obj;
//            info.AddValue("x", v.x);
//            info.AddValue("y", v.y);
//            info.AddValue("z", v.z);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        /// <param name="selector"></param>
//        /// <returns></returns>
//        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
//        {
//            Vector3 v = (Vector3)obj;
//            v.x = (float)info.GetValue("x", typeof(float));
//            v.y = (float)info.GetValue("y", typeof(float));
//            v.z = (float)info.GetValue("z", typeof(float));
//            return (object)v;
//        }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class Vector4SerializationSurrogate : ISerializationSurrogate
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
//        {
//            Vector4 v = (Vector4)obj;
//            info.AddValue("x", v.x);
//            info.AddValue("y", v.y);
//            info.AddValue("z", v.z);
//            info.AddValue("w", v.w);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        /// <param name="selector"></param>
//        /// <returns></returns>
//        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
//        {
//            Vector4 v = (Vector4)obj;
//            v.x = (float)info.GetValue("x", typeof(float));
//            v.y = (float)info.GetValue("y", typeof(float));
//            v.z = (float)info.GetValue("z", typeof(float));
//            v.w = (float)info.GetValue("w", typeof(float));
//            return (object)v;
//        }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class ColorSerializationSurrogate : ISerializationSurrogate
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
//        {
//            Color c = (Color)obj;
//            info.AddValue("r", c.r);
//            info.AddValue("g", c.g);
//            info.AddValue("b", c.b);
//            info.AddValue("a", c.a);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        /// <param name="selector"></param>
//        /// <returns></returns>
//        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
//        {
//            Color c = (Color)obj;
//            c.r = (float)info.GetValue("r", typeof(float));
//            c.g = (float)info.GetValue("g", typeof(float));
//            c.b = (float)info.GetValue("b", typeof(float));
//            c.a = (float)info.GetValue("a", typeof(float));
//            return (object)c;
//        }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class Color32SerializationSurrogate : ISerializationSurrogate
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
//        {
//            Color32 c = (Color32)obj;
//            info.AddValue("r", c.r);
//            info.AddValue("g", c.g);
//            info.AddValue("b", c.b);
//            info.AddValue("a", c.a);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="info"></param>
//        /// <param name="context"></param>
//        /// <param name="selector"></param>
//        /// <returns></returns>
//        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
//        {
//            Color32 c = (Color32)obj;
//            c.r = (byte)info.GetValue("r", typeof(byte));
//            c.g = (byte)info.GetValue("g", typeof(byte));
//            c.b = (byte)info.GetValue("b", typeof(byte));
//            c.a = (byte)info.GetValue("a", typeof(byte));
//            return (object)c;
//        }
//    }
//}
