using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EndStunned", story: "[Boss] ends stunned phase", category: "Action", id: "9490933b4c63794d22091ff005b69bae")]
public partial class EndStunnedAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    protected override Status OnStart()
    {
        return Status.Running;
    }


    protected override Status OnUpdate()
    {
        Boss.Value.SetState(BossState.Attack);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

