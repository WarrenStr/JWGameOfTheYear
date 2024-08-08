using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    private int _current;

    #region Overrides of Node

    /// <inheritdoc />
    protected override void OnStart()
    {
        _current = 0;
    }

    /// <inheritdoc />
    protected override void OnStop() { }

    /// <inheritdoc />
    protected override State OnUpdate() 
    {
        if (children is { Count: >= 1 })
            return children[_current]!.Update() switch
            {
                State.Running => State.Running,
                State.Failure => State.Failure,
                State.Success => ++ _current == children.Count ? State.Success : State.Running, 
                _ => State.Failure
            };
        Debug.LogWarning("Sequencer Node has no children.");
        return State.Failure;
    }

    #endregion
}
