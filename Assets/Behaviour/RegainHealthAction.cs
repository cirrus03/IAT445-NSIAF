using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RegainHealth", story: "[Boss] regains health", category: "Action", id: "585b77e2b4e509652354ab297bde291d")]
public partial class RegainHealthAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {   
        Boss.Value.RegainHealth();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

