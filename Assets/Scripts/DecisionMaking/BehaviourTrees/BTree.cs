using UnityEngine;

public class BTree
{
    BTNode m_Root;
    public Blackboard m_Blackboard;

    public BTree(BTNode root)
    {
        m_Blackboard = new Blackboard();
        m_Root = root;
        m_Root.AddBBRecursive(m_Blackboard);
    }

    public void Process()
    {
        m_Root.Process();
    }
}
