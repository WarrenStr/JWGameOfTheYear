using System.Collections.Generic;

/// <summary>
/// This node can have one or more child nodes and determines the order in which these children are executed.
/// Common types of composite nodes include Sequence & Composite nodes
/// </summary>
public abstract class CompositeNode : Node
{
    public List<Node> children = new List<Node>();
}
