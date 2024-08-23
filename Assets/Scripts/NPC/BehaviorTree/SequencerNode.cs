/// <summary>
/// Executes child nodes in order until one fails.
/// </summary>
public class SequencerNode : CompositeNode
{
    int current;

    protected override void OnStart()
    {
        current = 0;
        //Debug.Log("SequencerNode started");
    }


    protected override void OnStop()
    {
        //Debug.Log("SequencerNode stopped");
    }


    protected override State OnUpdate()
    {
        var child = children[current];
        switch (child.Update())
        {
            case State.Running:
                return State.Running;

            case State.Failure:
                return State.Failure;

            case State.Success:
                current++;
                break;
        }

        return current == children.Count ? State.Success : State.Running;
    }
}
