using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action OnHealthDepleted;
    public Action<float, float> OnHealthChanged;

    [SerializeField]
    float m_MaxHealth;
    float m_CurrentHealth;

    Animator m_Animator;

    public float HealthRatio { get { return m_CurrentHealth / m_MaxHealth; } }

    public void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_CurrentHealth = m_MaxHealth;
    }

    public void ApplyDamage(int damage)
    {
        m_CurrentHealth -= damage;
        OnHealthChanged?.Invoke(m_MaxHealth, m_CurrentHealth);
        if(m_CurrentHealth <=0)
        {
            OnHealthDepleted.Invoke();
        }
    }
    public void ApplyHealing(float healing)
    {
        m_CurrentHealth += healing;
        if (m_CurrentHealth > m_MaxHealth)
        {
            m_CurrentHealth = m_MaxHealth;
        }
        OnHealthChanged?.Invoke(m_MaxHealth, m_CurrentHealth);
    }
}
