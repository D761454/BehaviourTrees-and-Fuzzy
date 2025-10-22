using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Cohesion : SteeringBehaviour
{
    public float m_CohesionRange;
    
    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_CohesionRange);

        // pull agents towards each other
        Vector2 accumulatedPosition = Vector2.zero;
        int validAgents = 0;

        foreach (Collider2D entity in entities)
        {
            if (entity.gameObject != gameObject)
            {
                if (m_Manager.m_Entity.m_Velocity != Vector2.zero)
                {
                    if (Maths.Dot(Maths.Normalise(m_Manager.m_Entity.m_Velocity), (Vector2)entity.transform.position - (Vector2)transform.position) > m_FOV)
                    {
                        validAgents++;
                        accumulatedPosition += (Vector2)entity.transform.position;
                    }
                }
                else
                {
                    if (Maths.Dot(Vector2.up, (Vector2)entity.transform.position - (Vector2)transform.position) > m_FOV)
                    {
                        validAgents++;
                        accumulatedPosition += (Vector2)entity.transform.position;
                    }
                }
            }
        }

        Vector2 cohesionForce = accumulatedPosition / validAgents;

        if (validAgents > 0)
        {
            cohesionForce -= m_Manager.m_Entity.m_Velocity;
            return cohesionForce;
            //Vector2 m_DesiredVelocity = cohesionForce - (Vector2)transform.position;
            //Vector2 m_DesiredVelocityNormalised = Maths.Normalise(m_DesiredVelocity);
            //m_DesiredVelocity = m_DesiredVelocityNormalised * m_Manager.m_Entity.m_MaxSpeed;

            //m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

            //return m_Steering * m_Weight;
        }

        return Vector2.zero;
    }
}
