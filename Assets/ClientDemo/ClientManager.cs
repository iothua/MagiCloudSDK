using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.NetWorks;
using MagiCloud.NetWorks.Client;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour
{
    private MessageEvent messageEvent;

    public Text txtExperimentPath;

    private void Start()
    {
        messageEvent = new MessageEvent();
        messageEvent.experimentEvent.txtExperimentPath = txtExperimentPath;
        NetManager.connetion.Connect("127.0.0.1", 8888);

    }

    public void OnBack()
    {
        messageEvent.experimentEvent.SendExpinfoRes();

    }

    private void Update()
    {
        NetManager.Update();
    }

    private void OnDestroy()
    {
        NetManager.connetion.Close();
    }
}
