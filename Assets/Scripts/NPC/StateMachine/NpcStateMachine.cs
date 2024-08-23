// How to Program in Unity: State Machines Explained https://www.youtube.com/watch?v=Vt8aZDPzRjI

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcStateMachine : MonoBehaviour
{
    public TextMeshProUGUI stateText;

    private NpcBaseState _currentState;

    public NpcDefaultState DefaulState;
    public NpcIdleState IdleState;
    public NpcPatrolState PatrolState;

    private void Awake() 
    {
        DefaulState = new NpcDefaultState(this); // Create instances of states.
        IdleState = new NpcIdleState(this);
        PatrolState = new NpcPatrolState(this);
    }

private void Start()
    {
        _currentState = IdleState;
        _currentState.EnterState(this);
    }


    private void FixedUpdate() 
    {
        _currentState.UpdatePhysics(this); 
    }


    private void Update() 
    {
        _currentState.UpdateLogic(this); 
    }


    private void LateUpdate() 
    {
        _currentState.UpdateLate(this);
    }


    public void SwitchState(NpcBaseState newState)
    {
        _currentState.ExitState(this);
        _currentState = newState;
        newState.EnterState(this);
    }
}
