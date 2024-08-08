using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogNode : ActionNode
{
    public string message;

    #region Overrides of Node

    /// <inheritdoc />
    protected override void OnStart() => Debug.Log($"OnStart: {message}");

    /// <inheritdoc />
    protected override void OnStop() => Debug.Log($"OnStop: {message}");

    /// <inheritdoc />
    protected override State OnUpdate()
    {
        Debug.Log($"OnUpdate: {message}");
        return State.Success;
    }

    #endregion
}
