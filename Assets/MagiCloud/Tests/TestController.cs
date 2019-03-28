using System;
using System.Collections.Generic;
using MagiCloud.Core;
using UnityEngine;
using MagiCloud.Core.Events;
using MagiCloud.RotateAndZoomTool;
using MagiCloud;

public class TestController : MonoBehaviour
{
    private MBehaviour behaviour;

    public ExecutionPriority executionPriority;
    public float executionOrder;

    public Transform center;

    private void Awake()
    {
        //behaviour = new MBehaviour(executionPriority, executionOrder, enabled);

        //behaviour.OnAwake_MBehaviour(() =>
        //{
        //    Debug.Log("Awake:" + gameObject);
        //});

        //behaviour.OnEnable_MBehaviour(() =>
        //{
        //    Debug.Log("Enable   " + gameObject);
        //});

        //behaviour.OnDisable_MBehaviour(() =>
        //{
        //    Debug.Log("OnDisable    " + gameObject);
        //});

        //behaviour.OnUpdate_MBehaviour(() =>
        //{
        //    Debug.Log("OnUpdate " + gameObject);
        //});

        //behaviour.OnDestroy_MBehaviour(() =>
        //{
        //    Debug.Log("OnDestroy");
        //});

        //MBehaviourController.AddBehaviour(behaviour);
    }

    private void Start()
    {
        RotateAndZoomManager.StartCameraZoom(center,2,20);
        RotateAndZoomManager.StartCameraAroundCenter(center);
        MSwitchManager.OnInitializeMode(OperateModeType.Move | OperateModeType.Rotate | OperateModeType.Zoom);
    }

    public void OnOperate()
    {
        MSwitchManager.CurrentMode =  OperateModeType.Move;
        //禁止UI
        MagiCloud.KGUI.UIShieldController.UnAllShileldAssign();

    }

    public void OnTool()
    {
        MSwitchManager.CurrentMode = OperateModeType.Tool;
        MagiCloud.KGUI.UIShieldController.ShieldDownward(0);
    }

    public void OnRotate()
    {
        MSwitchManager.CurrentMode = OperateModeType.Rotate;
        MagiCloud.KGUI.UIShieldController.ShieldDownward(0);
    }

    public void OnZoom()
    {
        MSwitchManager.CurrentMode = OperateModeType.Zoom;
        MagiCloud.KGUI.UIShieldController.ShieldDownward(0);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateAndZoomManager.StartCameraZoom(center, 2, 10);
            RotateAndZoomManager.StartCameraAroundCenter(center);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            RotateAndZoomManager.StopCameraAroundCenter();
            RotateAndZoomManager.StopCameraZoom();
        }
    }

    void onGrab(int handindex)
    {

    }

    void OnRelease(int handindex)
    {}

    void OnTargetEnter(int handindex)
    {}

    void OnTargetExit(int handindex)
    {

    }

    void ontest()
    {
        gameObject.AddGrabObject(onGrab);
        gameObject.RemoveGrabObject(onGrab);
        gameObject.RemoveGrabObjectAll();

        gameObject.AddReleaseObject(OnRelease);
        gameObject.RemoveReleaseObject(OnRelease);
        gameObject.RemoveReleaseObjectAll();

        gameObject.AddRayTargetEnter(OnTargetEnter);
        gameObject.RemoveRayTargetEnter(OnTargetEnter);
        gameObject.RemoveRayTargetEnter();


        gameObject.AddRayTargetExit(OnTargetExit);
        gameObject.RemoveRayTargetExit();
        gameObject.RemoveRayTargetExit(OnTargetExit);


        EventHandRay.AddListener(onRay);
        EventHandRay.RemoveListener(onRay);

        EventHandRays.AddListener(OnRays);
        EventHandRays.RemoveListener(OnRays);

        EventHandStart.AddListener(onstart);
        EventHandStart.RemoveListener(onstart);

        EventHandStop.AddListener(onstop);
        EventHandStop.RemoveListener(onstop);

        EventHandUIRay.AddListener(onRay);
        EventHandUIRay.RemoveListener(onRay);

        EventCameraZoom.AddListener(onzoom);
        EventCameraZoom.RemoveListener(onzoom);

        EventCameraRotate.AddListener(onrotate);
        EventCameraRotate.RemoveListener(onrotate);

        EventHandGrabObject.AddListener(ongrabobject);
        EventHandGrabObject.RemoveListener(ongrabobject);

        EventHandReleaseObject.AddListener(onreleaseobject);
        EventHandReleaseObject.RemoveListener(onreleaseobject);

        EventHandGrabObjectKey.AddListener(gameObject,onGrab);
        EventHandGrabObjectKey.RemoveListener(gameObject,onGrab);

        EventHandReleaseObjectKey.AddListener(gameObject, OnRelease);
        EventHandReleaseObjectKey.RemoveListener(gameObject, OnRelease);

        EventHandRayTarget.AddListener(onraytarget);
        EventHandRayTarget.RemoveListener(onraytarget);

        EventHandRayTargetEnter.AddListener(gameObject, OnTargetEnter);
        EventHandRayTargetEnter.RemoveListener(gameObject, OnTargetEnter);

        EventHandRayTargetExit.AddListener(gameObject, OnTargetExit);
        EventHandRayTargetExit.RemoveListener(gameObject, OnTargetExit);

        EventHandUIRayEnter.AddListener(gameObject, onuienter);
        EventHandUIRayEnter.RemoveListener(gameObject, onuienter);

        EventHandUIRayExit.AddListener(gameObject, onuiexit);
        EventHandUIRayExit.RemoveListener(gameObject, onuiexit);

        EventHandUIRay.AddListener(onRay);
        EventHandUIRay.RemoveListener(onRay);
        
    }
    void onuienter(int hand)
    {}

    void onuiexit(int hand)
    {}


    void onraytarget(RaycastHit hit,int hand)
    {}


    void ongrabobject(GameObject game,int hand)
    {}

    void onreleaseobject(GameObject game,int hand)
    {}

    void onrotate(Vector3 rotation)
    {}

    void onzoom(float value)
    {}

    void onstart(int hand)
    {}

    void onstop(int hand)
    {}


    void onRay(Ray ray,int hand)
    {

    }

    void OnRays(Ray ray,Ray uiRay,int hand)
    {}

    private void OnEnable()
    {
        //behaviour.IsEnable = true;
    }

    private void OnDisable()
    {
        //behaviour.IsEnable = false;
    }

    private void OnDestroy()
    {
        //behaviour.OnExcuteDestroy();
    }

}