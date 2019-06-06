using MagiCloud;
using MagiCloud.Core.Events;
using MagiCloud.KGUI;
using MagiCloud.UIFrame;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FreeDraw
{
    /// <summary>
    ///     /* 1。将此附加到启用读/写的sprite图像
    ///     2。设置要在光线投射中使用的绘图层
    ///     3。将一个二维对撞机（就像一个盒子对撞机2d）附加到这个精灵上
    ///     4。按住鼠标左键绘制此纹理！
    ///     负责人：苏金明
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    public class Drawable : MonoBehaviour
    {
        // 画笔颜色
        public static Color Pen_Colour = Color.red;             // 更改这些设置以更改默认图形设置
        // 画笔宽度（实际上是半径，以像素为单位）
        public static int Pen_Width = 3;
        public delegate void Brush_Function(Vector2 world_position);
        //这是当左键单击发生时调用的函数。
        //传入您自己的自定义画笔以更改画笔类型
        //在唤醒方法中设置默认函数
        public Brush_Function current_brush;

        public LayerMask Drawing_Layers;

        public bool Reset_Canvas_On_Play = true;
        public static Drawable drawable;

        // 画布每次重置为 0000的颜色
        private Color Reset_Colour = new Color(0, 0, 0, 0);         // 默认情况下，将画布重置为透明

        private Sprite drawable_sprite;
        private Texture2D drawable_texture;                         //画板精灵体的Texture2D

        private Vector2 previous_drag_position;                     //之前鼠标拖拽的位置
        private Color[] clean_colours_array;                        //全部像素颜色 空白颜色
        private Color32[] cur_colors;                               //当前需要改变颜色的位置
        private Color32[] start_colors;                             //初始空白画面，用作清空处理使用
        private bool mouse_was_previously_held_down = false;
        private bool no_drawing_on_current_drag = false;

        private int handIndex = -1;
        private bool IsPressed = false;
        private GameObject ray_Object;

        private bool isOnClane = false;  //是否按下了
        private OperateModeType operateType;

        private GameObject resource_Draw_Canvas;    //画笔辅助工具
        private UI_ButtonEmbed Draw_button;
        private bool isDraw = false;


        private Color32[] last = new Color32[1];   //最后一个  用作撤销使用
        List<Color32[]> recordColors = new List<Color32[]>(1);
        // Color32[] backColors = new Color32[1] ;        //用于撤销之前再还原

        private List<Color32[]> RestoreColors = new List<Color32[]>(1); //用于撤销之前再还原



        void Awake()
        {
            drawable = this;
            // 此处设置默认画笔
            current_brush = PenBrush;

            drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
            drawable_texture = drawable_sprite.texture;

            resource_Draw_Canvas = GameObject.Instantiate(Resources.Load("KGUI_Canvas_Draw") as GameObject);
            resource_Draw_Canvas.GetComponent<RectTransform>().transform.position = new Vector3(0, 0, 100);

            // 初始化要使用的干净像素
            clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
            for (int x = 0; x < clean_colours_array.Length; x++)
                clean_colours_array[x] = Reset_Colour;

            // 当我们在编辑器中单击“播放”时，是否应该重置画布图像？
            if (Reset_Canvas_On_Play)
                ResetCanvas();

            start_colors = drawable_texture.GetPixels32();

            //最后一帧画面，也就是第一帧记录的画面
            //last = drawable_texture.GetPixels32();
            //recordColors.Add(last);
        }
        private OperateModeType curOperateModeType;

        private void Start()
        {
            curOperateModeType = MSwitchManager.CurrentMode;
            MSwitchManager.CurrentMode = OperateModeType.Tool;
            EventHandGrip.AddListener(OnGrip);
            EventHandIdle.AddListener(OnIdle);
            MSwitchManager.AddListener(OnSwitch);

            GetComponent<SpriteRenderer>().enabled = false;
            Draw_button = UIManager.Instance.GetUIComponent<UI_ButtonEmbed>("Draw");
        }


        private void OnSwitch(OperateModeType obj)
        {
            //if (operateType != OperateModeType.Tool)
            //SetClear();
        }

        private void OnIdle(int handIndex)
        {
            if (this.handIndex != handIndex) return;

            IsPressed = false;
            previous_drag_position = Vector2.zero;
            //如果是在画线就记录数据
            if (isDraw)
            {
                Debug.Log("记录");
                //鼠标释放的时候记录数据
                Record();
            }
            isDraw = false;

        }

        private void OnGrip(int handIndex)
        {
            if (this.handIndex != -1 && this.handIndex != handIndex) return;
            IsPressed = true;
            this.handIndex = handIndex;

            MagiCloud.UIOperate uIOperate = MOperateManager.GetUIOperate(handIndex);
            ray_Object = uIOperate.rayObject;
        }

        //////////////////////update////////////////////////////////////////

        //检测用户单击的时间，然后调用相应的函数
        void Update()
        {
            if (IsPressed) // && MSwitchManager.CurrentMode == OperateModeType.Tool
            {
                //发出摄像检测是否在画图还是按钮上
                Vector3 temp = MOperateManager.GetHandScreenPoint(handIndex);

                temp.x = Mathf.Clamp(temp.x, 0, Screen.width);
                temp.y = Mathf.Clamp(temp.y, 0, Screen.height);

                Vector2 mouse_world_position = MUtility.UICamera.ScreenToWorldPoint(temp);

                if (ray_Object == null)  //判断是否按在按钮上上
                {
                    //射线检测
                    Vector3 rayVector3 = temp + new Vector3(0, 0, 1000);
                    Ray ray = MUtility.UICamera.ScreenPointToRay(rayVector3);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 10000, 1 << 5))
                        if (hit.collider.gameObject == this.gameObject)
                        {
                            isDraw = true;
                            current_brush(mouse_world_position);
                        }
                }
            }
        }

        // 默认画笔类型。有宽度和颜色。
        //以世界坐标传递一个点
        //将世界周围的像素点更改为静态钢笔颜色
        public void PenBrush(Vector2 world_point)
        {
            //思路：
            // pixel_pos 获取到世界坐标，获取到屏幕的值
            //第一次点击，第一帧 ！！previous_drag_position 就是画线的起始点，总是零点，然后获取到屏幕位置，重新赋值  
            //拖动就获取到当前的位置，画成一条线，重新赋值改变颜色 ColourBetween
            //最后一步 赋值 当前的颜色
            // previous_drag_position  = 当然位置 pixel_pos 


            //获得世界坐标对应的像素
            Vector2 pixel_pos = WorldToPixelCoordinates(world_point);

            cur_colors = drawable_texture.GetPixels32(); //获得像素所有值

            if (previous_drag_position == Vector2.zero)      //第一次点击，总是画线的起始点，然后获取到屏幕位置，重新赋值，
            {
                // 如果这是我们第一次拖拽这张图片，只需在鼠标位置加上像素。
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else                                           //
            {
                // 两点之间颜色的改变
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ApplyMarkedPixelChanges();
            previous_drag_position = pixel_pos;
        }


        // UI使用的辅助方法来设置用户想要的画笔
        //为实现的任何新画笔创建一个新的画笔
        public void SetPenBrush()
        {
            // PenBrush是要设置为当前画笔的方法的名称
            current_brush = PenBrush;
        }


        /// <summary>
        /// 将像素的颜色设置为从起点到终点的直线，以确保中间的所有内容都是彩色的。
        /// </summary>
        /// <param name="start_point">起始点</param>
        /// <param name="end_point">终点</param>
        /// <param name="width">宽度</param>
        /// <param name="color">需要改变的颜色</param>
        public void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
        {
            // 从开始到结束的距离
            float distance = Vector2.Distance(start_point, end_point); //需要插值的距离
            Vector2 direction = (start_point - end_point).normalized;  //插值方向

            Vector2 cur_position = start_point;

            // 根据自上次更新以来经过的时间量，计算应该在起始点和结束点之间插入多少次
            float lerp_steps = 1 / distance;                            //插值进度

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(start_point, end_point, lerp);
                //将像素标记为彩色
                MarkPixelsToColour(cur_position, width, color);
            }
        }


        /// <summary>
        /// 以penThickness（笔半径）为中心把像素中心及周围坐标标记为需要着色
        /// 并检查被标记着色的点是否超出范围
        /// </summary>
        /// <param name="center_pixel">开始点到结束点</param>
        /// <param name="pen_thickness">画笔的宽度</param>
        /// <param name="color_of_pen">画笔的颜色</param>
        public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // 计算出我们需要在每个方向（x和y）上色多少像素。
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                // 检查X是否环绕图像，这样我们就不会在图像的另一侧绘制像素。
                if (x >= (int)drawable_sprite.rect.width || x < 0)
                    continue;

                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    if (y != 1080)  //如果超过了1080 就不用修改了
                        MarkPixelToChange(x, y, color_of_pen);
                }
            }
        }



        /// <summary>
        /// 把当前像素点颜色记录进curColor数组 获得标记要更改的像素
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void MarkPixelToChange(int x, int y, Color color)
        {
            // 需要将x和y坐标转换为数组的平面坐标
            int array_pos = y * (int)drawable_sprite.rect.width + x;

            // 检查这个位置是否有效
            if (array_pos > cur_colors.Length || array_pos < 0)
                return;

            cur_colors[array_pos] = color;
        }



        /// <summary>
        /// 更改应用标记的像素
        /// </summary>
        public void ApplyMarkedPixelChanges()
        {
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
        }

        /// <summary>
        /// 世界点在精灵体的像素坐标位置
        /// </summary>
        /// <param name="world_position"></param>
        /// <returns></returns>
        public Vector2 WorldToPixelCoordinates(Vector2 world_position)
        {
            // 将坐标更改为此图像的本地坐标
            Vector3 local_pos = transform.InverseTransformPoint(world_position);

            // 将这些更改为像素坐标
            float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;

            //需要把我们的坐标居中
            float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
            float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

            // 将当前鼠标位置舍入到最近的像素
            Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            return pixel_pos;
        }

        // 
        // setpixels32比setpixel快得多
        // 基于中心厚度（笔半径），中心像素和中心像素周围的像素数目
        /// <summary>
        /// 直接为像素着色，比使用MarkPixelsToColour然后使用ApplyMarkedPixelChanges慢
        /// setpixels32比setpixel快得多
        /// 以penThickness（笔半径）为中心把像素中心及周围着色
        /// </summary>
        /// <param name="center_pixel">当前画笔的位置</param>
        /// <param name="pen_thickness">宽度</param>
        /// <param name="color_of_pen"></param>
        public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // 计算出我们需要在每个方向（x和y）上色多少像素。
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    drawable_texture.SetPixel(x, y, color_of_pen);
                }
            }

            drawable_texture.Apply();
        }


        #region 功能按键

        /// <summary>
        /// 记录数据
        /// </summary>
        public void Record()
        {
            last = drawable_texture.GetPixels32();    //记录当前画面线在哪
            recordColors.Add(last);
            RestoreColors.Clear();
        }


        /// <summary>
        /// 撤销最后一个
        /// </summary>
        public void RemoveLastColor()
        {
            if (recordColors.Count == 0) return;
            Debug.Log("撤销的数量" + recordColors.Count);
            if (isOnClane == false)
                RestoreColors.Add(drawable_texture.GetPixels32()); // 再删除之前添加到还原记录 0 -1 -2
            else
                isOnClane = false;

            if (recordColors.Count > 1)      // 默认里面有一个空白的（start 执行添加） 从第二个开始才是记录第一笔
            {
                int index = recordColors.Count - 2;

                Color32[] color = recordColors[index];
                drawable_texture.SetPixels32(color);
                drawable_texture.Apply();
                //删除最后一个画线
                // last = recordColors[recordColors.Count - 1];    
                last = drawable_texture.GetPixels32();
            }
            else if (recordColors.Count == 1)
            {
                drawable_texture.SetPixels32(start_colors);
                drawable_texture.Apply();
            }

            recordColors.RemoveAt(recordColors.Count - 1);
        }

        /// <summary>
        /// 还原 
        /// </summary>
        public void RestoreBackColor()
        {

            if (RestoreColors.Count <= 0) return;
            Debug.Log("还原的:" + RestoreColors.Count);
            //得到当前画面的画线
            int index = RestoreColors.Count - 1;
            Color32[] color = RestoreColors[index];  //应用复原 0 - 1 -2
            drawable_texture.SetPixels32(color);
            drawable_texture.Apply();


            last = drawable_texture.GetPixels32();   //在还原之前，得到记录的值
            //复原后，需要把记录的数据添加一次 ，添加到复原的画面
            recordColors.Add(last);
            RestoreColors.RemoveAt(RestoreColors.Count - 1);

            Debug.Log("还原后再添加到撤销:" + recordColors.Count);
            //last = RestoreColors[recordColors.Count - 1];
        }


        /// <summary>
        /// 清空当面面板的记录数据
        /// </summary>
        public void Clane_LastColor()
        {
            //recordColors.Clear();

            drawable_texture.SetPixels32(start_colors);
            drawable_texture.Apply();
            if (!recordColors.Contains(start_colors))
            {
                recordColors.Add(start_colors);
            }
            Debug.Log("清除之后" + recordColors.Count);
            isOnClane = true;
            RestoreColors.Clear();
        }

        /// <summary>
        /// 重置功能  将每个像素更改为重置颜色
        /// </summary>
        public void ResetCanvas()
        {
            drawable_texture.SetPixels(clean_colours_array);
            drawable_texture.Apply();
            RestoreColors.Clear();
            recordColors.Clear();
        }

        #endregion

        public void Exit()
        {
            Draw_button.GetComponent<KGUI_Toggle>().IsValue = true;
        }

        private void OnDisable()
        {
            EventHandGrip.RemoveListener(OnGrip);
            EventHandIdle.RemoveListener(OnIdle);
            MSwitchManager.RemoveListener(OnSwitch);
        }

        private void OnDestroy()
        {
            ResetCanvas();
            Destroy(resource_Draw_Canvas);
            MSwitchManager.CurrentMode = curOperateModeType;
        }

    }
}
