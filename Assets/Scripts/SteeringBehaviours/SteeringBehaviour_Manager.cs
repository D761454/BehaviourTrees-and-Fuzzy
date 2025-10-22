using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Manager : MonoBehaviour
{
    public MovingEntity m_Entity { get; private set; }
    public float m_MaxForce = 500;
    public float m_RemainingForce;
    public List<SteeringBehaviour> m_SteeringBehaviours;

	private void Awake()
	{
        m_Entity = GetComponent<MovingEntity>();

        if(!m_Entity)
            Debug.LogError("Steering Behaviours only working on type moving entity", this);
    }

	public Vector2 GenerateSteeringForce()
    {
        Vector2 combinedForce = Vector2.zero;
        m_RemainingForce = m_MaxForce;

        int count = m_SteeringBehaviours.Count;

        for(int i = 0; i < count; i++)
        {
            // if we have force to use left and our behaviour is active, check is active 1st to break out immediately if inactive
            if (m_SteeringBehaviours[i].m_Active && m_RemainingForce > 0)
            {
                Vector2 tempForce = m_SteeringBehaviours[i].CalculateForce();

                // if force is more than available
                if (Maths.Magnitude(tempForce) > m_RemainingForce)
                {
                    // cut off force to remaining limit
                    tempForce = Maths.Normalise(tempForce) * m_RemainingForce;
                }
                // remove force from remaining
                m_RemainingForce -= Maths.Magnitude(tempForce);
                combinedForce += tempForce;
            }
        }

        // limit speed to max speed
        return Maths.Normalise(combinedForce) * Mathf.Min(combinedForce.magnitude, m_Entity.m_MaxSpeed);
    }

    public void EnableExclusive(SteeringBehaviour behaviour)
	{
        if(m_SteeringBehaviours.Contains(behaviour))
		{
            foreach(SteeringBehaviour sb in m_SteeringBehaviours)
			{
                sb.m_Active = false;
			}

            behaviour.m_Active = true;
		}
        else
		{
            Debug.Log(behaviour + " doesn't not exist on object", this);
		}
	}
    public void DisableAllSteeringBehaviours()
    {
        foreach (SteeringBehaviour sb in m_SteeringBehaviours)
        {
            sb.m_Active = false;
        }
    }

    public void AddSteeringBehaviour(SteeringBehaviour behaviour) 
    {
        m_SteeringBehaviours.Add(behaviour);
    }

    public void RemoveSteeringBehaviour(SteeringBehaviour behaviour)
    {
        m_SteeringBehaviours.Remove(behaviour);
    }
}
