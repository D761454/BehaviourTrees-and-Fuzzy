using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SteeringBehaviour_Evade : SteeringBehaviour
{
    [Header("Evade Properties")]
    [Header("Settings")]
    public MovingEntity m_EvadingEntity;
    public float m_EvadeRadius;

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;

    public void Update()
    {
        switch (m_Active)
        {
            case false:
                if (m_EvadingEntity){
                    Vector2 toTarget = m_EvadingEntity.transform.position - transform.position;
                    float distBetween = Maths.Magnitude(toTarget);

                    if (distBetween <= m_EvadeRadius)
                    {
                        m_Active = true;
                    }
                }
                break;
        }
    }

    public override Vector2 CalculateForce()
    {
        if (m_EvadingEntity)
        {
            Vector2 toTarget = m_EvadingEntity.transform.position - transform.position;
            float distBetween = Maths.Magnitude(toTarget);

            if (distBetween <= m_EvadeRadius)
            {
                float combinedSpeed = Maths.Magnitude(m_EvadingEntity.m_Velocity) + Maths.Magnitude(m_Manager.m_Entity.m_Velocity);

                if (combinedSpeed <= 0)
                {
                    combinedSpeed = 0.01f;
                }

                float predTime = distBetween / combinedSpeed;

                Vector2 targetPosition = (Vector2)m_EvadingEntity.transform.position + (m_EvadingEntity.m_Velocity * predTime);

                Vector2 m_DesiredVelocity = (Vector2)transform.position - targetPosition;
                Vector2 m_DesiredVelocityNormalised = Maths.Normalise(m_DesiredVelocity);

                m_DesiredVelocity = m_DesiredVelocityNormalised * m_Manager.m_Entity.m_MaxSpeed;

                m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

                return m_Steering * Mathf.Lerp(m_Weight, 0, Mathf.Min(distBetween, m_EvadeRadius) / m_EvadeRadius);
            }
            else
            {
                m_Active = false;
            }
        }
        

        return Vector2.zero;
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = m_Debug_RadiusColour;
                Gizmos.DrawWireSphere(transform.position, m_EvadeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
