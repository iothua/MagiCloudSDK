// This script can be used to control the system mouse - position of the mouse cursor and clicks
// Author: Akhmad Makhsadov
//

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// 该脚本可用于控制系统鼠标 - 鼠标光标的位置和点击
/// </summary>
public class MouseControl
{
    // Import function mouse_event() from WinApi
    //导入函数mouse_event()来自WinApi
    [DllImport("User32.dll")] 
    private static extern void mouse_event(MouseFlags dwFlags, int dx, int dy, int dwData, System.UIntPtr dwExtraInfo);

	private static float windowX = 0f;
	private static float windowY = 0f;


    // Flags needed to specify the mouse action 
    /// <summary>
    /// 需要指定鼠标操作的标志
    /// </summary>
    [System.Flags]
    private enum MouseFlags { 
        /// <summary>
        /// 移动
        /// </summary>
        Move = 0x0001,
        /// <summary>
        /// 左键按下
        /// </summary>
        LeftDown = 0x0002,
        /// <summary>
        /// 左键抬起
        /// </summary>
        LeftUp = 0x0004,
        /// <summary>
        /// 右键按下
        /// </summary>
        RightDown = 0x0008,
        /// <summary>
        /// 右键抬起
        /// </summary>
        RightUp = 0x0010,
        /// <summary>
        /// 绝对
        /// </summary>
        Absolute = 0x8000,
        }

    //    public static int MouseXSpeedCoef = 45000; // Cursor rate in Х direction
    //    public static int MouseYSpeedCoef = 45000; // Cursor rate in Y direction

    /// <summary>
    /// 共函数将鼠标指针移动到指定位置
    /// </summary>
    /// <param name="screenCoordinates">屏幕坐标</param>
    /// <param name="isConvertToFullScreen">转化为全屏</param>
    public static void MouseMove(Vector3 screenCoordinates, bool isConvertToFullScreen)
    {
		float screenX = screenCoordinates.x;
		float screenY = screenCoordinates.y;

		if(isConvertToFullScreen || Screen.fullScreen)
		{
			float screenResW = (float)Screen.currentResolution.width;
			float screenResH = (float)Screen.currentResolution.height;
			
			if(windowX == 0f && windowY == 0f)
			{
				windowX = (screenResW - Screen.width) / 2f;
				
				#if UNITY_EDITOR
				windowY = (screenResH - Screen.height - 36f) / 2f;
				#else
				windowY = (screenResH - Screen.height) / 2f;
				#endif
			}
			
			screenX = (windowX + screenCoordinates.x * Screen.width) / screenResW;
			screenY = (windowY + screenCoordinates.y * Screen.height) / screenResH;
		}

		Vector2 mouseCoords = new Vector2();
		mouseCoords.x = screenX * 65535;
		mouseCoords.y = (1.0f - screenY) * 65535;
		
        mouse_event(MouseFlags.Absolute | MouseFlags.Move, (int)mouseCoords.x, (int)mouseCoords.y, 0, System.UIntPtr.Zero);

//		Vector2 screenPos = new Vector2(screenCoordinates.x * Screen.width, screenCoordinates.y * Screen.height);
//		Vector3 mousePos = Input.mousePosition;
//		Vector2 scrMouseDiff = screenPos - (Vector2)mousePos;
//		Debug.Log("Screen: " + screenPos + ", Mouse: " + mousePos + ", Diff: " + scrMouseDiff);
    }

    // Public function to emulate a mouse button click (left button)
    /// <summary>
    /// 模拟鼠标按钮(左键)的公共函数
    /// </summary>
    public static void MouseClick()
    {
        mouse_event(MouseFlags.LeftDown, 0, 0, 0, System.UIntPtr.Zero);
        mouse_event(MouseFlags.LeftUp, 0, 0, 0, System.UIntPtr.Zero);
    }

    // Public function to emulate a mouse drag event (left button)
    /// <summary>
    /// 模拟鼠标拖动事件(左按钮)的公共函数
    /// </summary>
    public static void MouseDrag()
    {
        mouse_event(MouseFlags.LeftDown, 0, 0, 0, System.UIntPtr.Zero);
    }

    // Public function to emulate a mouse release event (left button)
    /// <summary>
    /// 模拟鼠标释放事件的公共函数(左按钮)
    /// </summary>
    public static void MouseRelease()
    {
        mouse_event(MouseFlags.LeftUp, 0, 0, 0, System.UIntPtr.Zero);
    }
	
}


