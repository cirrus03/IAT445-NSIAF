using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FlyReady", story: "[Boss] checks if fly attack is ready", category: "Action", id: "fe052d2505aa97c1aa10d007177a6316")]
public partial class FlyReadyAction : Action
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
            Debug.LogError("Boss is NULL in FlyReadyAction");
            return Status.Failure;
        }

        Boss.Value.FlyReady();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

