#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Archives;
using System;
using System.Text;

namespace Loxodon.Framework.Bundles.Objects
{
    public class PPtr
    {
        public int FileID { get; private set; }
        public long PathID { get; private set; }
        public string Name { get; private set; }
        public string TypeName { get { return Name.Replace("PPtr<", "").Replace(">", ""); } }

        public TypeID TypeID
        {
            get
            {
                try
                {
                    return (TypeID)Enum.Parse(typeof(TypeID), TypeName);
                }
                catch (Exception)
                {
                    return TypeID.UnknownType;
                }
            }
        }

        public PPtr(int fileID, long pathID, string name)
        {
            this.FileID = fileID;
            this.PathID = pathID;
            this.Name = name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PPtr))
                return false;

            var p = (PPtr)obj;
            return p.FileID == FileID && p.PathID == PathID;
        }

        public override int GetHashCode()
        {
            return this.PathID.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("{0} ", Name).Append("{ ");
            buf.AppendFormat("FileID:{0}, ", FileID);
            buf.AppendFormat("PathID:{0} ", PathID);
            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif