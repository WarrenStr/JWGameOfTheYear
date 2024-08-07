using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcStateMachine : MonoBehaviour
{
    public TextMeshProUGUI stateText;
    public SimpleNpcFov simpleNpcFov;

    NpcBaseState currentState;
    public NpcIdleState IdleState = new NpcIdleState();
    public NpcPatrolState PatrolState = new NpcPatrolState();


    private void Start()
    {
        currentState = IdleState;

        currentState.EnterState(this);
    }


    private void FixedUpdate() 
    { 
        currentState.UpdatePhysics(this); 
    }


    private void Update() 
    { 
        currentState.UpdateLogic(this); 
    }


    private void LateUpdate() 
    { 
        currentState.UpdateLate(this);
    }


    public void SwitchState(NpcBaseState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        newState.EnterState(this);
    }
}
