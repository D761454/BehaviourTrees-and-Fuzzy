using UnityEngine;

public abstract class FuzzyState : MonoBehaviour
{
    public bool m_active;
    protected float m_degreeOfActivation;

    abstract public void Enter();

    abstract public void Exit();

    abstract public void Run();

    abstract public float CalculateActivation();
}
