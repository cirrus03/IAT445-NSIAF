using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SpawnBabies", story: "[Boss] spawns babies", category: "Action", id: "c73e0e9e17799173841b12b20ae4af00")]
public partial class SpawnBabiesAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {   
        Debug.Log("spawning minions");
        Boss.Value.SpawnMinions();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

