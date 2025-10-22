using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Seek : SteeringBehaviour
{
    [Header("Seek Properties")]
    [Header("Settings")]
    public Vector2 m_TargetPosition;

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_TargetColour = Color.yellow;

    public override Vector2 CalculateForce()
    {
        Vector2 m_DesiredVelocity = m_TargetPosition - (Vector2)transform.position;
        Vector2 m_DesiredVelocityNormalised = Maths.Normalise(m_DesiredVelocity);

        m_DesiredVelocity = m_DesiredVelocityNormalised * m_Manager.m_Entity.m_MaxSpeed;

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
            
                base.OnDrawGizmosSelected();
            }
        }
    }
}
