using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class BTNode
{
    public enum BTState{
        SUCCESS,
        FAILURE,
        PROCESSING,
        CONDITIONALABORT,
        ABORT
    }

    public string m_cannon = "Cannon";
    public string m_health = "Health";
    public string m_enemyPos = "Enemy";
    public string m_fleeBehaviour = "FleeBehaviour";
    public string m_seekBehaviour = "SeekBehaviour";
    public string m_steeringBehaviourManager = "SteeringBehaviourManager";
    public string m_decisionMaking = "DecisionMaking";
    public string m_selfPosition = "SelfPos";
    public string m_enemyTag = "EnemyTag";
    public string m_targetPos = "TargetPos";
    public string m_hpPickup = "HealthPickup";
    public string m_ammoPickup = "AmmoPickup";
    public string m_pickupAvailable = "PickupAvailable";
    public string m_aStar = "AStar";
    public string m_targetReached = "TargetReached";


    public BTState m_State;
    public List<BTNode> m_Children;
    public int m_ActiveChild = 0;
    public Blackboard m_Blackboard;

/// <summary>
/// default constructor
/// </summary>
    public BTNode(){
        m_State = BTState.PROCESSING;
        m_Children = new List<BTNode>();
        m_ActiveChild = 0;
    }

/// <summary>
/// single child constructor - decorators
/// </summary>
/// <param name="child"></param>
    public BTNode(BTNode child){
        m_State = BTState.PROCESSING;
        m_Children = new List<BTNode>{
            child
        };
        m_ActiveChild = 0;
    }

/// <summary>
/// multiple children constructor - composites
/// </summary>
/// <param name="children"></param>
    public BTNode(List<BTNode> children) {
        m_State = BTState.PROCESSING;
        m_Children = children;
        m_ActiveChild = 0;
    }

    abstract public BTState Process();

    public void AddBBRecursive(Blackboard BB){
        m_Blackboard = BB;
        foreach (BTNode child in m_Children){
            child.AddBBRecursive(BB);
        }
    }
}
