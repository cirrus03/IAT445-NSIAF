using UnityEngine;

public class BossStateMachine : MonoBehaviour
{

    public enum BossState { Idle, Combat, Shielded, Stunned, Recovering, Dead }

    public BossState currentState {get; private set;}

    public 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // currentState = BossState.Idle;
        currentState = BossState.Combat;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(BossState newState)
    {
        
    }
}
