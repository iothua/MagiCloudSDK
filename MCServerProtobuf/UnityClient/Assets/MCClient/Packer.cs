using System;
using System.Linq;
using System.Text;

namespace MagiCloudServer
{
    public class Packer
    {
        private byte[] data = new byte[1024];
        private int offset = 0;
        public void AddCount(int count)
        {
            offset+=count;
        }

        public byte[] Data => data;
        public int Offset => offset;

        public int RemingSize => data.Length-offset;

        /// <summary>
        /// 解码
        /// </summary>
        public void DeCoding()
        {
            while (true)
            {
                if (offset<=4) return;
                int count = BitConverter.ToInt32(data,0);
                if (offset-4>=count)
                {
                    string s = Encoding.UTF8.GetString(data,4,count);
                    Console.WriteLine("解析出数据："+s);
                    Array.Copy(data,count+4,data,0,offset-4-count);
                    offset-=(count+4);
                }
                else
                {
                    break;
                }
            }
        }

        public byte[] Coding(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int dataLength = dataBytes.Length;      //根据数据长度生成头部字节
            byte[] lengthBytes = BitConverter.GetBytes(dataLength);
            byte[] bytes = lengthBytes.Concat(dataBytes).ToArray();
            return bytes;
        }
    }
}
