using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Has one child and is capable of augmenting the return state of it’s child. This uses the Decorator pattern.
public abstract class DecoratorNode : Node
{
    public Node child;
}
