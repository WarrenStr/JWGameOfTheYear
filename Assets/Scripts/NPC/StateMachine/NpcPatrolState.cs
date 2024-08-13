using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcPatrolState : NpcBaseState
{
    public NpcPatrolState(NpcStateMachine npcSM) : base(npcSM)
    {

    }

    public override void EnterState(NpcStateMachine npcSM)
    {

        UpdateStateUI();

        //npcSM.simpleNpcFov.OnDetected += SimpleNpcFov_OnDetected;
        
        StartTimer();
    }


    public override void UpdatePhysics(NpcStateMachine npcSM) // Update()
    {

    }


    public override void UpdateLogic(NpcStateMachine npcSM) // FixedUpdate()
    {

    }


    public override void UpdateLate(NpcStateMachine npcSM) // LateUpdate()
    {

    }


    public override void UpdateStateUI()
    {
        string rawStateName = this.GetType().Name;
       // npcSM.stateText.text = rawStateName.Replace("Npc", "").Replace("State", "");
    }


    public override void ExitState(NpcStateMachine npcSM)
    {
       // npcSM.simpleNpcFov.OnDetected -= SimpleNpcFov_OnDetected;
    }


    private void SimpleNpcFov_OnDetected(object sender, SimpleNpcFov.OnDetectedEventArgs e)
    {
        Debug.Log(e.target.name + "Event System Ran");
    }


    private async void StartTimer()
    {
        await TimerSystem.StartTimer(15.0f, ExecuteAfterWait);
    }


    private void ExecuteAfterWait()
    {
        npcSM.SwitchState(npcSM.IdleState);
    }
}
