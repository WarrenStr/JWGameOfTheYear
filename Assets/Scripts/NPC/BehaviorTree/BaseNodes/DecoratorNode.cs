/// <summary>
/// This node has a single child and is used to modify the behavior of that child.
/// For example, a decorator might retry an action until it succeeds, or it might invert the result of a child node (turning a success into a failure and vice versa).
/// </summary>
public abstract class DecoratorNode : Node
{
    public Node child;
}
