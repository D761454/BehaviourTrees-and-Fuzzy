using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Pursuit : SteeringBehaviour
{

    [Header("Pursuit Properties")]
    [Header("Settings")]
    public MovingEntity m_PursuingEntity;

    private Vector2 m_TargetPosition;

    public override Vector2 CalculateForce()
    {
        if (m_PursuingEntity){
            Vector2 toTarget = m_PursuingEntity.transform.position - transform.position;
            float distBetween = Maths.Magnitude(toTarget);

            float combinedSpeed = Maths.Magnitude(m_PursuingEntity.m_Velocity) + Maths.Magnitude(m_Manager.m_Entity.m_Velocity);

            if (combinedSpeed <= 0)
            {
                combinedSpeed = 0.01f;
            }

            float predTime = distBetween / combinedSpeed;

            m_TargetPosition = (Vector2)m_PursuingEntity.transform.position + (m_PursuingEntity.m_Velocity * predTime);

            Vector2 m_DesiredVelocity = m_TargetPosition - (Vector2)transform.position;
            Vector2 m_DesiredVelocityNormalised = Maths.Normalise(m_DesiredVelocity);

            m_DesiredVelocity = m_DesiredVelocityNormalised * m_Manager.m_Entity.m_MaxSpeed;

            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

            return m_Steering * m_Weight;
        }
        return Vector2.zero;
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(m_TargetPosition, 0.5f);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
