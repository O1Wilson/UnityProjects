using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState activeState;
    public PatrolState patrolState;

    public void Initialize()
    {
        patrolState = new PatrolState();
        ChangeState(patrolState);
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (activeState != null)
        {
            activeState.Perform();
        }
    }

    public void ChangeState(BaseState newState)
    {
        if(activeState != null) //checks for an active state
        {
            activeState.Exit(); //run cleanup on activeState
        }
        
        activeState = newState;

        if (activeState != null) //fail-safe to make sure new state !null
        {
            //Setup new state.
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            activeState.Enter();
        }
    }
}
