using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Settler {
	private readonly Town startingTown;

	private readonly List<Tile> tiles;
	private readonly int population;

	private static World World => GameController.World;
	private Tile Tile => tiles[0];
	private Faction Faction => startingTown.faction;

	private bool active = true;

	private int steps;
	private float standards = 1;

	public Settler(Town town, int population) {
		startingTown = town;
		this.population = population;

		Vector2Int startDirection = Utility.RandomDirection();

		tiles = new List<Tile> {World.GetTile(town.tile.position + startDirection), town.tile};
		if (World.settings.drawRoads) Tile.customColor = Faction.color;
	}

	public void Update() {
		if (!active) return;

		steps++;

		Tile nextTile = FindBestTile();

		if (nextTile == null) {
			CreateTown();
			active = false;
			return;
		}

		if (World.settings.drawRoads && tiles.Count > 1) Tile.customColor = Color.clear;

		tiles.Insert(0, nextTile);

		if (World.settings.drawRoads) Tile.customColor = Faction.color;

		float compatibility = GetTownCompatibility(Tile);

		if (compatibility > standards) {
			CreateTown();
		} else {
			standards *= .98f;
		}
	}

	private void CreateTown() {
		Tile tile = tiles.FirstOrDefault(t => !t.IsWater && t.location == null);

		if (tile == null) {
			Debug.Log($"Can't find valid town location for settlers from {startingTown}");
			active = false;
			return;
		}

		Town town = new Town(tile, Faction, population);
		World.towns.Add(town);
		active = false;
	}

	private Tile FindBestTile() {
		int dx = tiles.Count == 1 ? 0 : Tile.x - tiles[1].x;
		int dy = tiles.Count == 1 ? 0 : Tile.y - tiles[1].y;

		int minX = dx == 0
			? -1
			: dy == 0
				? dx
				: Mathf.Min(0, dx);

		int maxX = dx == 0
			? 1
			: dy == 0
				? dx
				: Mathf.Max(0, dx);

		int minY = dy == 0
			? -1
			: dx == 0
				? dy
				: Mathf.Min(0, dy);

		int maxY = dy == 0
			? 1
			: dx == 0
				? dy
				: Mathf.Max(0, dy);

		Tile bestTile = null;
		float bestCompatibility = 0;

		for (int x = minX; x <= maxX; x++) {
			for (int y = minY; y <= maxY; y++) {
				if (x == 0 && y == 0) continue;

				Tile newTile = GameController.World.GetTile(Tile.x + x, Tile.y + y);

				if (newTile == null || newTile.location != null || newTile.IsWater) continue;

				float compatibility = GetTownCompatibility(newTile);

				if (bestTile == null || compatibility > bestCompatibility) {
					bestTile = newTile;
					bestCompatibility = compatibility;
				}
			}
		}

		return bestTile;
	}

	private float GetTownCompatibility(Tile tile) {
		float raceCompatibility = Faction.race.GetTileCompatibility(tile);
		float townCompatiblity = 0.005f * Mathf.Pow(DistanceToTown(tile), 2.32f);

		return (raceCompatibility + townCompatiblity) / 2;
	}

	private static float DistanceToTown(Tile tile) {
		if (tile.Town != null) return 0;
		int minDistance = int.MaxValue;
		foreach (Town town in World.towns) {
			int dist = Tile.DistanceSquared(tile, town.tile);
			if (dist < minDistance) {
				minDistance = dist;
			}
		}

		return Mathf.Sqrt(minDistance);
	}
}