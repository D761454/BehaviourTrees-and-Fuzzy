using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyDecisionMaking : DecisionMaking
{
    SteeringBehaviour_Manager m_SteeringBehaviours;
    FuzzyStates_Manager m_FuzzyStates;

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_FuzzyStates = GetComponent<FuzzyStates_Manager>();

        if (!m_FuzzyStates)
            Debug.LogError("Object doesn't have a Fuzzy State Manager attached", this);
    }

    void Start()
    {
        // use to prevent error on enemy initialisation with collision avoidance - only causes 1 error at the start of the program
        m_SteeringBehaviours.AddSteeringBehaviour(GetComponent<SteeringBehaviour_CollisionAvoidance>());
    }

    public override void update()
    {
        m_FuzzyStates.CalculateActiveStates();
        m_FuzzyStates.RunActiveStates();
    }
}
