using UnityEngine;
using System.Collections.Generic;

//자식중 하나라도 실패하거나 실행중이면 반환
public class BT_Sequence : BT_Node
{
    private List<BT_Node> children;
    public BT_Sequence(List<BT_Node> children)
    {
        this.children = children;
    }
    public override BT_NodeStatus Evaluate()
    {
        foreach(var node in children)
        {
            var status = node.Evaluate();
            //실패시 반환
            if (status == BT_NodeStatus.Failure)
                return BT_NodeStatus.Failure;
            //실행중 반환
            else if (status == BT_NodeStatus.Running)
                return BT_NodeStatus.Running;
        }
        //성공 반환
        return BT_NodeStatus.Success;
    }
}
