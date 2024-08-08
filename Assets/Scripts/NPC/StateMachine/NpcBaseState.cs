using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class NpcBaseState
{
    public NpcStateMachine npcSM;

    public abstract void EnterState(NpcStateMachine npcSM);
    public abstract void UpdatePhysics(NpcStateMachine npcSM);
    public abstract void UpdateLogic(NpcStateMachine npcSM);
    public abstract void UpdateLate(NpcStateMachine npcSM);
    public abstract void ExitState(NpcStateMachine npcSM);
    public abstract void UpdateStateUI();
}
