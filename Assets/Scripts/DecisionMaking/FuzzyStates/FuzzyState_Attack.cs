using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FuzzyState_Attack : FuzzyState
{
    [SerializeField] AnimationCurve m_CloseEnemiesCurve;
    [SerializeField] AnimationCurve m_healthCurve;
    [SerializeField] AnimationCurve m_ammoCurve;
    [SerializeField] string m_enemyTag;

    SteeringBehaviour_Manager m_SteeringBehaviours;     
    SteeringBehaviour_Evade m_Evade;
    SteeringBehaviour_Pursuit m_Pursuit;

    DecisionMaking m_DecisionMaking;

    Task13_DecisionMaking m_Target;

    Health m_health;
    Cannon m_Cannon;

    LayerMask mask;

    private void Awake()
    {
        mask = LayerMask.GetMask("GroundAI", "Environment");
        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_Evade = gameObject.AddComponent<SteeringBehaviour_Evade>();

        if (!m_Evade)
            Debug.LogError("Object doesn't have an Evade Steering Behaviour attached", this);

        m_Pursuit = gameObject.AddComponent<SteeringBehaviour_Pursuit>();

        if (!m_Pursuit)
            Debug.LogError("Object doesn't have an Pursuit Steering Behaviour attached", this);

        m_DecisionMaking = gameObject.GetComponent<DecisionMaking>();

        m_health = gameObject.GetComponent<Health>();

        if (!m_health)
            Debug.LogError("Object doesn't have a Health component attached", this);

        m_Cannon = GetComponentInChildren<Cannon>();

        if (!m_Cannon)
            Debug.LogError("Object doesn't have a Cannon component attached", this);

        m_active = true;
    }

    public override void Enter()
    {
        m_SteeringBehaviours.AddSteeringBehaviour(m_Evade);
        m_SteeringBehaviours.AddSteeringBehaviour(m_Pursuit);
        m_Evade.m_EvadeRadius = 4f;
    }

    public override void Exit()
    {
        m_SteeringBehaviours.RemoveSteeringBehaviour(m_Evade);
        m_SteeringBehaviours.RemoveSteeringBehaviour(m_Pursuit);
    }

    public override void Run()
    {
        if (m_Target != null)
        {
            float doa2 = Mathf.Clamp((m_healthCurve.Evaluate(m_health.HealthRatio) + m_ammoCurve.Evaluate(m_Cannon.AmmoRatio)) / 2f, 0.0f, 1.0f);

            m_Evade.m_EvadingEntity = m_Target;
            m_Evade.m_Weight = Mathf.Lerp(0, 30, Mathf.Clamp(m_degreeOfActivation - (doa2/2.0f), 0.0f, 1.0f));
            m_Pursuit.m_PursuingEntity = m_Target;
            m_Pursuit.m_Weight = Mathf.Lerp(0, 30, doa2);
            m_Cannon.AimAndFire();
        }
    }

    public override float CalculateActivation()
    {
        if (m_DecisionMaking.m_Enemies.Count > 0)
        {
            int enemies = 0;
            foreach (var enemy in m_DecisionMaking.m_Enemies)
            {
                if (Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, 5f, mask).collider.gameObject.tag == m_enemyTag)
                {
                    enemies ++;

                    // set closest visible enemy as target
                    if (enemies == 1)
                    {
                        m_Target = enemy;
                        m_Cannon.SetTarget(m_Target.transform);
                    }
                }
            }

            float closeEnemies = Mathf.Clamp(m_CloseEnemiesCurve.Evaluate(enemies / 10.0f), 0.0f, 1.0f);

            m_degreeOfActivation = closeEnemies;
        }
        else
        {
            m_degreeOfActivation = 0f;
        }

        if (m_degreeOfActivation == 0f)
        {
            m_Target = null;
            m_Cannon.SetTarget(null);
        }

        return m_degreeOfActivation;
    }
}
