using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public string Name;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("打印：" + Name);
    }

}
