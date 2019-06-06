using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FreeDraw
{
    // Helper methods used to set drawing settings
    /// <summary>
    /// 用于设置绘图设置的帮助器方法
    /// </summary>
    public class DrawingSettings : MonoBehaviour
    {
        public static bool isCursorOverUI = false;
        public float Transparency = 1f;

        // Changing pen settings is easy as changing the static properties Drawable.Pen_Colour and Drawable.Pen_Width
        /// <summary>
        /// 更改笔设置很容易,更改静态属性drawable.pen_颜色和drawable.pen_宽度
        /// </summary>
        /// <param name="new_color"></param>
        public void SetMarkerColour(Color new_color)
        {
            Drawable.Pen_Colour = new_color;
        }

        //设置大小
        // 新宽度是以像素为单位的半径
        /// <summary>
        /// 更改静态属性 drawable.pen_宽度
        /// </summary>
        /// <param name="new_width"></param>
        public void SetMarkerWidth1(int new_width)
        {
            Drawable.Pen_Width = new_width ;
        }

        public void SetMarkerWidth(float new_width)
        {
            float width =  Mathf.Clamp(new_width, 0.3f,1);

            SetMarkerWidth1(Mathf.RoundToInt(width *10));
           
        }


        //改变大小 369
        public void SetMarkerSize_Max_9()
        {
            SetMarkerWidth1(Mathf.RoundToInt(9));
        }

        public void SetMarkerSize_Medium_6()
        {
            SetMarkerWidth1(Mathf.RoundToInt(6));
        }

        public void SetMarkerSize_Min_3()
        {
            SetMarkerWidth1(Mathf.RoundToInt(3));
        }

        // 调用这些来更改笔设置 （直接拉到按钮上）

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="amount"></param>
        public void SetTransparency(float amount)
        {
            Transparency = amount;
            Color c = Drawable.Pen_Colour;
            c.a = amount;
            Drawable.Pen_Colour = c;
        }

        /// <summary>
        /// 设置成红色
        /// </summary>
        public void SetMarkerRed()
        {
            Color c = Color.red;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }

        /// <summary>
        /// 白色
        /// </summary>
        public void SetMarkerwhite()
        {
            Color c = Color.white;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }

        /// <summary>
        /// 蓝色
        /// </summary>
        public void SetMarkerBlue()
        {
            Color c = Color.blue;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }

        /// <summary>
        /// 橡皮擦功能
        /// </summary>
        public void SetEraser()
        {
            //SetMarkerColour(new Color(255f, 255f, 255f, 1f));
            Color c = Drawable.Pen_Colour;
            c.a = 0;
            Drawable.Pen_Colour = c;
        }

        //橡皮擦功能
        public void PartialSetEraser()
        {
            SetMarkerColour(new Color(255f, 255f, 255f, 0.5f));
        }

        /// <summary>
        /// 撤销最后一个(返回)
        /// </summary>
        public void Set_RemoveLastColor()
        {
            Drawable.drawable.RemoveLastColor();
        }

        /// <summary>
        /// 还原（前进）
        /// </summary>
        public void Set_RestoreBackColor()
        {
            Drawable.drawable.RestoreBackColor();
        }

        public void Set_Clear_CurSprite()
        {
            Drawable.drawable.Clane_LastColor();
        }

        public void Set_Exit()
        {
            Drawable.drawable.Exit();
        }

    }
}