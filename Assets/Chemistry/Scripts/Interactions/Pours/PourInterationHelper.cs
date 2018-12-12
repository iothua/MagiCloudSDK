using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Chemistry.Interactions
{


    /// <summary>
    /// 倒水交互控制
    /// 开发者：阮榆皓
    /// </summary>
    //[RequireComponent(typeof(InteractionPourWater))]
    public class PourInterationHelper : MonoBehaviour
    {
        [Header("读条UI对象")]
        public GameObject ReadingImgObj;
        [Header("读条UI预制体路径")]
        public string PourImgPrefabsPath = "PourImg";

        //[Header("倒水点")]--自己
        //public Transform PourPt;
        [Header("倒水仪器局部坐标")]
        public Vector3 localPos = Vector3.zero;
        [SerializeField, Header("倒水仪器局部旋转值")]
        public Vector3 localRot = Vector3.zero;

        [Header("进入倒水操作的读条时间")]
        public float LimitTime = 0.5f;

        //[HideInInspector]
        //public float curTime = 0.0f;

        [Header("离开倒水范围的距离(距离至少为两个距离检测点半径和+仪器半径)")]
        public float LeaveDistance = 400.0f;

        [Header("倒水旋转的速度")]
        public float RotSpeed = 1.0f;

        [Header("倒水水流的速度(ml/帧)")]
        public float WaterSpeed = 1.0f;


        //public void ResetAllData()
        //{
        //    curTime = 0.0f;
        //}

        //旋转中心点
        //public Transform RotPt;

        //private void Awake()
        //{
        //    if (OnEnterPour == null)
        //    {
        //        OnEnterPour = new EventPour();
        //    }
        //    if (OnBeginPour == null)
        //    {
        //        OnBeginPour = new EventPour();
        //    }
        //    if (OnStayPour == null)
        //    {
        //        OnStayPour = new EventPour();
        //    }
        //    if (OnEndPour == null)
        //    {
        //        OnEndPour = new EventPour();
        //    }
        //    if (OnExitPour == null)
        //    {
        //        OnExitPour = new EventPour();
        //    }
        //}


        // Update is called once per frame
        //void Update()
        //{
        //    if (startTimer)
        //    {
        //        curTimer += Time.deltaTime;
        //        //读条
        //        ReadingImg.fillAmount = Mathf.Clamp((1.0f - curTimer / LimitTime), 0.0f, 1.0f);
        //        if (curTimer>=LimitTime)
        //        {
        //            //吸附

        //            //

        //        }
        //    }
        //}


    }
}
