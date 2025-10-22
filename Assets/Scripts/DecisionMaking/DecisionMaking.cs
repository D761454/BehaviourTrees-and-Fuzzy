using System.Collections.Generic;
using UnityEngine;

public abstract class DecisionMaking : MonoBehaviour
{
    public List<Task13_DecisionMaking> m_Enemies { get; private set; }

    protected virtual void Awake()
    {
        m_Enemies = new List<Task13_DecisionMaking>();
    }    

    public abstract void update();

    /// <summary>
    /// Updated from the decision making entity to produce a list of enemies in range.
    /// </summary>
    public void UpdateEnemyList(List<Task13_DecisionMaking> enemies)
    {
        m_Enemies.Clear();
        m_Enemies.AddRange(enemies);
    }
}