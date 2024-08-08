using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Node : ScriptableObject 
{
    [SerializeField] private bool started;
    [SerializeField] private State state = State.Running;


    public enum State
    {
        Running,
        Success,
        Failure
    }


    /// <summary>
    /// Runs when the Node first starts running (Initialize the Node).
    /// </summary>
    protected abstract void OnStart();


    /// <summary>
    /// Runs when the Node Stops (Any Cleanup that the Node may need to do).
    /// </summary>
    protected abstract void OnStop();


    /// <summary>
    /// Runs every Update of the Node.
    /// </summary>
    /// <returns>The State the Node is in once it finishes Updating.</returns>
    protected abstract State OnUpdate();


    /// <summary>
    /// Start node and run OnStart() & OnUpdate() logic and stop node if it returns failure or success.
    /// </summary>
    /// <returns></returns>
    public State Update()
    {
        if (started == false)
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
}


