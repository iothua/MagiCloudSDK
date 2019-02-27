using System.IO;

namespace MagiCloud.TextToAudio
{
    public class WAV
    {
        private byte[] bytes;

        public WAV(string fileName) : this(GetBytes(fileName))
        {

        }

        public WAV(byte[] wav)
        {
            ChannelCount=wav[22];
            Frequency=BytesToInt(wav,24);
            int pos = 12;
            while (!(wav[pos]==100&&wav[pos+1]==97&&wav[pos+2]==116&&wav[pos+3]==97))
            {
                pos+=4;
                int size = wav[pos]+wav[pos+1]*256+wav[pos+2]*65536+wav[pos+3]*16777216;
                pos+=4+size;
            }
            pos+=8;
            SampleCount=(wav.Length-pos)/2;
            if (ChannelCount==2) SampleCount/=2;
            LeftChannel=new float[SampleCount];
            if (ChannelCount==2) RightChannel=new float[SampleCount];
            else RightChannel=null;
            int i = 0;
            while (pos<wav.Length)
            {
                LeftChannel[i]=BytesToFloat(wav[pos],wav[pos+1]);
                pos+=2;
                if (ChannelCount==2)
                {
                    RightChannel[i]=BytesToFloat(wav[pos],wav[pos+1]);
                    pos+=2;
                }
                i++;
            }
        }

        public int SampleCount { get; internal set; }
        public int Frequency { get; internal set; }
        public float[] LeftChannel { get; internal set; }
        public float[] RightChannel { get; internal set; }
        public int ChannelCount { get; internal set; }

        /// <summary>
        /// bytes转成float
        /// </summary>
        /// <param name="firstByte"></param>
        /// <param name="secondByte"></param>
        /// <returns></returns>
        private static float BytesToFloat(byte firstByte,byte secondByte)
        {
            short s = (short)((secondByte<<8)|firstByte);
            return s/32768.0F;
        }

        private static int BytesToInt(byte[] bytes,int offset = 0)
        {
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                value|=((int)bytes[offset+i])<<(i*8);
            }
            return value;
        }
        private static byte[] GetBytes(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }
        public override string ToString()
        {
            return string.Format("[WAV:LeftChannel={0},RightChannel={1},ChannelCount={2},SampleCount={3},Frequency={4}]",LeftChannel,RightChannel,ChannelCount,SampleCount,Frequency);
        }
    }
}