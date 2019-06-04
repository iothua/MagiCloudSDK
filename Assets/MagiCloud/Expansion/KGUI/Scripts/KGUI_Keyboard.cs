using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MagiCloud.KGUI
{
    public static class KGUI_Keyboard
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void Keybd_event(

       byte bvk,//虚拟键值 ESC键对应的是27

       byte bScan,//0

       int dwFlags,//0为按下，1按住，2释放

       int dwExtraInfo//0

       );


    }
}
