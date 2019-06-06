using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud.KGUI;
using System;

public class Example_DrawText : MonoBehaviour {

    public KGUI_Toggle Draw_Button;
    private bool isOpenDraw = false;

    private GameObject Draw_Backgroud;

	void Start ()
    {
        Draw_Button.OnValueChanged.AddListener(OnDrawClick);
    }

    private void OnDrawClick(bool arg0)
    {
        arg0 = !arg0;
        if (arg0)
        {
            Debug.Log("加载");
            Draw_Backgroud = GameObject.Instantiate(Resources.Load("Draw_Sprite_Background") as GameObject, Vector3.forward,Quaternion.identity);
        }
        else
        {
            Debug.Log("清空删除");
            Destroy(Draw_Backgroud);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
