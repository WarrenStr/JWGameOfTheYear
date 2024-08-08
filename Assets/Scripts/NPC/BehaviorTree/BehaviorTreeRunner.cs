using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    [SerializeField] private BehaviorTree _tree;

    // Start is called before the first frame update
    private void Start()
    {
        _tree = ScriptableObject.CreateInstance<BehaviorTree>();

        DebugLogNode log1 = ScriptableObject.CreateInstance<DebugLogNode>();
        log1.message = "Testing 1";
       
        DebugLogNode log2 =ScriptableObject.CreateInstance<DebugLogNode>();
        log2.message = "Testing 2";
        
        DebugLogNode log3 = ScriptableObject.CreateInstance<DebugLogNode>();
        log3.message = "Testing 3";

        CompositeNode sequence = ScriptableObject.CreateInstance<SequencerNode>();
        sequence.children.Add(log1);
        sequence.children.Add(log2);
        sequence.children.Add(log3);

        DecoratorNode loop = ScriptableObject.CreateInstance<RepeatNode>();
        loop.child = sequence;

        _tree.rootNode = loop;
    }

    // Update is called once per frame
    void Update()
    {
        _tree.Update();
    }
}
