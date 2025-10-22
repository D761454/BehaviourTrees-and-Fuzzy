using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SteeringBehaviour_CollisionAvoidance : SteeringBehaviour
{
    [System.Serializable]
    public struct Feeler
	{
        [Range(0, 360)]
        public float m_Angle;
        public float m_MaxLength;
        public Color m_Colour;
    }

    public Feeler[] m_Feelers;
    Vector2[] m_FeelerVectors;
    float[] m_FeelersLength;
    
    [SerializeField]
    LayerMask m_FeelerLayerMask;

    private void Start()
    {
        m_FeelersLength = new float[m_Feelers.Length];
        m_FeelerVectors = new Vector2[m_Feelers.Length];
    }

    public override Vector2 CalculateForce()
    {
        UpdateFeelers();

        m_Steering = Vector2.zero;
        m_DesiredVelocity = Vector2.zero;

        Vector2 tempVector = Vector2.zero;
        Vector2 point = Vector2.zero;

        for (uint i = 0; i < m_Feelers.Length; i++)
        {
            RaycastHit2D tempHit = Physics2D.Raycast(transform.position, m_FeelerVectors[i],
                m_FeelersLength[i], m_FeelerLayerMask.value);

            if (tempHit)
            {
                if (tempVector == Vector2.zero)
                {
                    tempVector = tempHit.distance * m_FeelerVectors[i];
                    point = tempHit.point;
                }
                else if (tempHit.distance < Maths.Magnitude(tempVector))
                {
                    tempVector = tempHit.distance * m_FeelerVectors[i];
                    point = tempHit.point;
                }
            }
        }

        if (tempVector != Vector2.zero)
        {
            m_DesiredVelocity = (Vector2)transform.position - point;
            float distance = Maths.Magnitude(m_DesiredVelocity);

            if (distance <= m_Feelers[0].m_MaxLength)
            {
                Vector2 m_DesiredVelocityNormalised = Maths.Normalise(m_DesiredVelocity);
                m_DesiredVelocity = m_DesiredVelocityNormalised * m_Manager.m_Entity.m_MaxSpeed;

                m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

                return m_Steering * m_Weight;
            }
        }

        return Vector2.zero;
    }

    void UpdateFeelers()
    {
        for (int i = 0; i < m_Feelers.Length; ++i)
        {
            m_FeelersLength[i] = Mathf.Lerp(1, m_Feelers[i].m_MaxLength, Maths.Magnitude(m_Manager.m_Entity.m_Velocity) / m_Manager.m_Entity.m_MaxSpeed);
            m_FeelerVectors[i] = Maths.RotateVector(Maths.Normalise(m_Manager.m_Entity.m_Velocity), m_Feelers[i].m_Angle) * m_FeelersLength[i];
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                for (int i = 0; i < m_Feelers.Length; ++i)
                {
                    Gizmos.color = m_Feelers[i].m_Colour;
                    Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_FeelerVectors[i]);
                }

                base.OnDrawGizmosSelected();
            }
        }
    }
}
