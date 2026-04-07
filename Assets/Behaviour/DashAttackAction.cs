using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DashAttack", story: "[Boss] attacks Player with dash attack", category: "Action", id: "83555fe56fc37fa47021b49953cb4183")]
public partial class DashAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    protected override Status OnStart()
    {
        Debug.Log("DashAttack node fired");
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

