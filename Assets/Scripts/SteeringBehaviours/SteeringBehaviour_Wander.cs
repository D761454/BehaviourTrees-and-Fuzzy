using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SteeringBehaviour_Wander : SteeringBehaviour
{
    [Header("Wander Properties")]
    [Header("Settings")]
    public float m_WanderRadius = 2; 
    public float m_WanderOffset = 2;
    public float m_AngleDisplacement = 15;

    Vector2 m_CirclePosition;
    Vector2 m_PointOnCircle;
    float m_Angle = 0.0f;

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;
    [SerializeField]
    protected Color m_Debug_EntityToCentreColour = Color.cyan;
    [SerializeField]
    protected Color m_Debug_CenteToPointColour = Color.green;
    [SerializeField]
    protected Color m_Debug_PointToEntityColour = Color.magenta;


    public override Vector2 CalculateForce()
    {
        m_Angle += Random.Range(-m_AngleDisplacement, m_AngleDisplacement);
        float angle = m_Angle * Mathf.Deg2Rad;

        Vector2 wanderDir = new Vector2(Mathf.Cos(angle) - Mathf.Sin(angle), Mathf.Sin(angle) + Mathf.Cos(angle));

        wanderDir = Maths.Normalise(wanderDir) * m_WanderRadius;

        if (m_CirclePosition == null)
        {
            m_CirclePosition = (Vector2)transform.position;
            m_CirclePosition += Vector2.up * m_WanderOffset;
        }
        else
        {
            m_CirclePosition = (Vector2)transform.position;

            Vector2 velocity = m_Manager.m_Entity.m_Velocity;

            if (velocity == Vector2.zero)
            {
                velocity = Vector2.up;
            }

            // agent facing dir is main problem
            m_CirclePosition += Maths.Normalise(velocity) * m_WanderOffset;
        }

        m_PointOnCircle = m_CirclePosition + wanderDir;

        // seek

        Vector2 m_DesiredVelocity = m_PointOnCircle - (Vector2)transform.position;
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
                Gizmos.color = m_Debug_RadiusColour;
				Gizmos.DrawWireSphere(m_CirclePosition, m_WanderRadius);

                Gizmos.color = m_Debug_EntityToCentreColour;
				Gizmos.DrawLine(transform.position, m_CirclePosition);

                Gizmos.color = m_Debug_CenteToPointColour;
				Gizmos.DrawLine(m_CirclePosition, m_PointOnCircle);

                Gizmos.color = m_Debug_PointToEntityColour;
				Gizmos.DrawLine(transform.position, m_PointOnCircle);

                base.OnDrawGizmosSelected();
			}
        }
	}
}
