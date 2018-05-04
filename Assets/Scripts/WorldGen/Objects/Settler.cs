using UnityEngine;

public class Settler {
	private readonly World world;
	public readonly Town startingTown;
	private readonly Road road;

	private Faction Faction => startingTown.faction;
	private Race Race => startingTown.Race;

	public readonly int population;

	private int pathIndex = 0;
	private readonly Tile[] path;
	public Tile Tile => path[Mathf.Min(pathIndex, path.Length - 1)];

	private readonly Tile goal;

	public bool Active { get; private set; } = true;

	public Settler(Town town, Tile goal, int population) {
		startingTown = town;
		this.population = population;
		this.goal = goal;
		world = town.tile.world;

		road = new Road(startingTown, population);


		path = Pathfinding.FindPath(town.tile, goal, town.Race);

		if (path == null) {
			Debug.LogError($"No path found from {town} to {goal}");
			Destroy();
		}
	}

	public void Update() {
		if (Tile == goal) {
			road.AddTile(Tile);
			CreateTown();
			return;
		}

		pathIndex++;

		road.AddTile(Tile);
	}

	private void CreateTown() {
		if (Tile.location != null) {
			Debug.LogError($"{Tile} already contains location, removing settlers");
			Destroy();
			return;
		}

		Town town = new Town(Tile, Faction, population, startingTown);
		world.towns.Add(town);

		town.roads.Add(road);

		Destroy();
	}

	private void Destroy() {
		Active = false;
	}

	private Tile FindBestTile() {
		Vector2Int dir = GetDirection(goal, Tile);

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

				Tile newTile = GameController.World.GetTile(Tile.x + x, Tile.y + y);

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