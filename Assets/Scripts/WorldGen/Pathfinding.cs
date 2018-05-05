using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class Pathfinding {
	[CanBeNull]
	public static Tile[] FindPath(Tile start, Tile goal, Race race) {
		Heap<Tile> open = new Heap<Tile>(start.world.squareSize);
		HashSet<Tile> closed = new HashSet<Tile>();

		open.Add(start);

		while (open.Count > 0) {
			Tile current = open.RemoveFirst();
			closed.Add(current);

			if (current == goal) {
				return RetracePath(start, goal);
			}

			foreach (Tile neighbor in current.GetNeighbors()) {
				if (neighbor.IsWater || closed.Contains(neighbor)) continue;

				int movementCost = current.gCost + GetDistance(current, neighbor) + Mathf.RoundToInt((1 - neighbor.GetRaceCompatibility(race)) * 10);

				if (movementCost < neighbor.gCost || !open.Contains(neighbor)) {
					neighbor.gCost = movementCost;
					neighbor.hCost = GetDistance(neighbor, goal);
					neighbor.parent = current;

					if (open.Contains(neighbor)) {
						open.Update(neighbor);
					} else {
						open.Add(neighbor);
					}
				}
			}
		}

		return null;
	}

	private static Tile[] RetracePath(Tile start, Tile goal) {
		List<Tile> path = new List<Tile>();

		Tile current = goal;
		while (current != start) {
			path.Insert(0, current);
			current = current.parent;
		}

		return path.ToArray();
	}

	private static int GetDistance(Tile a, Tile b) {
		int dx = (a.x - b.x).Abs();
		int dy = (a.y - b.y).Abs();

		if (dx > dy) return 14 * dy + 10 * (dx - dy);
		return 14 * dx + 10 * (dy - dx);
	}
}