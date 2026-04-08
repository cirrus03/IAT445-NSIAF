using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToPlayerAlways", story: "[Boss] moves to [palyer]", category: "Action", id: "fe5ff39f78e9f27fff78c2bb3fba52d8")]
public partial class MoveToPlayerAlwaysAction : Action
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;
    [SerializeReference] public BlackboardVariable<Transform> Palyer;

    float timer = 0f;
    float moveDuration = 1f;

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

        while (timer < moveDuration)
        {
             Palyer.Value.position = Vector2.MoveTowards(
            Boss.Value.transform.position,
           Palyer.Value.position,
            Boss.Value.moveSpeed * Time.deltaTime
        );
            timer += Time.deltaTime;
        }

        // Boss.Value.MoveTowardsPlayer();
       

        return Status.Running; //keep moving each frame

    }

    protected override void OnEnd()
    {
    }
}

