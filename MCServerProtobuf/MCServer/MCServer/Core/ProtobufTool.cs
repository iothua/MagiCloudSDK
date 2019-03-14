using Google.Protobuf;
using System;
using System.IO;
using System.Linq;

namespace MCServer
{
    public class ProtobufTool
    {
        public byte[] bytes;    //消息缓存
        public int byteLength;  //消息长度
        public int type;        //消息类型
        /// <summary>
        /// 将文件序列化为 byte[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        public byte[] Serialize<T>(T msg) where T : IMessage
        {
            byte[] result = null;
            using (MemoryStream stream = new MemoryStream())
            {
                CodedOutputStream outputStream = new CodedOutputStream(stream);
                outputStream.WriteMessage(msg);
                outputStream.Flush();
                result =stream.ToArray();
                return result;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="data"></param>
        public T DeSerialize<T>(T t,byte[] data) where T : IMessage
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    CodedInputStream inputStream = new CodedInputStream(data);
                    inputStream.ReadMessage(t);
                    return t;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return t; 
            }
        }

        /// <summary>
        /// 创建协议数据 类型+内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="pbuf"></param>
        /// <returns></returns>
        public byte[] CreatData<T>(int type,T pbuf) where T : IMessage
        {
            byte[] pbdata = Serialize(pbuf);
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(type);
                this.type=type;
                writer.Write(pbdata);
                writer.Flush();
                byte[] result = WriteMessage(stream.ToArray());
                if (bytes==null)
                    bytes=result;
                else
                    bytes=bytes.Concat(result).ToArray();
                byteLength=bytes.Length;
                return bytes;
            }
        }

        /// <summary>
        /// 写入数据    //长度+数据
        /// </summary>
        /// <param name="msg">类型+内容</param>
        /// <returns></returns>
        private byte[] WriteMessage(byte[] msg)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Position=0;
                BinaryWriter writer = new BinaryWriter(stream);
                int len = msg.Length;
                writer.Write(len);
                writer.Write(msg);
                writer.Flush();
                return stream.ToArray();
            }
        }


        /// <summary>
        /// 获取协议
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ProtobufTool Read(byte[] data)
        {
            ProtobufTool protobuf = new ProtobufTool();
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(stream);
                int dataLength = reader.ReadInt32();
                int typeId = reader.ReadInt32();
                //  int len = reader.ReadInt32();
                byte[] pddata = reader.ReadBytes(dataLength-4);
                //   byte[] pddata = reader.ReadBytes(reader.ReadInt32());
                protobuf.type =typeId;
                protobuf.bytes=pddata;
                protobuf.byteLength =dataLength;
            }
            return protobuf;
        }
    }



}
