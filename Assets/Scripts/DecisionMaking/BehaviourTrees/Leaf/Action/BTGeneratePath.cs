using UnityEngine;
using System.Linq;

/// <summary>
/// generate aa* path to target location
/// </summary>
public class BTGeneratePath : BTNode
{
    public override BTState Process() {
        Pathfinding_AStar aStar = (Pathfinding_AStar)m_Blackboard.GetFromDictionary(m_aStar);
        Transform selfPosT = (Transform)m_Blackboard.GetFromDictionary(m_selfPosition);
        Vector2 selfPos = (Vector2)selfPosT.transform.position;
        Vector2 targetPos = (Vector2)m_Blackboard.GetFromDictionary(m_targetPos);

        aStar.GeneratePath(TileGrid.GetNodeClosestWalkableToLocation(selfPos), TileGrid.GetNodeClosestWalkableToLocation(targetPos));
        m_Blackboard.AddToDictionary(m_targetPos, aStar.m_Path.Last<Vector2>());

        return BTState.SUCCESS;
    }

    
}