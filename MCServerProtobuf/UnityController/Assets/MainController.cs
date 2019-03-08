using MagiCloudServer;
using MCServer;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 总控制端
/// </summary>
public class MainController :MonoBehaviour
{
    public int id;
    SettingReq req;
    public Button low;
    public Button middle;
    public Button hight;
    public Slider volume;
    NetManager netManager;
    void Start()
    {
        netManager=new NetManager();
        netManager.connetion.Connect("127.0.0.1",8888); //192.168.1.24
        low.onClick.AddListener(() => netManager.SendQulitySet(SystemSettingInfo.Types.Performance.Low));
        middle.onClick.AddListener(() => netManager.SendQulitySet(SystemSettingInfo.Types.Performance.Middle));
        hight.onClick.AddListener(() => netManager.SendQulitySet(SystemSettingInfo.Types.Performance.Hight));
        volume.onValueChanged.AddListener(netManager.SendVolumeSet);
    }

    void Update()
    {
        netManager.Update();
    }




    private void OnDestroy()
    {
        netManager.connetion.Close();
    }
}
