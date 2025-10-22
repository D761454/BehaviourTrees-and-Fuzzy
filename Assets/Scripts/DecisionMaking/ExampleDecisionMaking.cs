using UnityEngine;

public class ExampleDecisionMaking : DecisionMaking
{

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Wander m_Wander;
    SteeringBehaviour_Arrive m_Arrive;

    Task13_DecisionMaking m_Target;

    Cannon m_Cannon;

    protected override void Awake()
    {
        base.Awake();
        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_Wander = GetComponent<SteeringBehaviour_Wander>();

        if (!m_Wander)
            Debug.LogError("Object doesn't have a Wander Steering Behaviour attached", this);


        m_Arrive = GetComponent<SteeringBehaviour_Arrive>();

        if (!m_Arrive)
            Debug.LogError("Object doesn't have a Arrive Steering Behaviour attached", this);

        m_Cannon = GetComponentInChildren<Cannon>();

        if (!m_Cannon)
            Debug.LogError("Object doesn't have a Cannon attached", this);

        m_Arrive.m_Active = false;
        m_Wander.m_Active = true;
    }

    public override void update()
    {
        if(m_Enemies.Count > 0)
        {
            m_Target = m_Enemies[0];
            m_Cannon.SetTarget(m_Target.transform);
        }
        else
        {
            m_Target = null;
            m_Cannon.SetTarget(null);
        }

        if (m_Target != null) 
        {
            m_Arrive.m_Active = true;
            m_Wander.m_Active = false;

            //seeks 3 units away from target
            Vector2 seekPoint = m_Target.transform.position - ((m_Target.transform.position - transform.position).normalized) * 3;
            m_Arrive.m_TargetPosition = seekPoint;
        }
        else
        {
            m_Arrive.m_Active = false;
            m_Wander.m_Active = true;
        }

        m_Cannon.AimAndFire();
    }
}
