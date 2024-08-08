using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatNode : DecoratorNode
{
    #region Overrides of Node

    /// <inheritdoc />
    protected override void OnStart() { }

    /// <inheritdoc />
    protected override void OnStop() { }

    /// <inheritdoc />
    protected override State OnUpdate()
    {
        child.Update();
        return State.Running;
    }

    #endregion
}
