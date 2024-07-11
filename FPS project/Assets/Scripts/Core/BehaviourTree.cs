using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BTNodeState
{
    Success,
    Failure,
    Running
}

public class BTNode
{
    protected List<BTNode> children = new List<BTNode>();
    public void AddChild(BTNode node)
    {
        children.Add(node);
    }

    public virtual BTNodeState Evaluate()
    {
        return BTNodeState.Failure;
    }
}

public class BTAction : BTNode
{
    public System.Func<BTNodeState> action;

    public BTAction(System.Func<BTNodeState> action)
    {
        this.action = action;
    }

    public override BTNodeState Evaluate()
    {
        return action();
    }
}

public class BTSelector : BTNode
{
    public override BTNodeState Evaluate()
    {
        foreach (BTNode node in children)
        {
            BTNodeState result = node.Evaluate();
            if (result == BTNodeState.Success)
                return BTNodeState.Success;
            else if (result == BTNodeState.Running)
                return BTNodeState.Running;
        }
        return BTNodeState.Failure;
    }
}

public class BTCondition : BTNode
{
    public System.Func<bool> condition;

    public BTCondition(System.Func<bool> condition)
    {
        this.condition = condition;
    }

    public override BTNodeState Evaluate()
    {
        return condition() ? BTNodeState.Success : BTNodeState.Failure;
    }
}

public class BTSequence : BTNode
{
    public override BTNodeState Evaluate()
    {
        bool isAnyChildrenRunning = false;

        foreach (BTNode node in children)
        {
            BTNodeState result = node.Evaluate();
            if (result == BTNodeState.Failure)
            {
                return BTNodeState.Failure;
            }
            else if (result == BTNodeState.Running)
            {
                isAnyChildrenRunning = true;
            }
        }
        return isAnyChildrenRunning ? BTNodeState.Running : BTNodeState.Success;

    }
}

public class BehaviourTree : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
