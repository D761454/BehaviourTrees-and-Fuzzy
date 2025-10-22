using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task9_GroupMovingAgent : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
	SteeringBehaviour_Seek m_Seek;
	SteeringBehaviour_Seperation m_Seperation;
	SteeringBehaviour_Cohesion m_Cohesion;
	SteeringBehaviour_Alignment m_Alignment;

	GameObject m_SeekingPosition;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Seek = GetComponent<SteeringBehaviour_Seek>();

		if (!m_Seek)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

		m_Seperation = GetComponent<SteeringBehaviour_Seperation>();

		if (!m_Seperation)
			Debug.LogError("Object doesn't have a Seperation Steering Behaviour attached", this);

		m_Cohesion = GetComponent<SteeringBehaviour_Cohesion>();

		if (!m_Cohesion)
			Debug.LogError("Object doesn't have a Cohesion Steering Behaviour attached", this);

		m_Alignment = GetComponent<SteeringBehaviour_Alignment>();

		if (!m_Alignment)
			Debug.LogError("Object doesn't have a Alignment Steering Behaviour attached", this);
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

    private void Start()
    {
		m_SeekingPosition = GameObject.Find("Seeking Position");
    }

    private void Update()
    {
		m_Seek.m_TargetPosition = m_SeekingPosition.transform.position;
    }
}
