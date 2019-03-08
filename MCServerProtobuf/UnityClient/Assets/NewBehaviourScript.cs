using MagiCloudServer;
using MCServer;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript :MonoBehaviour
{
    public int id;
    SettingReq req;
    public Text qulity;
    public Slider volume;
    void Start()
    {
        req = new SettingReq();
        NetManager.connetion.Connect("127.0.0.1",8888); //192.168.1.24
        MessageDistribution.AddListener((int)EnumCmdID.SettingReq,SettingReaCallback);
    }

    private void SettingReaCallback(ProtobufTool protobuf)
    {
        using (MemoryStream stream = new MemoryStream(protobuf.bytes))
        {
            protobuf.DeSerialize(req,protobuf.bytes);
            if (qulity!=null) qulity.text=req.Info.Type.ToString();
            if (volume!=null) volume.value=req.Info.Volume*0.01f;
        }
    }

    void Update()
    {
        NetManager.Update();
    }




    private void OnDestroy()
    {
        NetManager.connetion.Close();
    }
}
