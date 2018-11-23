using System;
using System.Collections.Generic;
using MagiCloud.Core;
using UnityEngine;

public class TestController : MonoBehaviour
{
    private MBehaviour behaviour;

    public ExecutionPriority executionPriority;
    public float executionOrder;

    private void Awake()
    {
        behaviour = new MBehaviour(executionPriority, executionOrder, enabled);

        behaviour.OnAwake(() =>
        {
            Debug.Log("Awake:" + gameObject);
        });

        behaviour.OnEnable(() =>
        {
            Debug.Log("Enable   " + gameObject);
        });

        behaviour.OnDisable(() =>
        {
            Debug.Log("OnDisable    " + gameObject);
        });

        behaviour.OnUpdate(() =>
        {
            Debug.Log("OnUpdate " + gameObject);
        });

        behaviour.OnDestroy(() =>
        {
            Debug.Log("OnDestroy");
        });

        MBehaviourController.AddBehaviour(behaviour);
    }

    private void OnEnable()
    {
        behaviour.IsEnable = true;
    }

    private void OnDisable()
    {
        behaviour.IsEnable = false;
    }

    private void OnDestroy()
    {
        behaviour.OnExcuteDestroy();
        MBehaviourController.RemoveBehaviour(behaviour);
    }

}