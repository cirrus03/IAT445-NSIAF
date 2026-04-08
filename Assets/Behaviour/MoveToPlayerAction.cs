

using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;


[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToPlayer", story: "[Boss] moves towards player if distance is more than [dashRange] or less than [flyRange]", category: "Action", id: "8e1ab8df25db52deefe53796ca5b4fe8")]
public partial class MoveToPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;
    [SerializeReference] public BlackboardVariable<float> dashRange;
    [SerializeReference] public BlackboardVariable<float> flyRange;
    protected override Status OnStart()
    {
       
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Boss.Value == null)
        {
            Debug.LogError("Boss is NULL");
            return Status.Failure;
        }

        float distance = Boss.Value.GetDistanceToPlayer();

        //if boss is in range for any attack, stop moving so attacks can run
        if (distance <= dashRange || distance >= flyRange)
        {
            return Status.Failure; //fail so Try In Order moves to Dash/Fly
        }

        //if not move towards player
        Boss.Value.MoveTowardsPlayer();

        return Status.Running; //keep moving each frame

    }

    protected override void OnEnd()
    {
        
    }
}


// [Serializable, GeneratePropertyBag]
// [NodeDescription(name: "MoveToPlayer", story: "[Boss] moves towards player", category: "Action", id: "8e1ab8df25db52deefe53796ca5b4fe8")]
// public partial class MoveToPlayerAction : Action
// {
//     [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

//     protected override Status OnStart()
//     {
//         return Status.Running;
//     }

//     protected override Status OnUpdate()
//     {
//         if (Boss.Value == null)
//         {
//             Debug.LogError("Boss is NULL");
//             return Status.Failure;
//         }

//         Boss.Value.MoveTowardsPlayer();

//         return Status.Running; // ⚠️ IMPORTANT
//     }

//     protected override void OnEnd()
//     {
//     }
// }

