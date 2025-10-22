using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SteeringBehaviour_Arrive : SteeringBehaviour
{
    [Header("Arrive Properties")]
    [Header("Settings")]
    public Vector2 m_TargetPosition;
    public float m_SlowingRadius; 

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;
    [SerializeField]
    protected Color m_Debug_TargetColour = Color.cyan;


    public override Vector2 CalculateForce()
    {
        Vector2 m_DesiredVelocity = m_TargetPosition - (Vector2)transform.position;
        float distance = Maths.Magnitude(m_DesiredVelocity);

        Vector2 m_DesiredVelocityNormalised = Maths.Normalise(m_DesiredVelocity);  

        if (distance >= m_SlowingRadius)
        {
            m_DesiredVelocity = m_DesiredVelocityNormalised * m_Manager.m_Entity.m_MaxSpeed;
        }
        else
        {
            m_DesiredVelocity = m_DesiredVelocityNormalised * m_Manager.m_Entity.m_MaxSpeed * (distance / m_SlowingRadius);
        }

        m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

        return m_Steering * m_Weight;
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = m_Debug_TargetColour;
                Gizmos.DrawSphere(m_TargetPosition, 0.5f);

                Gizmos.color = m_Debug_RadiusColour;
                Gizmos.DrawWireSphere(transform.position, m_SlowingRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
