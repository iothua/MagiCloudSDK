#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;

namespace Loxodon.Framework.Bundles.Archives
{
    public static class ArchiveExtensions
    {
        private static PropertyInfo inspectorMode = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

        static TypeNodeParser parser = new TypeNodeParser();

        static ArchiveExtensions()
        {
        }

        public static T GetObject<T>(this IObjectInfo info) where T : class
        {
            if (info is ObjectInfo)
                return (T)parser.Create((ObjectInfo)info);

            if (info is BuiltinObjectInfo)
                return (info as BuiltinObjectInfo).GetObject<T>();

            if (info is NullObjectInfo)
                return (info as NullObjectInfo).GetObject<T>();

            if (info is MissingObjectInfo)
                return (info as MissingObjectInfo).GetObject<T>();

            return null;
        }

        public static long GetLocalFileID(this UnityEngine.Object target)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            inspectorMode.SetValue(serializedObject, InspectorMode.Debug, null);
            SerializedProperty localIdProperty = serializedObject.FindProperty("m_LocalIdentfierInFile");
            return localIdProperty.longValue;
        }
    }
}
#endif