using System.Collections.Generic;
using UnityEngine;

public class Settler {
	private readonly World world;
	public readonly Town startingTown;
	private readonly Road road;

	public readonly List<Tile> tiles;
	public readonly int population;

	public Tile Tile => tiles[0];
	private Faction Faction => startingTown.faction;
	private Race Race => startingTown.Race;

	public bool Active { get; private set; } = true;

	private int steps;
	private float standards = 1;

	public Settler(Town town, int population) {
		startingTown = town;
		this.population = population;
		world = town.world;

		road = new Road(world, startingTown, population);

		tiles = new List<Tile> {town.tile};
	}

	public void Update() {
		if (!Active) {
			Debug.LogError("This shouldn't be happening");
			return;
		}

		steps++;

		Tile nextTile = tiles.Count == 1
			? FindBestTile(Tile, startingTown.parent?.tile)
			: FindBestTile(Tile, tiles[1]) ?? FindBestTile(Tile);

		if (nextTile == null) {
			CreateTown(Tile);
			return;
		}

		if (nextTile.GetTownCompatibility(Race) >= standards) {
			CreateTown(nextTile);
			return;
		}

		tiles.Insert(0, nextTile);
		standards -= .02f;

		road.AddTile(Tile);
	}

	private void CreateTown(Tile tile) {
		Town town = new Town(world, tile, Faction, population, startingTown);
		world.towns.Add(town);

		town.roads.Add(road);

		road.AddTile(tile);

		Active = false;
	}

	private Tile FindBestTile(Tile currentTile, Tile previousTile = null) {
		int dx = 0, dy = 0;

		if (previousTile != null) {
			dx = (currentTile.x - previousTile.x).Sign();
			dy = (currentTile.y - previousTile.y).Sign();
		}

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