using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DrawLine
{
    /// <summary>
    /// 画笔
    /// 负责人：邓荣
    /// </summary>
    public class Drawable : MonoBehaviour
    {
        public static Drawable drawable;                            //全局静态画笔对象
        public static Color penColour = Color.red;                  //画笔颜色
        public static int penWidth = 1;
        public delegate void BrushFunction(Vector2 world_position);
        public BrushFunction currentBrush;                          //绘制函数
        public Color resetColour = new Color(0, 0, 0, 0);           //重置为改颜色
        Sprite drawableSprite;                                      //画板精灵体
        Texture2D drawableTexture;                                  //画板精灵体的Texture2D
        Vector2 preDragPosition;                                    //之前鼠标拖拽的位置
        Color[] cleanColorArray;                                    //用于清空的填充颜色数组
        Color32[] curColor;                                         //画板精灵体的当前颜色数组(绘制是动态改变)

        void Awake()
        {
            drawable = this;                        //全局静态画笔对象
            currentBrush = PenBrush;                //默认笔刷
            drawableSprite = this.GetComponent<SpriteRenderer>().sprite;
            drawableTexture = drawableSprite.texture;

            cleanColorArray = new Color[(int)drawableSprite.rect.width * (int)drawableSprite.rect.height];  //重置时Texture2D的颜色
            for (int x = 0; x < cleanColorArray.Length; x++)
                cleanColorArray[x] = resetColour;
        }

        void Update()
        {
            bool mouse_held_down = Input.GetMouseButton(0);
            if (mouse_held_down)
            {
                Vector3 temp = Input.mousePosition;
                temp.x = Mathf.Clamp(temp.x, 0, Screen.width);
                temp.y = Mathf.Clamp(temp.y, 0, Screen.height);
                Vector2 mouse_world_position = MagiCloud.MUtility.UICamera.ScreenToWorldPoint(temp);
                currentBrush(mouse_world_position);
            }
            else if (!mouse_held_down)
            {
                preDragPosition = Vector2.zero;
            }
        }

        /// <summary>
        /// 基本工作流程模板
        /// </summary>
        /// <param name="worldPosition"></param>
        public void BrushTemplate(Vector2 worldPosition)
        {
            // 1. 将世界位置更改为像素坐标
            Vector2 pixelPos = WorldToPixelCoordinates(worldPosition);

            // 2. 确保像素数组的变量在此帧中更新
            curColor = drawableTexture.GetPixels32();

            if (preDragPosition == Vector2.zero)
            {
                
                MarkPixelsToColour(pixelPos, penWidth, penColour);              //第一次点击
            }
            else
            {
                
                ColourBetween(preDragPosition, pixelPos, penWidth, penColour);  //根据之前鼠标位置和当前鼠标位置进行操作
            }

            
            ApplyMarkedPixelChanges();                                          //把颜色应用到贴图

            // 4. 如果拖动，更新之前的位置
            preDragPosition = pixelPos;
        }

        /// <summary>
        /// 实时绘制函数，把worldPoint周围像素更改为静态penColour颜色
        /// </summary>
        /// <param name="worldPoint"></param>
        public void PenBrush(Vector2 worldPoint)
        {
            Vector2 pixelPos = WorldToPixelCoordinates(worldPoint);

            curColor = drawableTexture.GetPixels32();

            if (preDragPosition == Vector2.zero)
                MarkPixelsToColour(pixelPos, penWidth, penColour);                  // 如果是第一次拖动鼠标，只需在鼠标位置为像素着色
            else
                ColourBetween(preDragPosition, pixelPos, penWidth, penColour);      // 从上次更新调用的位置开始着色
            ApplyMarkedPixelChanges();

            preDragPosition = pixelPos;
        }


        //设置画笔绑定的绘制函数
        public void SetPenBrush()
        {
            currentBrush = PenBrush;
        }

        /// <summary>
        /// 两点之间插值画线
        /// 这个函数可以优化，使折线更平滑
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="width"></param>
        /// <param name="color"></param>
        public void ColourBetween(Vector2 startPoint, Vector2 endPoint, int width, Color color)
        {
            float distance = Vector2.Distance(startPoint, endPoint);    //需要插值的距离
            Vector2 direction = (startPoint - endPoint).normalized;     //插值方向

            Vector2 curPosition = startPoint;

            float lerp_steps = 1 / distance;                            //插值进度

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)         //插值
            {
                curPosition = Vector2.Lerp(startPoint, endPoint, lerp);
                MarkPixelsToColour(curPosition, width, color);
            }
        }




        /// <summary>
        /// 以penThickness（笔半径）为中心把像素中心及周围坐标标记为需要着色
        /// 并检查被标记着色的点是否超出范围
        /// </summary>
        /// <param name="centerPixel"></param>
        /// <param name="penThickness"></param>
        /// <param name="penColor"></param>
        public void MarkPixelsToColour(Vector2 centerPixel, int penThickness, Color penColor)
        {
            int centerX = (int)centerPixel.x;
            int centerY = (int)centerPixel.y;
            //int extraRadius = Mathf.Min(0, penThickness - 2);

            //根据centerPixel及penThickness计算出每一个方向需要着色的像素数
            for (int x = centerX - penThickness; x <= centerX + penThickness; x++)
            {
                if (x >= (int)drawableSprite.rect.width     //坐标时候超出rect大小
                    || x < 0)
                    continue;

                for (int y = centerY - penThickness; y <= centerY + penThickness; y++)
                {
                    MarkPixelToChange(x, y, penColor);
                }
            }
        }

        /// <summary>
        /// 把当前像素点颜色记录进curColor数组
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void MarkPixelToChange(int x, int y, Color color)
        {
            int arrayPos = y * (int)drawableSprite.rect.width + x;

            if (arrayPos > curColor.Length || arrayPos < 0)
                return;

            curColor[arrayPos] = color;
        }

        /// <summary>
        /// 把颜色应用到精灵体的Texture2D
        /// </summary>
        public void ApplyMarkedPixelChanges()
        {
            drawableTexture.SetPixels32(curColor);
            drawableTexture.Apply();
        }

        /// <summary>
        /// 直接为像素着色，比使用MarkPixelsToColour然后使用ApplyMarkedPixelChanges慢
        /// 因为SetPixels32比SetPixel快得多
        /// 以penThickness（笔半径）为中心把像素中心及周围着色
        /// </summary>
        /// <param name="centerPixel"></param>
        /// <param name="penThickness"></param>
        /// <param name="penColor"></param>
        public void ColourPixels(Vector2 centerPixel, int penThickness, Color penColor)
        {
            int centerX = (int)centerPixel.x;
            int centerY = (int)centerPixel.y;
            //int extraRadius = Mathf.Min(0, penThickness - 2);

            for (int x = centerX - penThickness; x <= centerX + penThickness; x++)
            {
                for (int y = centerY - penThickness; y <= centerY + penThickness; y++)
                {
                    drawableTexture.SetPixel(x, y, penColor);
                }
            }
            drawableTexture.Apply();
        }

        /// <summary>
        /// 世界点在精灵体的像素坐标位置
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2 WorldToPixelCoordinates(Vector2 worldPosition)
        {
            Vector3 localPos = transform.InverseTransformPoint(worldPosition);

            float pixelWidth = drawableSprite.rect.width;
            float pixelHeight = drawableSprite.rect.height;
            float unitsToPixels = pixelWidth / drawableSprite.bounds.size.x * transform.localScale.x;   //坐标像素值的比例

            float centeredX = localPos.x * unitsToPixels + pixelWidth / 2;                              //在精灵体的坐标原点下计算像素值
            float centeredY = localPos.y * unitsToPixels + pixelHeight / 2;

            Vector2 pixelPos = new Vector2(Mathf.RoundToInt(centeredX), Mathf.RoundToInt(centeredY));   //float转int
            return pixelPos;
        }


        /// <summary>
        /// 重置精灵体颜色数组为cleanColorArray
        /// </summary>
        public void ResetCanvas()
        {
            drawableTexture.SetPixels(cleanColorArray);
            drawableTexture.Apply();
        }

        private void OnDestroy()
        {
            ResetCanvas();
        }
    }
}