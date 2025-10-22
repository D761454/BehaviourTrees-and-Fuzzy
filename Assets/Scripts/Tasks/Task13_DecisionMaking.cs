using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Task13_DecisionMaking : MovingEntity
{
	SteeringBehaviour_Manager m_SteeringBehaviours;
    DecisionMaking m_DecisionMaking;
    
    float m_ScanRange = 5f;
    public int m_Team;

    protected Transform m_HealthPickupLocation;
    protected Transform m_AmmoPickupLocation;

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_DecisionMaking = GetComponent<DecisionMaking>();

        if(!m_DecisionMaking)
        {
            Debug.LogError("Object doesn't have a decison making system attached", this);
        }

        PickupManager.OnPickUpSpawned += PickupSpawned;

        Health health = GetComponent<Health>();

        if(!health)
            Debug.LogError("Object doesn't have a health system attached", this);

        health.OnHealthDepleted += OnDeath;

        

    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }

    // Update is called once per frame
    public void Update()
    {
        Scan();
        if (m_DecisionMaking)
            m_DecisionMaking.update();
    }

    protected void Scan()
    {
        List<Task13_DecisionMaking> enemies = new List<Task13_DecisionMaking>();

        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_ScanRange);

        if (entities.Length > 0)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Task13_DecisionMaking entity = entities[i].GetComponent<Task13_DecisionMaking>();
                if (entity != null && entity.m_Team != m_Team)
                {
                    enemies.Add(entity);
                }
            }
        }

        if (m_DecisionMaking)
            m_DecisionMaking.UpdateEnemyList(enemies);
    }

    void PickupSpawned(Transform health, Transform ammo)
    {
        m_HealthPickupLocation = health;
        m_AmmoPickupLocation = ammo;
    }

    void OnDeath()
    {
        if(m_Team != 0)
            PlayerUI.OnScoreUpdate.Invoke();
    }
}