using UnityEngine;

/// <summary>
/// This node will randomly select one of its children and execute it.
/// </summary>
public class RandomSelectorNode : CompositeNode
{
    private int _selectedIndex = -1;

    protected override void OnStart()
    {
        _selectedIndex = Random.Range(0, children.Count);
        //Debug.Log("RandomSelectorNode started & selected node index: " + _selectedIndex);
    }

    protected override void OnStop()
    {
        //Debug.Log("RandomSelectorNode stopped");
    }

    protected override State OnUpdate()
    {
        // Update the selected child node
        if (children[_selectedIndex].Update() == State.Running)
        {
            return State.Running;
        }

        return State.Success;
    }
}
