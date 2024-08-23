/// <summary>
/// Execute its children in order until one of them succeeds.
/// </summary>
public class SelectorNode : CompositeNode
{
    private int _currentIndex = 0;

    protected override void OnStart()
    {
        _currentIndex = 0;  // Start with the first child
        //Debug.Log("SelectorNode started");
    }


    protected override void OnStop()
    {
        //Debug.Log("SelectorNode stopped");
    }


    protected override State OnUpdate()
    {
        while (_currentIndex < children.Count)
        {
            var childState = children[_currentIndex].Update();

            if (childState == State.Running)
            {
                return State.Running;
            }

            if (childState == State.Success)
            {
                return State.Success;  // Return success if any child succeeds
            }

            _currentIndex++;  // Move to the next child if the current one fails
        }

        return State.Failure;  // Return failure if all children fail
    }
}
