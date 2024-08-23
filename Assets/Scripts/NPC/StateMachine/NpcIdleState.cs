using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// To switch states call npcSM.SwitchState(npcSM.STATENAME)

public class NpcIdleState : NpcBaseState
{
    public NpcIdleState(NpcStateMachine npcSM) : base(npcSM)
    {

    }


    public override void EnterState(NpcStateMachine npcSM)
    {
        UpdateStateUI();
        
        //StartTimer();
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
        npcSM.stateText.text = rawStateName.Replace("Npc", "").Replace("State", "");
    }


    public override void ExitState(NpcStateMachine npcSM)
    {

    }


    //private void SimpleNpcFov_OnDetected(object sender, SimpleNpcFov.OnDetectedEventArgs e)
    //{
    //    Debug.Log(e.target.name + "Event System Ran");
    //}


    //private async void StartTimer()
    //{
    //    await TimerSystem.StartTimer(15.0f, ExecuteAfterWait);
    //}


    //private void ExecuteAfterWait()
    //{
    //    npcSM.SwitchState(npcSM.PatrolState);
    //}
}
