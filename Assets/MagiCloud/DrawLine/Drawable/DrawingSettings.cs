using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DrawLine
{
    /// <summary>
    /// 设置画笔
    /// 负责人：邓荣
    /// </summary>
    public class DrawingSettings : MonoBehaviour
    {
        private bool isEraser;
        private Color preColor;
        public void SetMarkerWidth(float newWidth)
        {
            float temp = Mathf.Clamp(newWidth * 10, 1, float.MaxValue);
            Drawable.penWidth = (int)temp;
        }
        public void SetMarkerRed()
        {
            Color color = Color.red;
            Drawable.penColour = color;
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerGreen()
        {
            Color color = Color.green;
            Drawable.penColour = color;
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerBlue()
        {
            Color color = Color.blue;
            Drawable.penColour = color;
            Drawable.drawable.SetPenBrush();
        }
        public void SetEraser()
        {
            if (isEraser)
            {
                Drawable.penColour = preColor;
                Drawable.drawable.SetPenBrush();
                isEraser = false;
            }
            else
            {
                preColor = Drawable.penColour;
                Drawable.penColour = new Color(255f, 255f, 255f, 0);
                Drawable.drawable.SetPenBrush();
                isEraser = true;
            }
        }
        public void SetClear()
        {
            Drawable.drawable.ResetCanvas();
            Drawable.penColour = preColor;
            Drawable.drawable.SetPenBrush();
            isEraser = false;
        }
    }
}