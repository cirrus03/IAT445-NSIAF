using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnterStun", story: "[Boss] becomes stunned", category: "Action", id: "e6b0a00352db665e0f5b464e9deee7f7")]
public partial class EnterStunAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {   
        Boss.Value.startStun();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

