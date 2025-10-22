using UnityEngine;
using System.Linq;

/// <summary>
/// generate random location on map
/// </summary>
public class BTGetRandomLocation : BTNode
{
    public override BTState Process() {
        Pathfinding_AStar aStar = (Pathfinding_AStar)m_Blackboard.GetFromDictionary(m_aStar);
        Transform selfPosT = (Transform)m_Blackboard.GetFromDictionary(m_selfPosition);
        Vector2 selfPos = (Vector2)selfPosT.transform.position;
        bool targetReached = (bool)m_Blackboard.GetFromDictionary(m_targetReached);

        // toggle to prevent regen random position on reevaluation of tree
        if (targetReached)
        {
            Debug.Log("gen random location");
            Rect size = TileGrid.m_GridSize;
            float x1 = Random.Range(size.xMin, size.xMax);
            float y1 = Random.Range(size.yMin, size.yMax);

            Vector2 targetPos = new Vector2(x1, y1);

            m_Blackboard.AddToDictionary(m_targetPos, targetPos);
            m_Blackboard.AddToDictionary(m_targetReached, false);
        }

        // allways success
        return BTState.SUCCESS;
    }
}
