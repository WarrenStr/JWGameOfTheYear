using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Has a list of children and is the control flow of the behavior tree like switch statements and for loops. There are 2 types Composite Nodes the Selector and Sequence. This uses the Composite pattern.
public abstract class CompositeNode : Node
{
    public List<Node> children = new List<Node>();
}
