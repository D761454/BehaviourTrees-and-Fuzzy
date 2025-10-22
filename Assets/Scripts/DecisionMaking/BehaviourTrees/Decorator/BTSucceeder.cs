using UnityEngine;

    /// <summary>
/// Returns Success regardless of child state
/// </summary>
/// <returns></returns>
public class BTSucceeder : BTNode
{
    /// <summary>
/// Returns Success regardless of child state
/// </summary>
/// <returns></returns>
    public BTSucceeder(BTNode child) : base(child) {
        
    }
        /// <summary>
/// Returns Success regardless of child state
/// </summary>
/// <returns></returns>
    public override BTState Process() {
        BTState ret = m_Children[m_ActiveChild].Process();

        return BTState.SUCCESS;
    }
}
