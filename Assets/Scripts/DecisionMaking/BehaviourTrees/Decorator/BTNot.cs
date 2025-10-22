using UnityEngine;

/// <summary>
/// returns the opposite of the child node
/// </summary>
/// <returns></returns>
public class BTNot : BTNode
{
    /// <summary>
/// returns the opposite of the child node
/// </summary>
/// <returns></returns>
    public BTNot(BTNode child) : base(child) {
        
    }

/// <summary>
/// returns the opposite of the child node
/// </summary>
/// <returns></returns>
    public override BTState Process() {
        BTState ret = m_Children[m_ActiveChild].Process();

        if (ret == BTState.SUCCESS) {
            return BTState.FAILURE;
        }
        else if (ret == BTState.FAILURE) {
            return BTState.SUCCESS;
        }

        return ret;
    }
}
