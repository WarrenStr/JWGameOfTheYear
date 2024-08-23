using UnityEngine;

/// <summary>
/// Logs a custom debug message to Unity's console
/// </summary>
public class DebugLogNode : ActionNode
{
    public string message;

    protected override void OnStart()
    {
        //Debug.Log("DebugLogNode started");
    }

    protected override void OnStop()
    {
        //Debug.Log("DebugLogNode stopped");
    }

    protected override State OnUpdate()
    {
        Debug.Log($"OnUpdate: {message}");
        return State.Success;
    }
}
