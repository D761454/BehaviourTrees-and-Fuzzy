using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SteeringBehaviour_Seperation : SteeringBehaviour
{
    public float m_SeperationRange;
    Vector2 accumulatedSeperationForce = Vector2.zero;
    
    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_SeperationRange);

        accumulatedSeperationForce = Vector2.zero;

        // keep dist away from avg location of all neighbouring agents

        foreach (Collider2D entity in entities)
        {
            if (entity.gameObject != gameObject)
            {
                Vector2 toEntity = (Vector2)entity.transform.position - (Vector2)transform.position;

                if (m_Manager.m_Entity.m_Velocity != Vector2.zero)
                {
                    if (Maths.Dot(Maths.Normalise(m_Manager.m_Entity.m_Velocity), toEntity) > m_FOV)
                    {
                        accumulatedSeperationForce -= toEntity;
                    }
                }
                else
                {
                    if (Maths.Dot(Vector2.up, toEntity) > m_FOV)
                    {
                        accumulatedSeperationForce -= toEntity;
                    }
                }
            }
        }

        if (entities.Length > 1)
        {
            return accumulatedSeperationForce;
            //Vector2 m_DesiredVelocity = accumulatedSeperationForce;

            //Vector2 m_DesiredVelocityNormalised = Maths.Normalise(m_DesiredVelocity);
            //m_DesiredVelocity = m_DesiredVelocityNormalised * m_Manager.m_Entity.m_MaxSpeed;

            //m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

            //return m_Steering * m_Weight;
        }

        return Vector2.zero;
    }
}
