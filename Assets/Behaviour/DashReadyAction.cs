using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DashReady", story: "[Boss] checks if dash attack is ready", category: "Action", id: "2e0ba5b63d545555225a6d06d8c2f9ee")]
public partial class DashReadyAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        if (Boss.Value == null)
        {
            Debug.LogError("Boss is NULL in DashReadyAction");
            return Status.Failure;
        }

        Boss.Value.DashReady();
        return Status.Success;

    }

    protected override void OnEnd()
    {
    }
}

