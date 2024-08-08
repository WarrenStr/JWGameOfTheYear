using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviorTree", menuName = "Behavior Tree")]
public class BehaviorTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;

    private bool _hasRootNode = false;


    public Node.State Update()
    {
        if (_hasRootNode == false)
        {
            _hasRootNode = rootNode != null;

            if (rootNode == false) 
            {
                Debug.LogWarning($"{name} need to be a root node in order to properly run. Please add one.", this);
            }
        }

        if (_hasRootNode == true)
        {
            if (treeState == Node.State.Running)
            {
                treeState = rootNode.Update();
            }
            else
            {
                treeState = Node.State.Failure;
            }
        }
        return treeState;
    }
}
