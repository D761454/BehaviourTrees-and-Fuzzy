using UnityEngine;
using System.Linq;

public class FuzzyState_HealthPickup : FuzzyState
{
    [SerializeField]
    AnimationCurve m_HealthCurve;

    Health m_Health;
    SteeringBehaviour_Manager m_SteeringBehaviours;     
    SteeringBehaviour_Seek m_Seek;

    [SerializeField]
    Pathfinding_AStar m_AStar;

    protected Transform m_HealthPickup;
    protected Transform m_AmmoPickup;
    bool m_PickUpAvailable = false;

    float m_Time = 5.0f;
    float m_Timer = 0.0f;

    private void Awake()
    {
        PickupManager.OnPickUpSpawned += PickupSpawned;
        Pickup.PickUpCollected += PickupCollected;

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_Health = GetComponent<Health>();

        if (!m_Health)
            Debug.LogError("Object doesn't have a Health Component attached", this);

        m_Seek = gameObject.AddComponent<SteeringBehaviour_Seek>();

        if (!m_Seek)
            Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

        m_AStar = new Pathfinding_AStar(m_AStar.m_AllowDiagonal, m_AStar.m_CanCutCorners, m_AStar.m_Debug_ChangeTileColours);

        m_active = true;
    }

    public override void Enter()
    {
        m_SteeringBehaviours.AddSteeringBehaviour(m_Seek);
        m_AStar.m_Path.Clear();
    }

    public override void Exit()
    {
        m_SteeringBehaviours.RemoveSteeringBehaviour(m_Seek);
    }

    public override void Run()
    {
        m_Timer -= Time.deltaTime;

        m_Seek.m_Weight = Mathf.Lerp(5, 30, m_degreeOfActivation);

        if (m_AStar.m_Path.Count == 0)
        {
            m_AStar.GeneratePath(TileGrid.GetNodeClosestWalkableToLocation(transform.position), TileGrid.GetNodeClosestWalkableToLocation(m_HealthPickup.position));

            m_Timer = m_Time;
        }
        else if (m_AStar.m_Path.Count > 0)
        {
            if (m_Timer <= 0.0f)
            {
                m_AStar.GeneratePath(TileGrid.GetNodeClosestWalkableToLocation(transform.position), TileGrid.GetNodeClosestWalkableToLocation(m_AStar.m_Path.Last<Vector2>()));

                m_Timer = m_Time;
            }
            else{
                Vector2 closestPoint = m_AStar.GetClosestPointOnPath(transform.position);

                if (Maths.Magnitude(closestPoint - (Vector2)transform.position) < 0.5f)
                    closestPoint = m_AStar.GetNextPointOnPath(transform.position);

            m_Seek.m_TargetPosition = closestPoint;
            }
        }
    }

    public override float CalculateActivation()
    {
        if (m_PickUpAvailable)
        {
            float needsHealth = 1.0f - m_HealthCurve.Evaluate(m_Health.HealthRatio);
            m_degreeOfActivation = needsHealth;
            return m_degreeOfActivation;
        }

        m_degreeOfActivation = 0f;
        return m_degreeOfActivation;
    }

    void PickupSpawned(Transform health, Transform ammo)
    {
        m_HealthPickup = health;
        m_AmmoPickup = ammo;

        m_PickUpAvailable = true;
    }

    void PickupCollected()
    {
        m_PickUpAvailable = false;
    }
}
