using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FlyAttack", story: "[Boss] attacks player with fly attack", category: "Action", id: "304c5448c8a1571cb7bfcd9dcb4d562a")]
public partial class FlyAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    protected override Status OnStart()
    {
        Debug.Log("FlyAttack node fired");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
       if (Boss.Value == null)
        {
            Debug.LogError("Boss is NULL in DashAttackAction");
            return Status.Failure;
        }

        Boss.Value.DashAttack();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

