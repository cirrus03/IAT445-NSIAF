using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateDistance", story: "[Boss] updates [distance]", category: "Action", id: "d45eeaa976898a2c0eb88b09e474240a")]
public partial class UpdateDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;
    [SerializeReference] public BlackboardVariable<float> Distance;

    protected override Status OnStart()
    {
        return Status.Running;
    }

   protected override Status OnUpdate()
{
    Distance.Value = Boss.Value.GetDistanceToPlayer();
    // Debug.Log("found distance");
    Debug.Log("Boss: " + Boss.Value);
    Debug.Log("Distance: " + Distance.Value);
    return Status.Success;
}

    protected override void OnEnd()
    {
    }
}

