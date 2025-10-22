using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Flee : SteeringBehaviour
{
    [Header("Flee Properties")]
    [Header("Settings")]
    public Transform m_FleeTarget;
    public float m_FleeRadius;

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;

    public override Vector2 CalculateForce()
    {
        if (m_FleeTarget)
        {
            Vector2 m_DesiredVelocity = (Vector2)transform.position - (Vector2)m_FleeTarget.position;
            float distance = Maths.Magnitude(m_DesiredVelocity);

            if (distance <= m_FleeRadius)
            {
                Vector2 m_DesiredVelocityNormalised = Maths.Normalise(m_DesiredVelocity);
                m_DesiredVelocity = m_DesiredVelocityNormalised * m_Manager.m_Entity.m_MaxSpeed;

                m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

                return m_Steering * Mathf.Lerp(m_Weight, 0, Mathf.Min(distance, m_FleeRadius) / m_FleeRadius);
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
                Gizmos.DrawWireSphere(transform.position, m_FleeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
