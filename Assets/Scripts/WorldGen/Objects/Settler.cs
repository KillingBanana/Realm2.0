using UnityEngine;

public class Settler {
	private readonly World world;
	public readonly Town startingTown;
	private readonly Road road;

	private Faction Faction => startingTown.faction;
	private Race Race => startingTown.Race;

	public readonly int population;

	public Tile tile;
	private readonly Tile goal;

	public bool Active { get; private set; } = true;

	public Settler(Town town, Tile goal, int population) {
		startingTown = town;
		this.population = population;
		world = town.tile.world;

		road = new Road(startingTown, population);

		tile = goal; //town.tile;
		this.goal = goal;
	}

	public void Update() {
		if (tile == goal) {
			road.AddTile(tile);
			CreateTown();
			return;
		}

		Tile nextTile = FindBestTile();

		tile = nextTile;

		road.AddTile(tile);
	}

	private void Destroy() {
		Active = false;
	}

	private void CreateTown() {
		if (tile.location != null) {
			Debug.LogError($"{tile} already contains location, removing settlers");
			Destroy();
			return;
		}

		Town town = new Town(tile, Faction, population, startingTown);
		world.towns.Add(town);

		town.roads.Add(road);

		Destroy();
	}

	private Tile FindBestTile() {
		Vector2Int dir = GetDirection(goal, tile);

		int minX = dir.x == 0
			? -1
			: dir.y == 0
				? dir.x
				: Mathf.Min(0, dir.x);

		int maxX = dir.x == 0
			? 1
			: dir.y == 0
				? dir.x
				: Mathf.Max(0, dir.x);

		int minY = dir.y == 0
			? -1
			: dir.x == 0
				? dir.y
				: Mathf.Min(0, dir.y);

		int maxY = dir.y == 0
			? 1
			: dir.x == 0
				? dir.y
				: Mathf.Max(0, dir.y);

		Tile bestTile = null;

		for (int x = minX; x <= maxX; x++) {
			for (int y = minY; y <= maxY; y++) {
				if (x == 0 && y == 0) continue;
				if (world.settings.wigglyRoads && x == dir.x && y == dir.y) continue;

				Tile newTile = GameController.World.GetTile(tile.x + x, tile.y + y);

				if (newTile == null || newTile.location != null || newTile.IsWater) continue;

				float compatibility = newTile.GetRaceCompatibility(Race);

				if (bestTile == null || compatibility > bestTile.GetRaceCompatibility(Race)) {
					bestTile = newTile;
				}
			}
		}

		return bestTile;
	}

	private static Vector2Int GetDirection(Tile goalTile, Tile startTile) {
		Vector2Int dir = new Vector2Int(
			(goalTile.x - startTile.x).Sign(),
			(goalTile.y - startTile.y).Sign()
		);

		return dir;
	}

	public override string ToString() => $"Settlers from {startingTown.Name}";
}