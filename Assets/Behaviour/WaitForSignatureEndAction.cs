using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForSignatureEndAction", story: "[Boss] waits for minions to all die", category: "Action", id: "4188fc81584c347b9b825ebdaabb7eab")]
public partial class WaitForSignatureEndAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    protected override Status OnUpdate()
    {
        if (Boss.Value.IsSignatureActive())
        {
            return Status.Running; //keep waiting, dont finsh
        }

        return Status.Success; //done waiting
    }
}

