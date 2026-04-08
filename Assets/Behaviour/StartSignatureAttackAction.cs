using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartSignatureAttackAction", story: "[Boss] starts signature attack", category: "Action", id: "590e2ea0816fd2dc6f5863159a972339")]
public partial class StartSignatureAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {   
        Debug.Log("in teh start signature action");
        Debug.Log(Boss.Value.currentState);

        if(Boss.Value.currentState != BossState.Signature)
        {
            Debug.Log("you don't belong in signature graph");
            // return Status.Failure;
        }

        Boss.Value.StartSignatureAttack();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

