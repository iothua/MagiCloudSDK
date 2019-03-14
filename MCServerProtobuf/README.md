## protobuf在c#、unity中使用教程
##### 1.搭建协议编辑器
  https://github.com/protocolbuffers/protobuf/releases 中下载对应c#包,解压.
  下载Camke,用camke导出protoc.exe,将exe路径配置到环境变量中

##### 2.生成c#版Protobuf的 DLL
  找到 **protobuf-3.7.0-rc-3\csharp\src\Google.Protobuf.sln** 用vs2017打开.
  在解决方案中选中 **oogle.Protobuf** 右键点击生成
  **protobuf-3.7.0-rc-3\csharp\src\Google.Protobuf\bin\Debug\net45** 中的dll可以加入到项目中使用


##### 3.项目目录结构
    Project
    --UnityProject
        --Assets...

    --ServerProject
        --Protobuf

    --ProtobufProject
        --Proto
  在ProtobufProject目录中创建 **ProtoToCSharp.bat** 文件
  ~~~ cmd
@echo off
cd Proto        //.proto存放目录
set client_dest_path="..\..\UnityProject\Assets\Network\Protobuf"       //导出到unity项目文件路径
set server_dest_path="..\..\ServerProject\Protobuf"                     //导出到Server项目文件路径
for %%i in (*.*) do protoc --csharp_out=%client_dest_path% %%i          
for %%i in (*.*) do protoc --csharp_out=%server_dest_path% %%i
echo success
pause

  ~~~




##### 4.编写 .proto 脚本(以消息协议为例)
``` proto
syntax = "proto3";          //声明protobuf版本，不加默认为 2.x,3.x版本才支持c#
package MagiCloudServer;    //package 相当于 namespace
message Message{            //message 相当于 class
int32  test=1;              //int32 对应 int ;1表示这个属性的序列号，而不是默认值
}
enum EnumTest{              //枚举：必须从0开始，且0表示空
    NONE=0;
    A=1;
    B=2;
}

```
##### 5.

服务端与客户端可采用异步通讯

protobuf协议
~~~ c#
public class ProtobufTool{
    //协议规则 长度+消息类型+内容
int type;
int length;
byte[] data;
}
~~~


Server

定义消息请求结构

系统设置请求 > 编码 > 接收请求 > 解码> 发送返回包>编码>收到返回包>解码,执行

心跳包：
1 客户端每隔一个时间间隔发生一个探测包给服务器
2 客户端发包时启动一个超时定时器
3 服务器端接收到检测包，应该回应一个包
4 如果客户机收到服务器的应答包，则说明服务器正常，删除超时定时器
5 如果客户端的超时定时器超时，依然没有收到应答包，则说明服务器挂了


制定消息协议
消息由 长度

##### protobuf编码：
~~~ csharp

... 
//编码
public byte[] Coding<T>(int type,T package) where T:IMessage
{
    byte[] data=  WriteLength(WriteType(SerializeProtobuf(package)))
    return data;
}

//序列化包
public byte[] SerializeProtobuf<T>(T package) where T:IMessage
{
    byte[] result = null;
    using (MemoryStream stream = new MemoryStream())
    {
        CodedOutputStream outputStream = new CodedOutputStream(stream);
        outputStream.WriteMessage(package);
        outputStream.Flush();
        result =stream.ToArray();
        return result;
    }
}
//写入消息类型
public byte[] WriteType(byte[] data)
{
    using (MemoryStream stream = new MemoryStream())
    {
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(type);
        writer.Write(data);
        writer.Flush();
        return stream.ToArray();
    }
}
//写入消息长度
public byte[] WriteLength(byte[] data)
{
    using (MemoryStream stream = new MemoryStream())
    {
        stream.Position=0;
        BinaryWriter writer = new BinaryWriter(stream);
        int len = data.Length;
        writer.Write(len);
        writer.Write(data);
        writer.Flush();
        return stream.ToArray();
    }
}
~~~
##### protobuf解码：
~~~ csharp

//解码
public T DeCoding(T package, byte[] data)
{
    return Deserialize(GetData(data).bytes);
}

//获得解码后的数据
public ProtobufTool GetData(byte[] data){
    
    ProtobufTool protobufData=new ProtobufTool();
    using (MemoryStream stream = new MemoryStream(data))
    {
        BinaryReader reader = new BinaryReader(stream);
        int dataLength = reader.ReadInt32();
        int typeId = reader.ReadInt32();
        byte[] pddata = reader.ReadBytes(dataLength-4);
        protobufData.type =typeId;
        protobufData.bytes=pddata;
        protobufData.byteLength =dataLength;
    }
    return  protobufData;
}

//将数据反序列化为包
public T Deserialize<T>(T package,byte[] data) where T:IMessage
{
    try
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            CodedInputStream inputStream = new CodedInputStream(data);
            inputStream.ReadMessage(package);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
    return package;
}
~~~


    
   