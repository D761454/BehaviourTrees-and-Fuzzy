using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathFinding
{
    protected GridNode[] m_GridNodes;

    [Header("Settings")]
	[Tooltip("Sets whether the agent can move diagonally")]
	public bool m_AllowDiagonal;
	[Tooltip("Sets whether the agent can cut corners when moving diagonally")]
	public bool m_CanCutCorners;

    /// <summary>
    /// Caps the max iteration count when calculating paths. Useful to stop infinate loops
    /// </summary>
    protected int m_MaxPathCount = 10000;

    [Tooltip("Changes the colour of the grid to show pathing")]
    [SerializeField]
    public bool m_Debug_ChangeTileColours;

	public List<Vector2> m_Path { get; protected set; }

    public PathFinding(bool allowDiagonal, bool cutCorners, bool debug_ChangeTileColours = false)
    {
        m_Path = new List<Vector2>();
        m_AllowDiagonal = allowDiagonal;
        m_CanCutCorners = cutCorners;
        m_GridNodes = TileGrid.GridNodes;
        m_Debug_ChangeTileColours |= debug_ChangeTileColours;
    }

    public abstract void GeneratePath(GridNode start, GridNode end);

    public Vector2 GetClosestPointOnPath(Vector2 position)
    {
        float distance = float.MaxValue;
        int closestPoint = int.MaxValue;

        for (int i = 0; i < m_Path.Count; ++i)
        {
            float tempDistance = Maths.Magnitude(m_Path[i] - position);
            if (tempDistance < distance)
            {
                closestPoint = i;
                distance = tempDistance;
            }
        }

        for (int j = 0; j <= closestPoint - 1; ++j)
        {
            m_Path.RemoveAt(0);
        }

        return m_Path[0];
    }

    public Vector2 GetNextPointOnPath(Vector2 position)
    {
        Vector2 pos = position;
        if (m_Path.Count > 0)
        {
            m_Path.RemoveAt(0);

            if (m_Path.Count > 0)
                pos = m_Path[0];
        }

        return pos;

    }

    protected float Heuristic_Manhattan(GridNode start, GridNode end)
    {
        return Mathf.Abs(end.transform.position.x - start.transform.position.x) + Mathf.Abs(end.transform.position.y - start.transform.position.y);
    }

    protected float Heuristic_Euclidean(GridNode start, GridNode end)
    {
        float a = Mathf.Abs(end.transform.position.x - start.transform.position.x); 
        float b = Mathf.Abs(end.transform.position.y - start.transform.position.y);

        return Mathf.Sqrt((a*a) + (b*b));
    }

    protected float Heuristic_Octile(GridNode start, GridNode end)
    {
        float a = Mathf.Abs(end.transform.position.x - start.transform.position.x);
        float b = Mathf.Abs(end.transform.position.y - start.transform.position.y);

        return Mathf.Max(a, b) + (0.41f * Mathf.Min(a, b));
    }

    protected float Heuristic_Chebyshev(GridNode start, GridNode end)
    {
        float a = Mathf.Abs(end.transform.position.x - start.transform.position.x);
        float b = Mathf.Abs(end.transform.position.y - start.transform.position.y);

        return Mathf.Max(a, b);
    }
}
