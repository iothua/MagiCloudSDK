using MagiCloud.KGUI;
using System;
using UnityEngine;

namespace MagiCloud.Features
{
    /// <summary>
    /// 物体标签
    /// </summary>
    [ExecuteInEditMode]
    public class LabelData :MonoBehaviour
    {
        public int id;
        public Sprite labelBackground;
        public string labelName;
        public Vector2 labelSize = new Vector2(170,50);
        public Vector3 labelOffset;

        public Vector2 peakZreaZ = new Vector2(0,float.MaxValue);  //Z轴合适区域 x = 5.55, y = 8.80;
        public Vector2 clearAreaZ = Vector2.one; //Z轴标签 x = 5, y = 11;

        public LabelType type = LabelType.总是显示;

        public GameObject appertaining;        //属于谁？

        public KGUI_Label label;
        public Color color = new Color(0.0588f,0.4241f,0.945f,1f);
        public int fontSize = 14;
        public FontStyle fontStyle = FontStyle.Bold;
        public bool useShadow = true;
        public bool useOutline = true;

        private void OnDestroy()
        {
            if (label!=null)
            {
                KGUI_LabelController.Instance.DestroyByLabelController(this);
            }
        }

    }

    public enum LabelType
    {
        总是显示,
        选中显示,
        不显示
    }
}
