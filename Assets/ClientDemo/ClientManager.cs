using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.NetWorks;
using MagiCloud.NetWorks.Client;
using UnityEngine.UI;

public class ClientManager :MonoBehaviour
{
    private MessageEvent messageEvent;

    public Text txtExperimentPath;

    private void Start()
    {
        messageEvent = new MessageEvent(ClientNetManager.connetion.messageDistribution);
        messageEvent.experimentEvent.txtExperimentPath = txtExperimentPath;
        ClientNetManager.connetion.Connect("127.0.0.1",8888);

    }

    public void OnBack()
    {
        messageEvent.experimentEvent.SendExpinfoRes();
        messageEvent.wakeupEvent.SetMinWindow();
    }

    private void Update()
    {
        ClientNetManager.Update();
    }

    private void OnDestroy()
    {
        ClientNetManager.connetion.Close();
    }
}
