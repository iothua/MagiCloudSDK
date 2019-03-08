using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServer
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

       

    }
}
