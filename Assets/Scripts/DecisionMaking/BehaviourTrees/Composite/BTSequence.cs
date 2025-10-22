using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Process the sequence of children nodes until all succeed or 1 fails
/// </summary>
public class BTSequence : BTNode
{
    /// <summary>
/// Process the sequence of children nodes until all succeed or 1 fails
/// </summary>
/// <returns></returns>
    public BTSequence(List<BTNode> children) : base(children) {
        
    }

/// <summary>
/// Process the sequence of children nodes until all succeed or 1 fails
/// </summary>
/// <returns></returns>
    public override BTState Process() 
    {
        BTState ret = m_Children[m_ActiveChild].Process();

        if (ret == BTState.FAILURE) {
            m_ActiveChild = 0;
            return ret;
        }
        else if (ret == BTState.SUCCESS) {
            m_ActiveChild++;
            if (m_ActiveChild == m_Children.Count) {
                m_ActiveChild = 0;
                return ret;
            }
            return BTState.PROCESSING;
        }
        else if (ret == BTState.CONDITIONALABORT){
            m_ActiveChild = 0;
            return BTState.PROCESSING;
        }
        else if (ret == BTState.ABORT){
            m_ActiveChild = 0;
            return BTState.ABORT;
        }

        return ret;
    }
}
