#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Loxodon.Framework.Bundles.Archives
{
    public class BuiltinArchive : AssetArchive
    {
        private Dictionary<long, BuiltinObjectInfo> objects = new Dictionary<long, BuiltinObjectInfo>();

        public BuiltinArchive(string name) : base(null, name, 0, 0, ArchiveType.BUILTIN)
        {
            this.Load();
        }

        public virtual void Load()
        {
            UnityEngine.Object[] list = AssetDatabase.LoadAllAssetsAtPath(this.Name);
            foreach (var obj in list)
            {
                TypeID typeId = TypeID.UnknownType;
                try
                {
                    string typeName = obj.GetType().Name;
                    typeId = (TypeID)Enum.Parse(typeof(TypeID), typeName);
                }
                catch (Exception) { }

                var info = new BuiltinObjectInfo(this, obj.GetLocalFileID(), typeId, obj);
                this.objects.Add(info.ID, info);
            }
        }

        public virtual IObjectInfo GetObjectInfo(long pathId)
        {
            BuiltinObjectInfo info = null;
            if (objects.TryGetValue(pathId, out info))
                return info;

            UnityEngine.Debug.LogWarningFormat("Object not found,AssetName:{0} ID:{1}", this.Name, pathId);
            return new MissingObjectInfo(this, pathId, TypeID.UnknownType);
        }
    }
}
#endif