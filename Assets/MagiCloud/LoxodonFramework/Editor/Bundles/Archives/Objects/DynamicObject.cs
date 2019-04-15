#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Archives;
using System.Collections.Generic;
using System.Text;

namespace Loxodon.Framework.Bundles.Objects
{
    public class DynamicObject : Dictionary<string, object>
    {
        private readonly TypeNode typeNode;

        public DynamicObject(TypeNode typeNode)
        {
            this.typeNode = typeNode;
        }

        public virtual TypeNode TypeNode { get { return this.typeNode; } }

        public TypeNode GetFieldTypeNode(string fieldName)
        {
            foreach (TypeNode node in this.TypeNode.Children)
            {
                if (node.FieldName.Equals(fieldName))
                    return node;
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            this.ToString(buf, 0);
            return buf.ToString();
        }
    }

    public class UnityDynamicObject : DynamicObject
    {
        public virtual long ID { get; set; }

        public virtual long Size { get; set; }

        public virtual TypeTree TypeTree { get { return (TypeTree)this.TypeNode; } }

        public virtual ObjectInfo ObjectInfo { get; set; }

        public virtual string Name
        {
            get
            {
                switch (TypeTree.TypeID)
                {
                    case TypeID.Shader:
                        {
                            DynamicObject serializedShader = this["m_ParsedForm"] as DynamicObject;
                            if (serializedShader == null || !serializedShader.ContainsKey("m_Name"))
                                return string.Empty;
                            return (string)serializedShader["m_Name"];
                        }
                    default:
                        {
                            return this.ContainsKey("m_Name") ? (string)this["m_Name"] : string.Empty;
                        }
                }
            }
        }

        public UnityDynamicObject(TypeTree tree) : base(tree)
        {
        }
    }
}
#endif
