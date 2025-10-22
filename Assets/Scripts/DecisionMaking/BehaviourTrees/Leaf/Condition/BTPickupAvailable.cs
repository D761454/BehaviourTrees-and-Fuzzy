using UnityEngine;

/// <summary>
/// sets target if pickups are present
/// </summary>
public class BTPickupAvailable : BTNode
{
    private bool m_switch = false;

    public override BTState Process() {
        Cannon cannon = (Cannon)m_Blackboard.GetFromDictionary(m_cannon);
        Health hp = (Health)m_Blackboard.GetFromDictionary(m_health);

        Transform health = (Transform)m_Blackboard.GetFromDictionary("HealthPickup");
        Transform ammo = (Transform)m_Blackboard.GetFromDictionary("AmmoPickup");
        bool available = (bool)m_Blackboard.GetFromDictionary(m_pickupAvailable);

        // use switch to prevent continually adding to dict when reevaluating tree after move to target
        if (available && !m_switch){
            // can assume hp and canon components present if pickup node attatched to tree
            float hpRatio = hp.HealthRatio;
            float ammoRatio = cannon.AmmoRatio;

            if (ammoRatio < 1 && hpRatio > ammoRatio)
            {
                m_Blackboard.AddToDictionary(m_targetPos, (Vector2)ammo.position);
                m_Blackboard.AddToDictionary(m_targetReached, false);
                m_switch = true;
                return BTState.SUCCESS;
            }
            else if (hpRatio < 1)
            {
                m_Blackboard.AddToDictionary(m_targetPos, (Vector2)health.position);
                m_Blackboard.AddToDictionary(m_targetReached, false);
                m_switch = true;
                return BTState.SUCCESS;
            }
            else{
                return BTState.FAILURE;
            }
        }

        // if switch is on (pickup was set) but no pickup available (got collected), reset switch
        if (m_switch && !available){
            m_switch = false;
        }

        return BTState.FAILURE;
    }

    
}
