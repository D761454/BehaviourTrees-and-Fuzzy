using UnityEngine;
using System.Linq;

/// <summary>
/// move along generated path to target
/// </summary>
public class BTMoveToTarget : BTNode
{
    float m_Time = 5.0f;
    float m_Timer = 0.0f;

    public override BTState Process() {
        SteeringBehaviour_Manager sm = (SteeringBehaviour_Manager)m_Blackboard.GetFromDictionary(m_steeringBehaviourManager);
        Vector2 targetPos = (Vector2)m_Blackboard.GetFromDictionary(m_targetPos);
        SteeringBehaviour_Seek seek = (SteeringBehaviour_Seek)m_Blackboard.GetFromDictionary(m_seekBehaviour);
        Transform selfPosT = (Transform)m_Blackboard.GetFromDictionary(m_selfPosition);
        Vector2 selfPos = (Vector2)selfPosT.transform.position;
        Pathfinding_AStar aStar = (Pathfinding_AStar)m_Blackboard.GetFromDictionary(m_aStar);

        m_Timer -= Time.deltaTime;

        // reached final pos / node
        if ((targetPos - selfPos).magnitude < 0.15f) {
            Debug.Log(sm.gameObject.name + " Reached target");
            m_Blackboard.AddToDictionary(m_targetReached, true);
            sm.m_SteeringBehaviours.Remove(seek);
            m_Timer = m_Time;
            return BTState.SUCCESS;
        }

        if (!sm.m_SteeringBehaviours.Contains(seek)){
                sm.m_SteeringBehaviours.Add(seek);
        }

        if (aStar.m_Path.Count > 0)
        {
            if (aStar.m_Path.Last<Vector2>() != targetPos)
            {
                aStar.GeneratePath(TileGrid.GetNodeClosestWalkableToLocation(selfPos), TileGrid.GetNodeClosestWalkableToLocation(targetPos));
                m_Blackboard.AddToDictionary(m_targetPos, aStar.m_Path.Last<Vector2>());
                Debug.Log(sm.gameObject.name + " Generating path, target changed " + targetPos);
                m_Timer = m_Time;
            }
            else if (m_Timer <= 0.0f)
            {
                // timer to prevent getting stuck
                aStar.GeneratePath(TileGrid.GetNodeClosestWalkableToLocation(selfPos), TileGrid.GetNodeClosestWalkableToLocation(aStar.m_Path.Last<Vector2>()));

                m_Timer = m_Time;
            }
            else
            {
                Vector2 closestPoint = aStar.GetClosestPointOnPath(selfPos);

                if ((closestPoint - selfPos).magnitude < 0.5f){
                    closestPoint = aStar.GetNextPointOnPath(selfPos);
                }
                    
                if (!sm.m_SteeringBehaviours.Contains(seek)){
                    sm.m_SteeringBehaviours.Add(seek);
                }

                seek.m_TargetPosition = closestPoint;
            }
        }

        // use abort to exit out and reevaluate entire tree so if conditions change, agent can respond accordingly
        return BTState.ABORT;
    }
}
