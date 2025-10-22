using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Process the sequence of children until it succeeds
/// </summary>
/// <returns></returns>
public class BTRepeat : BTNode
{
    /// <summary>
/// Process the sequence of children until it succeeds
/// </summary>
/// <returns></returns>
    public BTRepeat(BTNode child) : base(child) {
        
    }

/// <summary>
/// Process the sequence of children until it succeeds
/// </summary>
/// <returns></returns>
    public override BTState Process() 
    {
        BTState ret = m_Children[m_ActiveChild].Process();

        while (ret != BTState.SUCCESS){
            ret = m_Children[m_ActiveChild].Process();
        }

        return BTState.SUCCESS;
    }
}
