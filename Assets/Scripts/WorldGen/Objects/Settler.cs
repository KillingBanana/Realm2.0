using UnityEngine;

public class Settler {
	private readonly World world;
	private readonly Town startingTown;
	private Town childTown;
	private Road road;

	private Faction Faction => startingTown.faction;
	private Race Race => startingTown.Race;

	public readonly int population;

	private int pathIndex;

	private Tile[] Path {
		get { return path; }
		set {
			path = value;
			pathIndex = 0;
		}
	}

	private Tile[] path;

	public Tile tile;
	public Tile Goal => Path?[Path.Length - 1];

	public bool Active { get; private set; } = true;

	public Settler(Town town, Tile goal, int population) {
		startingTown = town;
		this.population = population;
		world = town.tile.world;

		tile = town.tile;

		Path = Pathfinding.FindPath(town.tile, goal, town.Race);
	}

	public void Update() {
		if (Path != null) {
			if (road != null) {
				road.AddTile(tile);
				if (tile == Goal) {
					Destroy();
					return;
				}
			} else {
				if (tile == Goal) {
					CreateTown();
					return;
				}
			}

			pathIndex++;
			tile = Path[pathIndex];
		}
	}

	private void CreateTown() {
		if (tile.location != null) {
			Debug.LogError($"{tile} already contains location, removing settlers");
			Destroy();
			return;
		}

		childTown = new Town(tile, Faction, population, startingTown);
		world.towns.Add(childTown);

		road = new Road(childTown);
		Path = Pathfinding.FindPath(tile, startingTown.tile, Race);
	}

	private void Destroy() {
		Active = false;
	}

	/*private Tile FindBestTile() {
		Vector2Int dir = GetDirection(Goal, Tile);

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
	}*/

	public override string ToString() => $"Settlers from {startingTown}";
}