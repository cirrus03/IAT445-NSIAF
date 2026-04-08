using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "SignatureStillActiveCondition", story: "[Boss] is performing signature attack", category: "Conditions", id: "cc5959b59d996690795ef5d56f4ab702")]
public partial class SignatureStillActiveCondition : Condition
{
    [SerializeReference] public BlackboardVariable<BossStateMachine> Boss;

     public override bool IsTrue()
    {
        return Boss.Value.IsSignatureActive();
    }
}
