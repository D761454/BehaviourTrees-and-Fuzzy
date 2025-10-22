using UnityEngine;

/// <summary>
/// detect enemies via a raycast
/// </summary>
public class BTEnemyDetection : BTNode
{
    LayerMask mask = LayerMask.GetMask("GroundAI", "Environment");
    public override BTState Process()
    {
        Transform selfPosT = (Transform)m_Blackboard.GetFromDictionary(m_selfPosition);
        Vector2 selfPos = (Vector2)selfPosT.transform.position;
        DecisionMaking dm = (DecisionMaking)m_Blackboard.GetFromDictionary(m_decisionMaking);
        string enemyTag = (string)m_Blackboard.GetFromDictionary(m_enemyTag);

        SteeringBehaviour_Manager sm = (SteeringBehaviour_Manager)m_Blackboard.GetFromDictionary(m_steeringBehaviourManager);
        Cannon cannon = (Cannon)m_Blackboard.GetFromDictionary(m_cannon);
        SteeringBehaviour_Flee flee = (SteeringBehaviour_Flee)m_Blackboard.GetFromDictionary(m_fleeBehaviour);

        if (dm.m_Enemies.Count > 0)
        {
            foreach (var enemy in dm.m_Enemies)
            {
                if (Physics2D.Raycast(selfPos, (Vector2)enemy.transform.position - selfPos, 5f, mask).collider.gameObject.tag == enemyTag)
                {
                    if (enemy != (Task13_DecisionMaking)m_Blackboard.GetFromDictionary(m_enemyPos))
                    {
                        m_Blackboard.AddToDictionary(m_enemyPos, enemy);
                    }
                    return BTState.SUCCESS;
                }
            }

            // no visible enemies
            cannon.SetTarget(null);
            cannon.ResetTime();
            if (sm.m_SteeringBehaviours.Contains(flee)){
                sm.m_SteeringBehaviours.Remove(flee);
            }

            return BTState.FAILURE;
        }

        cannon.SetTarget(null);

        if (sm.m_SteeringBehaviours.Contains(flee)){
            sm.m_SteeringBehaviours.Remove(flee);
            cannon.ResetTime();
        }
        
        
        return BTState.FAILURE;
    }
}
