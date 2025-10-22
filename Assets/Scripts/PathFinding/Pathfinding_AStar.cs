using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pathfinding_AStar : PathFinding
{
    [System.Serializable]
    class NodeInformation
    {
        public GridNode node;
        public NodeInformation parent;
        public float gCost;	// base cost
        public float hCost;	// heuristic
        public float fCost;	// combined cost

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

    public Pathfinding_AStar(bool allowDiagonal, bool cutCorners, bool debug_ChangeTileColours = false) : base(allowDiagonal, cutCorners, debug_ChangeTileColours) { }

    public override void GeneratePath(GridNode start, GridNode end)
    {
		//clears the current path
		m_Path.Clear();

		//lists to track the open and closed nodes
        List<NodeInformation> openList = new List<NodeInformation>();
		List<NodeInformation> closedList = new List<NodeInformation>();

        NodeInformation startingNode = new NodeInformation(start, null, 0, Heuristic_Manhattan(start, end));
        openList.Add(startingNode);

        NodeInformation current = startingNode;

		int maxIternation = 0;

		//loop while there is a node selected
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
				//Debug.Log("Path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
				SetPath(current);
				DrawPath(openList, closedList);
				return;
			}

			for (int i = 0; i < 8; ++i)
			{
                GridNode neighbour = current.node.Neighbours[i];

                //Makes sure the neighbour 
                if (neighbour == null || !neighbour.m_Walkable || closedList.Any(x => x.node == neighbour))
                {
                    continue;
                }

                float cost = current.gCost + Maths.Magnitude(current.node.transform.position - neighbour.transform.position);

                NodeInformation newNode = openList.Find(x => x.node == neighbour);

                bool oddDiagnonal = false;

                if (i % 2 != 0 && i != 0)
                {
                    if (!current.node.Neighbours[Maths.CircularClamp(0, 7, i-1)].m_Walkable || !current.node.Neighbours[Maths.CircularClamp(0, 7, i + 1)].m_Walkable)
                    {
                        oddDiagnonal = true;
                    }
                }

                if (!oddDiagnonal)
                {
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
                        openList.Add(newNode);
                    }
                }
            }

            openList.Remove(current);
            closedList.Add(current);

            if (openList.Count > 0)
            {
                current = GetCheapestNode(openList);
            }
            else
            {
                break;
            }
        }
        
        Debug.LogError("No path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
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

