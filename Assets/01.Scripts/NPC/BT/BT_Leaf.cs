using System;
using UnityEngine;

public class BT_Leaf : BT_Node
{
    private Func<BT_NodeStatus> action;
    public BT_Leaf(Func<BT_NodeStatus> action)
    {
        this.action = action;
    }

    public override BT_NodeStatus Evaluate()
    {
        return action();
    }
}
