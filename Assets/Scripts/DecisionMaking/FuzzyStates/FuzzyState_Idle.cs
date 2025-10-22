using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FuzzyState_Idle : FuzzyState
{
    SteeringBehaviour_Manager m_SteeringBehaviours;     
    SteeringBehaviour_Seek m_Seek;
    DecisionMaking m_DecisionMaking;

    [SerializeField]
    Pathfinding_AStar m_AStar;

    FuzzyStates_Manager m_Manager;
    FuzzyState_AmmoPickup m_AmmoPickupState;
    FuzzyState_HealthPickup m_HealthPickupState;
    FuzzyState_Attack m_AttackState;

    float m_Time = 5.0f;
    float m_Timer = 0.0f;

    [SerializeField]
    bool m_Debug_DrawPath;

    private void Awake()
    {

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();

        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_Seek = gameObject.AddComponent<SteeringBehaviour_Seek>();

        if (!m_Seek)
            Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

        m_DecisionMaking = gameObject.GetComponent<DecisionMaking>();

        m_Manager = gameObject.GetComponent<FuzzyStates_Manager>();

        m_AmmoPickupState = gameObject.GetComponent<FuzzyState_AmmoPickup>();
        m_HealthPickupState = gameObject.GetComponent<FuzzyState_HealthPickup>();
        m_AttackState = gameObject.GetComponent<FuzzyState_Attack>();

        m_AStar = new Pathfinding_AStar(m_AStar.m_AllowDiagonal, m_AStar.m_CanCutCorners, m_AStar.m_Debug_ChangeTileColours);

        m_active = true;
    }

    public override void Enter()
    {
        m_SteeringBehaviours.AddSteeringBehaviour(m_Seek);
        m_Seek.m_Weight = 15.0f;

        if (m_AStar.m_Path.Count > 0)
        {
            m_AStar.GeneratePath(TileGrid.GetNodeClosestWalkableToLocation(transform.position), TileGrid.GetNodeClosestWalkableToLocation(m_AStar.m_Path.Last<Vector2>()));
        }
    }

    public override void Exit()
    {
        m_SteeringBehaviours.RemoveSteeringBehaviour(m_Seek);
    }

    public override void Run()
    {
        m_Timer -= Time.deltaTime;

        if (m_AStar.m_Path.Count == 0)
        {
            Rect size = TileGrid.m_GridSize;
            float x1 = Random.Range(size.xMin, size.xMax);
            float y1 = Random.Range(size.yMin, size.yMax);

            m_AStar.GeneratePath(TileGrid.GetNodeClosestWalkableToLocation(transform.position), TileGrid.GetNodeClosestWalkableToLocation(new Vector2(x1, y1)));
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
        // check if state present 1st to prevent errors, if state present and is active (were low on hp or ammo and there's a pickup available)
        // turn off idle, also uses decision making to check for enemies rather than presence of attack state as an agent may not have attack state
        if ((m_HealthPickupState && m_Manager.GetActiveStates().Contains(m_HealthPickupState)) 
            || (m_AmmoPickupState && m_Manager.GetActiveStates().Contains(m_AmmoPickupState))
            || (m_AttackState && m_Manager.GetActiveStates().Contains(m_AttackState)))
        {
            m_degreeOfActivation = 0f;
            return m_degreeOfActivation;
        }

        m_degreeOfActivation = 1.0f;
        return m_degreeOfActivation;
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_DrawPath)
            {
                Gizmos.DrawLine(transform.position, m_Seek.m_TargetPosition);

                if (m_AStar.m_Path.Count > 1)
                {
                    for (int i = 0; i < m_AStar.m_Path.Count - 1; ++i)
                    {
                        Gizmos.DrawLine(m_AStar.m_Path[i], m_AStar.m_Path[i + 1]);
                    }
                }
            }
        }
    }
}
