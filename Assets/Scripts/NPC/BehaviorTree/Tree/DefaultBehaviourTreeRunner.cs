using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    BehaviourTree tree;
    public Animator animator; // Assign this in the Inspector.

    private void Start()
    {
        // Create the behavior tree.
        tree = ScriptableObject.CreateInstance<BehaviourTree>();

        // Create the animation nodes.
        var idleRelaxedNode = ScriptableObject.CreateInstance<PlayAnimationNode>();
        idleRelaxedNode.Initialize(animator, "Idle Relaxed");

        var idleBreathingNode = ScriptableObject.CreateInstance<PlayAnimationNode>();
        idleBreathingNode.Initialize(animator, "Idle Breathing");

        var idleArmStretchNode = ScriptableObject.CreateInstance<PlayAnimationNode>();
        idleArmStretchNode.Initialize(animator, "Idle Arm Stretching");

        var idleNeckStretchNode = ScriptableObject.CreateInstance<PlayAnimationNode>();
        idleNeckStretchNode.Initialize(animator, "Idle Neck Stretch");

        //// Create a random animation selector 
        //var randomSelectorNode = ScriptableObject.CreateInstance<RandomSelectorNode>();
        //randomSelectorNode.children.Add(idleBreathingNode);
        //randomSelectorNode.children.Add(idleArmStretchNode);
        //randomSelectorNode.children.Add(idleNeckStretchNode);

        //// Set sequnce to be played.
        //var playSequence = ScriptableObject.CreateInstance<SequencerNode>();
        //playSequence.children.Add(idleRelaxedNode);
        //playSequence.children.Add(randomSelectorNode);

        ////
        //var repeatLoopNode = ScriptableObject.CreateInstance<RepeateNode>();
        //repeatLoopNode.child = playSequence;

        var wait = ScriptableObject.CreateInstance<WaitNode>();
        wait.duration = 0.5f;

        // Create a sequence node to combine walk and run
        var sequence = ScriptableObject.CreateInstance<SequencerNode>(); // TO-DO Find out why all 3 animations are not playing all the way through.
        sequence.children.Add(idleBreathingNode);
        sequence.children.Add(idleArmStretchNode);
        sequence.children.Add(idleNeckStretchNode);



        // Set the selector as the root node of the behavior tree
        tree.rootNode = sequence;
    }

    private void Update()
    {
        tree.Update();
    }
}

//BehaviourTree tree;

//private void Start()
//{
//    tree = ScriptableObject.CreateInstance<BehaviourTree>();

//    var log1 = ScriptableObject.CreateInstance<DebugLogNode>();
//    log1.message = "This is a sample debug message 1";

//    var pause1 = ScriptableObject.CreateInstance<WaitNode>();
//    pause1.duration = 5.0f;


//    var log2 = ScriptableObject.CreateInstance<DebugLogNode>();
//    log2.message = "This is a sample debug message 2";


//    var log3 = ScriptableObject.CreateInstance<DebugLogNode>();
//    log3.message = "This is a sample debug message 3";


//    var sequence = ScriptableObject.CreateInstance<SequencerNode>();
//    sequence.children.Add(log1);
//    sequence.children.Add(pause1);
//    sequence.children.Add(log2);
//    sequence.children.Add(log3);

//    var loop = ScriptableObject.CreateInstance<RepeateNode>();
//    loop.child = sequence;

//    tree.rootNode = loop;
//}

//private void Update()
//{
//    tree.Update();
//}