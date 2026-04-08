using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "isZenMode2", story: "[Boss] checks it's health", category: "Conditions", id: "1e632e577cb76b61e5993c8d92b77cd1")]
public partial class IsZenMode2Condition : Condition
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    public override bool IsTrue()
    {   
        if(Boss.Value.lastHealthTriggered)
            return true;
        else
        {
            return false;
        }
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
