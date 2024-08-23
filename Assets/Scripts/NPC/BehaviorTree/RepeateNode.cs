/// <summary>
/// Repeats an action 
/// </summary>
public class RepeateNode : DecoratorNode
{
    protected override void OnStart()
    {
        //Debug.Log("RepeatNode started");
    }

    protected override void OnStop()
    {
        //Debug.Log("RepeatNode stopped");
    }

    protected override State OnUpdate()
    {
        child.Update();
        return State.Running; // Return only one state causing a loop.

    }
}
