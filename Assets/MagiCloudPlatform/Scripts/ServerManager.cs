using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.NetWorks.Server;
using MagiCloud.NetWorks;

/// <summary>
/// 服务端
/// </summary>
public class ServerManager :MonoBehaviour
{

    //客户端目录
    public string clientPath = @"\Build\Client\ClientDemo.exe";

    private string serverPath = Application.streamingAssetsPath + "/mcserver/MCServer.exe";

    private ProcessHelper helper;
    private MessageEvent messageEvent;

    private void Start()
    {
        StartServerProcess();
        messageEvent = new MessageEvent(ServerNetManager.connetion.messageDistribution);
        ServerNetManager.connetion.Connect("127.0.0.1",8888);
    }

    public void AddEvent()
    {
        messageEvent.experimentEvent.SendReq(() =>
        {
            messageEvent.wakeupEvent.OpenExe(clientPath);
            messageEvent.wakeupEvent.SendWakeup();
        },new ExperimentInfo()
        {
            Id = 0,
            Name = "高锰酸钾制取氧气",
            ExperimentPath = "ClientDemo/Prefabs/TestDemo.prefab",
            Own = "科学",
            IsBack = false
        });

        messageEvent.clientConnectEvent.Add(1,() =>
        {
            messageEvent.experimentEvent.SendReq(() =>
            {
                messageEvent.wakeupEvent.SendWakeup();
                //messageEvent.wakeupEvent.OpenExe(clientPath);
            },new ExperimentInfo()
            {
                Id = 0,
                Name = "高锰酸钾制取氧气",
                ExperimentPath = "ClientDemo/Prefabs/TestDemo.prefab",
                Own = "科学",
                IsBack = false
            });
        });
    }

    private void OnConnect()
    { }

    /// <summary>
    /// 启动服务器进程
    /// </summary>
    private void StartServerProcess()
    {
        helper = new ProcessHelper();
        helper.OpenExe(serverPath,true);
        helper.p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
    }

    private void Update()
    {
        ServerNetManager.Update();
    }

    private void OnApplicationQuit()
    {
        messageEvent.wakeupEvent.Exit();
    }

    private void OnDestroy()
    {
        ServerNetManager.connetion.Close();
        helper.Exit();
    }
}
