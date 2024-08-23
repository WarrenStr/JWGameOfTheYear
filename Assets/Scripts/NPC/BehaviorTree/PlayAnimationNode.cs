using UnityEngine;

/// <summary>
/// Play a specific animation. 
/// </summary>
public class PlayAnimationNode : ActionNode
{
    private Animator _animator;
    private string _animationName;

    public void Initialize(Animator anim, string animName)
    {
        _animator = anim;
        _animationName = animName;
    }

    protected override void OnStart()
    {
        _animator.Play(_animationName);
        //Debug.Log("Playing animation: " + _animationName);
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        Debug.Log("play animation is " + _animationName);
        // Determine if the animation has finished
        // _animator.GetCurrentAnimatorStateInfo(0).IsName(_animationName) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            //Debug.Log("play animation is " + _animationName);
            return State.Running; // Animation is stil playing.
            
        }

        return State.Success; // Animation has finished. 
    }
}
