using UnityEngine;
using System.Collections;
using MagiCloud.Core.Events;
using System.Collections.Generic;
using MagiCloud;
using MagiCloud.Core;

public class TestEvent : MonoBehaviour
{
    public GameObject key;

    public GameObject testobject;

    private void Start()
    {
        //EventCameraZoom.AddListener((float lerp) =>
        //{
        //    Debug.Log("缩放：" + lerp);
        //});

        //EventCameraRotate.AddListener((Vector3 lerp) =>
        //{
        //    Debug.Log("旋转向量值：" + lerp);
        //});

        //EventHandGrip.AddListener((int handIndex) =>
        //{
        //    Debug.Log("握拳：" + handIndex);
        //});

        //EventHandIdle.AddListener((int handIndex) =>
        //{
        //    Debug.Log("释放：" + handIndex);
        //});

        //Test();

        //var result = IsContains(1, OperatePlatform.Mouse);

        //Debug.Log("result:" + result);

        EventHandGrabObject.AddListener((go, handIndex) =>
        {
            //Debug.Log("抓取手：" + handIndex + "  物体：" + go);
        });

        //EventHandRayTarget.AddListener((hit, handIndex) =>
        //{
        //    Debug.Log("手：" + handIndex + "  射线点：" + hit.collider.gameObject);
        //});

        EventHandRayTargetEnter.AddListener(key, (handIndex) =>
        {
            //Debug.Log("手：" + handIndex + "物体移入：" + key);
        });

        EventHandRayTargetExit.AddListener(key, (handIndex) =>
        {
            //Debug.Log("手：" + handIndex + "物体移出：" + key);
        });

        EventHandReleaseObject.AddListener((go, handIndex) =>
        {
            //Debug.Log("释放手：" + handIndex + "  物体：" + go);
        });

        Instantiate(testobject);

    }

    private Dictionary<OperateKey, int> Tests = new Dictionary<OperateKey, int>();

    void Test()
    {
        Add(0, OperatePlatform.Mouse);
        Add(1, OperatePlatform.Mouse);
        Add(2, OperatePlatform.Mouse);
        Add(3, OperatePlatform.Mouse);
        Add(4, OperatePlatform.Mouse);
    }

    void Add(int handIndex,OperatePlatform platform)
    {
        Tests.Add(new OperateKey(handIndex, platform), handIndex);
    }

    bool IsContains(int handIndex, OperatePlatform platform)
    {
        return Tests.ContainsKey(new OperateKey(handIndex, platform));
    }

}
