using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BTAttack : BTNode
{
    float m_Time = 3f;
    public override BTState Process() {
        SteeringBehaviour_Manager sm = (SteeringBehaviour_Manager)m_Blackboard.GetFromDictionary(m_steeringBehaviourManager);
        Task13_DecisionMaking enemy = (Task13_DecisionMaking)m_Blackboard.GetFromDictionary(m_enemyPos);
        Cannon cannon = (Cannon)m_Blackboard.GetFromDictionary(m_cannon);
        SteeringBehaviour_Flee flee = (SteeringBehaviour_Flee)m_Blackboard.GetFromDictionary(m_fleeBehaviour);

        Transform selfPosT = (Transform)m_Blackboard.GetFromDictionary(m_selfPosition);

        // if enemy dies after detection e.g. bullet going towards enemy after detection, ensures no null ref
        if (enemy == null){
            cannon.SetTarget(null);
            cannon.ResetTime();
            if (sm.m_SteeringBehaviours.Contains(flee)){
                sm.m_SteeringBehaviours.Remove(flee);
            }
            return BTState.SUCCESS;
        }

        // add flee if not already present
        if (!sm.m_SteeringBehaviours.Contains(flee)){
            sm.m_SteeringBehaviours.Add(flee);
            cannon.SetTarget(enemy.transform);
        }

        flee.m_Weight = Mathf.Lerp(30, 5, cannon.AmmoRatio);
        flee.m_FleeTarget = enemy.transform;

        // if we have no ammo, break out, also remove flee if ammo pickup so we can go direct to it
        if (cannon.AmmoRatio == 0f){
            cannon.SetTarget(null);
            cannon.ResetTime();
            Debug.Log("out of ammo");
            if ((bool)m_Blackboard.GetFromDictionary(m_pickupAvailable)){
                if (sm.m_SteeringBehaviours.Contains(flee)){
                    sm.m_SteeringBehaviours.Remove(flee);
                }
            }
            return BTState.FAILURE;
        }

        if (cannon.m_FireTimer < m_Time){
            cannon.ResetTime();
        }

        cannon.AimAndFire();

        return BTState.CONDITIONALABORT;
    }
}
