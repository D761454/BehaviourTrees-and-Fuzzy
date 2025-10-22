using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class Pathfinding_JPS : PathFinding
{
	[System.Serializable]
	class NodeInformation
	{
		public GridNode node;
		public NodeInformation parent;
		public float gCost;
		public float hCost;
		public float fCost;

		public NodeInformation(GridNode node, NodeInformation parent, float gCost, float hCost)
		{
			this.node = node;
			this.parent = parent;
			this.gCost = gCost;
			this.hCost = hCost;
			fCost = gCost + hCost;
		}

		public void UpdateNodeInformation(NodeInformation parent, float gCost, float hCost)
		{
			this.parent = parent;
			this.gCost = gCost;
			this.hCost = hCost;
			fCost = gCost + hCost;
		}
	}

	public Pathfinding_JPS(bool allowDiagonal, bool cutCorners, bool debug_ChangeTileColours = false) : base(allowDiagonal, cutCorners, debug_ChangeTileColours) { }

	public override void GeneratePath(GridNode start, GridNode end)
	{
		m_Path.Clear();

        List<NodeInformation> openSet = new List<NodeInformation>();
        List<NodeInformation> closedSet = new List<NodeInformation>();

        NodeInformation startingNode = new NodeInformation(start, null, 0, Heuristic_Manhattan(start, end));
        openSet.Add(startingNode);

        NodeInformation current = startingNode;

        int maxIternation = 0;

        while (current != null)
        {
            maxIternation++;
            if (maxIternation > m_MaxPathCount)
            {
                Debug.LogError("Max Iteration Reached");
                break;
            }

            //if the current node is the end node, a path has been found.
            if (current.node == end)
            {
                Debug.Log("Path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
                SetPath(current);
                DrawPath(openSet, closedSet);
                return;
            }

            //// jump pt if cell infront or to side is taken
            for (int i = 0; i < 8; ++i)
            {
                GridNode neighbour = current.node.Neighbours[i];

                //Makes sure the neighbour 
                if (neighbour == null || !neighbour.m_Walkable || closedSet.Any(x => x.node == neighbour))
                {
                    continue;
                }

                if (neighbour == end)
                {
                    current.node = end;
                    continue;
                }

                if (i % 2 != 0)
                {
                    neighbour = SearchNode(neighbour, Maths.CircularClamp(0, 7, i-1), Maths.CircularClamp(0, 7, i+1));
                }
                else
                {
                    neighbour = SearchNode(neighbour, i);
                }

                //Makes sure the neighbour 
                if (neighbour == null || !neighbour.m_Walkable || closedSet.Any(x => x.node == neighbour))
                {
                    continue;
                }

                float cost = current.gCost + Maths.Magnitude(current.node.transform.position - neighbour.transform.position);

                NodeInformation newNode = openSet.Find(x => x.node == neighbour);

                if (newNode != null)
                {
                    if (cost + Heuristic_Manhattan(neighbour, end) < newNode.fCost)
                    {
                        newNode.UpdateNodeInformation(current, cost, Heuristic_Manhattan(neighbour, end));
                    }
                }
                else
                {
                    newNode = new NodeInformation(neighbour, current, cost, Heuristic_Manhattan(neighbour, end));
                    openSet.Add(newNode);
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (openSet.Count > 0)
            {
                current = GetCheapestNode(openSet);
            }
            else
            {
                break;
            }
        }

        Debug.LogError("No path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
    }

    /// <summary>
    /// searches along orthogonal axis for forced neighbour
    /// </summary>
    /// <param name="node"></param>
    /// <param name="dir1"></param>
    /// <returns></returns>
    GridNode SearchNode(GridNode node, int dir1)
    {
        while (node != null && node.m_Walkable)
        {
            if (!node.Neighbours[Maths.CircularClamp(0, 7, dir1 - 3)].m_Walkable 
                || !node.Neighbours[Maths.CircularClamp(0, 7, dir1 + 3)].m_Walkable)
            {
                return node;
            }
            node = node.Neighbours[dir1];
        }
        return null;
    }

    /// <summary>
    /// searches diagonally for a valid cell, then calls orthogonal search along axis for forced neighbour
    /// </summary>
    /// <param name="node"></param>
    /// <param name="dir1"></param>
    /// <param name="dir2"></param>
    /// <returns></returns>
    GridNode SearchNode(GridNode node, int dir1, int dir2)
    {
        GridNode nodeDir1 = null, nodeDir2 = null;

        if (node == null || !node.m_Walkable)
        {
            return null;
        }

        if (node.Neighbours[Maths.CircularClamp(0, 7, dir1 + 1)] != null && !node.Neighbours[Maths.CircularClamp(0, 7, dir1 + 1)].m_Walkable)
        {
            return node;
        }

        nodeDir1 = SearchNode(node, dir1);

        if (nodeDir1 != null)
        {
            return nodeDir1;
        }

        nodeDir2 = SearchNode(node, dir2);

        if (nodeDir2 != null)
        {
            return nodeDir2;
        }

        if (node.Neighbours[Maths.CircularClamp(0, 7, dir1 + 1)] != null && node.Neighbours[Maths.CircularClamp(0, 7, dir1 + 1)].m_Walkable)
        {
            SearchNode(node.Neighbours[Maths.CircularClamp(0, 7, dir1 + 1)], dir1, dir2);
        }

        return null;
    }

    /// <summary>
	/// pass in the final node information and sets m_Path
	/// </summary>
	private void SetPath(NodeInformation end)
    {
        NodeInformation current = end;
        while (current != null)
        {
            m_Path.Add(current.node.transform.position);
            current = current.parent;
        }

        m_Path.Reverse();
    }

    /// <summary>
    /// Returns the cheapest node in the list calculated by cost
    /// </summary>
    private NodeInformation GetCheapestNode(List<NodeInformation> nodes)
    {
        return nodes.OrderBy(n => n.fCost).First();
    }

    /// <summary>
    /// Changest the colour of the grid based on the values passed in
    /// </summary>
    void DrawPath(List<NodeInformation> open, List<NodeInformation> closed)
    {
        //drawPath
        if (m_Debug_ChangeTileColours)
        {
            TileGrid.ResetGridNodeColours();

            foreach (NodeInformation node in closed)
            {
                node.node.SetOpenInPathFinding();
            }

            foreach (NodeInformation node in open)
            {
                node.node.SetClosedInPathFinding();
            }
        }
    }
}

