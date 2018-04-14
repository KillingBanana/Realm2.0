using System.Collections.Generic;
using UnityEngine;

public class Settler {
	private readonly Town startingTown;

	private readonly List<Tile> tiles;
	private readonly int population;

	private readonly World world;
	public Tile Tile => tiles[0];
	private Faction Faction => startingTown.faction;
	private Race Race => startingTown.Race;

	public bool active = true;

	private int steps;
	private float standards = 1;

	public Settler(Town town, int population) {
		startingTown = town;
		this.population = population;
		world = town.world;

		Vector2Int startDirection = Utility.RandomDirection();

		tiles = new List<Tile> {world.GetTile(town.tile.position + startDirection), town.tile};
	}

	public void Update() {
		if (!active) return;

		steps++;

		Tile nextTile = FindBestTile(Tile, tiles[1]) ?? FindBestTile(Tile, Tile);

		if (nextTile == null) {
			CreateTown();
			return;
		}

		tiles.Insert(0, nextTile);

		if (Tile.GetTownCompatibility(Race) >= standards) {
			CreateTown();
			return;
		}

		standards -= .015f;
	}

	private void CreateTown() {
		if (Tile.location != null) {
			Debug.LogError("Location not null");
			active = false;
			return;
		}

		Town town = new Town(world, Tile, Faction, population, startingTown);
		world.towns.Add(town);
		active = false;
	}

	private Tile FindBestTile(Tile currentTile, Tile previousTile) {
		int dx = currentTile.x - previousTile?.x ?? 0;
		int dy = currentTile.y - previousTile?.y ?? 0;

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

				Tile tile = GameController.World.GetTile(currentTile.x + x, currentTile.y + y);

				if (tile == null || tile.location != null || tile.IsWater) continue;

				float compatibility = tile.GetTownCompatibility(Race);

				if (compatibility > bestCompatibility) {
					bestTile = tile;
					bestCompatibility = compatibility;
				}
			}
		}

		return bestTile;
	}

	public override string ToString() => $"Settlers from {startingTown.Name}";
}