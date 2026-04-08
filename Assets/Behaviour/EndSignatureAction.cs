using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EndSignatureAction", story: "[Boss] ends signature attack", category: "Action", id: "969d5033807bb61336e875c3a2f769b0")]
public partial class EndSignatureAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Boss.Value.EndSignature();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

