using UnityEditor;
using System;
using UnityEngine;

namespace Loxodon.Framework.Bundles.Editors
{
    public abstract class Panel
    {
        private readonly EditorWindow parent;

        public Panel(EditorWindow parent)
        {
            this.parent = parent;
        }

        public EditorWindow Parent { get { return this.parent; } }

        public virtual void Repaint()
        {
            this.parent.Repaint();
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }

        public abstract void OnGUI(Rect rect);
    }
}
