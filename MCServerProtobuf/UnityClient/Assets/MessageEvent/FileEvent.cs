using System;
using System.IO;

namespace MCServer
{
    /// <summary>
    /// 文件接收事件
    /// </summary>
    public class FileEvent
    {
        public FileEvent()
        {
        }
    }
    public class FileStruct
    {
        public string path;
        public string name;
        public string type;
        public byte[] context;

        public void Send()
        {

        }
    }
}
