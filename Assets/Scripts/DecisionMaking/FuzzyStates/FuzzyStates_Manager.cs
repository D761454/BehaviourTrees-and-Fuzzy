using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyStates_Manager : MonoBehaviour
{
    [SerializeField]
    List<FuzzyState> m_FuzzyStates;
    List<FuzzyState> m_ActiveFuzzyStates;

    public List<FuzzyState> GetActiveStates()
    {
        return m_ActiveFuzzyStates;
    }

    private void Awake()
    {
        m_ActiveFuzzyStates = new List<FuzzyState>();
    }

    public void CalculateActiveStates()
    {
        // save last frames active states
        List<FuzzyState> lastActiveStates = new List<FuzzyState>();
        lastActiveStates.AddRange(m_ActiveFuzzyStates);

        // clear active states list
        m_ActiveFuzzyStates.Clear();

        // calculate to see if any states are active
        for (int i = 0; i < m_FuzzyStates.Count; i++)
        {
            if (m_FuzzyStates[i].m_active)
            {
                if (m_FuzzyStates[i].CalculateActivation() > 0.0f)
                {
                    m_ActiveFuzzyStates.Add(m_FuzzyStates[i]);
                }
            }
        }

        // if state is new, enter
        // if state is no longer active, exit

        foreach (FuzzyState state in m_ActiveFuzzyStates)
        {
            if (!lastActiveStates.Contains(state))
            {
                state.Enter();
            }
        }

        foreach (FuzzyState state in lastActiveStates)
        {
            if (!m_ActiveFuzzyStates.Contains(state))
            {
                state.Exit();
            }
        }
    }

    public void RunActiveStates()
    {
        foreach (FuzzyState state in m_ActiveFuzzyStates)
        {
            state.Run();
        }
    }
}
