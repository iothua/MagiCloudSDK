using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Chemistry.Data;
using MagiCloud.Equipments;
using Chemistry.Chemicals;

public class TestReactionSystem : EquipmentBase, IReaction
{
    private DrugSystem drugSystem;
    private ReactionControl reactionControl;
    public DrugSystem DrugSystemIns {
        get {
            return drugSystem;
        }
    }

    public ReactionControl ReactionControlIns {
        get {
            return reactionControl;
        }
    }

    protected override void Start()
    {
        base.Start();

        drugSystem = new DrugSystem();

        drugSystem.AddDrug("过氧化氢", 20);

        reactionControl = new ReactionControl(drugSystem);
        ReactionControlIns.AddReactionCondition("无");


        reactionControl.StartProduct();
        reactionControl.EventStart += EventStart;
        reactionControl.EventUpdate += EventUpdate;
        reactionControl.EventEnd += EventEnd;
    }

    private void EventEnd(ReactionInfo obj)
    {
        Debug.Log("结束反应");
    }

    private void EventUpdate(ReactionInfo obj)
    {
        Debug.Log("反应中……");
    }

    private void EventStart(ReactionInfo obj)
    {
        Debug.Log("开始反应");
    }
}
