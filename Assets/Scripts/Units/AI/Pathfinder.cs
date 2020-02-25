using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
	private List<Node> checkedVector2Ints;
	private List<Node> uncheckedVector2Ints;
	private List<Vector2Int> finalPath;
	private Vector2Int currentStep;
	private Vector2Int final;
	private Vector2Int ownerPosition;

	public (bool moved, Vector2Int nextStep) FindPath(Vector2Int final, Vector2Int ownerPosition)
	{
		this.ownerPosition = ownerPosition;
		checkedVector2Ints = new List<Node>();
		uncheckedVector2Ints = new List<Node>();
		this.final = final;
		if (ownerPosition.Equals(final) ||
		    GameSessionManager.Instance.currentSceneObjects.currentWorld.TileExists(final))
		{
			return (false, World.END_COORDINATE);
		}

		uncheckedVector2Ints.Add(new Node(ownerPosition, null, 0, ComputeCostToEnd(ownerPosition), 0));

		while (uncheckedVector2Ints.Count > 0)
		{
			var current = uncheckedVector2Ints.OrderBy(node => node.CombinedCost()).First();
			uncheckedVector2Ints.Remove(current);
			if (current.coordinate == final)
			{
				var nextStep = ConstructPath(current);
				return (true, nextStep);
			}

			AddNeighbours(current);
			checkedVector2Ints.Add(current);
		}

		return (false, World.END_COORDINATE);
	}

	private double ComputeCostToEnd(Vector2Int start)
	{
		return Math.Max(Math.Abs(final.x - start.x), Math.Abs(final.y - start.y));
	}
	
	private Vector2Int ConstructPath(Node final)
	{
		finalPath = new List<Vector2Int>();
		AddToPath(final);
		finalPath.Add(World.END_COORDINATE);
		return finalPath.First();
	}

	private void AddToPath(Node node)
	{
		if (!node.parent.coordinate.Equals(ownerPosition))
		{
			AddToPath(node.parent);
		}

		finalPath.Add(node.coordinate);
	}

	private void AddNeighbours(Node origin)
	{
		var neighbours = World.NEIGHBOURS;
		foreach (var coordinate in neighbours.Select(neighbour => origin.coordinate + neighbour))
		{
			AddVector2Int(coordinate, origin.curCost, origin);
		}
	}

	private bool AddVector2Int(Vector2Int coordinate, int sum, Node parent)
	{
		var valid = GameSessionManager.Instance.currentSceneObjects.currentWorld.TileExists(coordinate);
		if (valid)
		{
			var newDepth = parent.actionCost + 1;
			var key = checkedVector2Ints.FirstOrDefault(n => n.Equals(coordinate));
			var checkedNotContainsKey = key == null;
			var curCost = 1 + sum;

			if (checkedNotContainsKey)
			{
				if (uncheckedVector2Ints.FirstOrDefault(n => n.Equals(coordinate)) == null)
				{
					var neighbour = new Node(coordinate, parent, curCost, ComputeCostToEnd(coordinate),
						newDepth);
					uncheckedVector2Ints.Add(neighbour);
				}

				return true;
			}

			if (curCost < key.curCost)
			{
				key.SetNewData(parent, curCost, ComputeCostToEnd(coordinate), newDepth);
			}

			return true;
		}

		return false;
	}

	private class Node
	{
		public Vector2Int coordinate;
		public Node parent;
		public int curCost;
		public double straightCostToEnd;
		public int actionCost;

		public Node(Vector2Int coordinate, Node parent, int curCost, double straightCostToEnd, int actionCost)
		{
			this.coordinate = coordinate;
			this.parent = parent;
			this.curCost = curCost;
			this.straightCostToEnd = straightCostToEnd;
			this.actionCost = actionCost;
		}

		public void SetNewData(Node parent, int curCost, double straightCostToEnd, int actionCost)
		{
			this.parent = parent;
			this.curCost = curCost;
			this.straightCostToEnd = straightCostToEnd;
			this.actionCost = actionCost;
		}

		public double CombinedCost()
		{
			return curCost + straightCostToEnd;
		}

		public bool Equals(Node other)
		{
			return coordinate.Equals(other.coordinate);
		}

		public bool Equals(Vector2Int other)
		{
			return coordinate.Equals(other);
		}
	}
}