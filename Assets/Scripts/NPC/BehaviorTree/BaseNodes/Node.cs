// Unity | Create Behaviour Trees using UI Builder, GraphView, and Scriptable Objects https://www.youtube.com/watch?v=nKpM98I7PeM&t=3023s

using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State // A state can return any of the 3 sub-states.
    {
        Running,
        Failure,
        Success
    }

    public State state = State.Running;
    public bool started = false;

    public State Update()
    {
        if (!started)
        {
            OnStart();
            started = true;
        }

        state = OnUpdate();

        if (state == State.Failure || state == State.Success)
        {
            OnStop();
            started = false;
        }

        return state;
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
}
