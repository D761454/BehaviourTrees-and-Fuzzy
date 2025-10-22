using System.Collections.Generic;
using System.Linq;
using Unity.Loading;
using UnityEngine;

[System.Serializable]
public class Pathfinding_Dijkstra : PathFinding
{
	[System.Serializable]
	class NodeInformation
	{
		public GridNode node { get; private set; }
		public NodeInformation parent { get; private set; }
		public float cost { get; private set; }

		public NodeInformation(GridNode node, NodeInformation parent, float cost)
		{
			this.node = node;
			this.parent = parent;
			this.cost = cost;
		}

		public void UpdateNodeInformation(NodeInformation parent, float cost)
		{
			this.parent = parent;
			this.cost = cost;
		}
	}

	public Pathfinding_Dijkstra(bool allowDiagonal, bool cutCorners, bool debug_ChangeTileColours = false) : base(allowDiagonal, cutCorners, debug_ChangeTileColours) { }

	public override void GeneratePath(GridNode start, GridNode end)
	{
		//clears the current path
		m_Path.Clear();
		
		//lists to track visited and none visited nodes
		List<NodeInformation> visited = new List<NodeInformation>();
		List<NodeInformation> notVisited = new List<NodeInformation>();

		NodeInformation startingNode = new NodeInformation(start, null, 0);
		notVisited.Add(startingNode);

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
				Debug.Log("Path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
				SetPath(current);
				DrawPath(visited, notVisited);
				return;
			}

			//check all 8 neighbours of the current tile. 0 = up and then goes clockwise.
			for (int i = 0; i < 8; ++i)
			{
				GridNode neighbour = current.node.Neighbours[i];

				//Makes sure the neighbour 
				if (neighbour == null || !neighbour.m_Walkable || visited.Any(x => x.node == neighbour))
				{
					continue;
				}

                float cost = current.cost + Maths.Magnitude(current.node.transform.position - neighbour.transform.position);

				//checks if node is in the list
				NodeInformation newNode = notVisited.Find(x => x.node == neighbour);

				bool oddDiagnonal = false;

                if (i % 2 != 0 && i != 0)
                {
                    if (!current.node.Neighbours[Maths.CircularClamp(0, 7, i - 1)].m_Walkable || !current.node.Neighbours[Maths.CircularClamp(0, 7, i + 1)].m_Walkable)
                    {
                        oddDiagnonal = true;
                    }
                }

                if (!oddDiagnonal)
				{
                    if (newNode != null)
                    {
                        if (cost < newNode.cost)
                        {
                            newNode.UpdateNodeInformation(current, cost);
                        }
                    }
                    else
                    {
                        newNode = new NodeInformation(neighbour, current, cost);
                        notVisited.Add(newNode);
                    }
                }
            }

            notVisited.Remove(current);
            visited.Add(current);

            if (notVisited.Count > 0)
			{
				current = GetCheapestNode(notVisited);
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
		return nodes.OrderBy(n => n.cost).First();
	}

	/// <summary>
	/// Changest the colour of the grid based on the values passed in
	/// </summary>
	void DrawPath(List<NodeInformation> visited, List<NodeInformation> notVisited)
	{
		//drawPath
		if (m_Debug_ChangeTileColours)
		{
			TileGrid.ResetGridNodeColours();

			foreach (NodeInformation node in notVisited)
			{
				node.node.SetOpenInPathFinding();
			}

			foreach (NodeInformation node in visited)
			{
				node.node.SetClosedInPathFinding();
			}
		}
	}
}

