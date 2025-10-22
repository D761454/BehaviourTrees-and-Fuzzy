using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Alignment : SteeringBehaviour
{
    public float m_AlignmentRange;
    
    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Vector2 accumulatedHeading = Vector2.zero;
        int validAgents = 0;

        MovingEntity[] agents = FindObjectsByType<MovingEntity>(FindObjectsSortMode.None);

        // keep agents facing same dir as neighbours

        foreach (var agent in agents)
        {
            if (agent.gameObject != gameObject && agent.gameObject.name != "Player")
            {
                Vector2 toAgent = (Vector2)agent.transform.position - (Vector2)transform.position;
                if (Maths.Magnitude(toAgent) < m_AlignmentRange)
                {
                    if (m_Manager.m_Entity.m_Velocity != Vector2.zero)
                    {
                        if (Maths.Dot(Maths.Normalise(m_Manager.m_Entity.m_Velocity), toAgent) > m_FOV)
                        {
                            validAgents++;
                            accumulatedHeading += agent.m_Velocity;
                        }
                    }
                    else
                    {
                        if (Maths.Dot(Vector2.up, toAgent) > m_FOV)
                        {
                            validAgents++;
                            accumulatedHeading += agent.m_Velocity;
                        }
                    }
                }
            }
        }

        Vector2 alignmentForce = accumulatedHeading / validAgents;

        if (validAgents > 0)
        {
            Vector2 m_DesiredVelocityNormalised = Maths.Normalise(alignmentForce);

            alignmentForce = m_DesiredVelocityNormalised * m_Manager.m_Entity.m_MaxSpeed;

            m_Steering = alignmentForce - m_Manager.m_Entity.m_Velocity;

            return m_Steering * m_Weight;
        }

        return Vector2.zero;
    }
}
