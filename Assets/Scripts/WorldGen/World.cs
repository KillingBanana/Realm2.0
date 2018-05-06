using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class World {
	public readonly int size;
	public readonly int squareSize;

	public readonly WorldSettings settings;

	private readonly Tile[,] tileMap;
	public readonly float[,] heightMap;

	public readonly List<Region> regions = new List<Region>();
	public readonly List<Faction> factions = new List<Faction>();
	public readonly List<Town> towns = new List<Town>();

	private readonly Random random;

	private const int MaxAttempts = 1000;

	public int Days { get; private set; }

	public World(WorldSettings settings) {
		this.settings = settings;
		size = settings.Size;
		squareSize = size * size;
		tileMap = new Tile[size, size];
		heightMap = new float[size, size];

		random = new Random(settings.seed);
	}

	private void Benchmark(string str, Stopwatch stopwatch) {
		if (settings.benchmark) {
			Debug.Log(string.Format(str, stopwatch.ElapsedMilliseconds));
			stopwatch.Restart();
		}
	}

	public void Generate() {
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		GenerateTileMap();

		Benchmark("Tilemap generated in {0}ms", stopwatch);

		GenerateRegions();

		Benchmark("Regions generated in {0}ms", stopwatch);

		GenerateCivs();

		Benchmark("World generated in {0}ms", stopwatch);

		while (Days < settings.days) Update();

		Benchmark("Days simulated in {0}ms", stopwatch);

		stopwatch.Stop();
	}

	[CanBeNull]
	public Tile GetTile(Vector2Int v) => GetTile(v.x, v.y);

	[CanBeNull]
	public Tile GetTile(int x, int y) => IsInMap(x, y) ? tileMap[x, y] : null;

	private bool IsInMap(int x, int y) => x >= 0 && x < size && y >= 0 && y < size;

	private Tile RandomTile() {
		return GetTile(random.Next(0, size), random.Next(0, size));
	}

	private void GenerateTileMap() {
		settings.GenerateHeightMap(heightMap);
		float[,] tempMap = settings.GenerateTempMap(heightMap);
		float[,] humidityMap = settings.GenerateHumidityMap(heightMap);

		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				tileMap[x, y] = new Tile(this, x, y, heightMap[x, y], tempMap[x, y], humidityMap[x, y]);
			}
		}
	}

	private void GenerateRegions() {
		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				Tile tile = tileMap[x, y];
				if (tile.region == null) FindRegion(tile);
			}
		}
	}

	private void FindRegion(Tile firstTile) {
		HashSet<Tile> tiles = new HashSet<Tile>();
		Queue<Tile> queue = new Queue<Tile>();
		queue.Enqueue(firstTile);

		while (queue.Count > 0) {
			Tile tile = queue.Dequeue();
			tiles.Add(tile);

			for (int j = -1; j <= 1; j++) {
				for (int i = -1; i <= 1; i++) {
					Tile newTile = GetTile(tile.x + i, tile.y + j);

					if (newTile != null && !(tiles.Contains(newTile) || queue.Contains(newTile)) && newTile.climate == tile.climate) {
						queue.Enqueue(newTile);
					}
				}
			}
		}

		OnRegionFound(firstTile.climate, tiles);
	}

	private void OnRegionFound(Climate climate, HashSet<Tile> tiles) {
		Region region = new Region(climate, tiles);
		regions.Add(region);
	}

	private void GenerateCivs() {
		int failedFactions = 0;
		while (factions.Count + failedFactions < settings.factions) {
			Race race = GameController.Races.RandomItem();
			Faction faction = new Faction(race);
			factions.Add(faction);

			Tile tile = GetBestTile(race, 100);

			if (tile == null) {
				Debug.Log($"Could not find suitable tile for {race}");
				failedFactions++;
				continue;
			}

			int population = (int) (tile.GetRaceCompatibility(race) * 5000);
			faction.capital = new Town(tile, faction, population, null);

			towns.Add(faction.capital);
		}
	}

	private Tile GetBestTile(Race race, int tries) {
		int attempts = 0;

		Tile bestTile = null;
		do {
			Tile tile = GetRandomTile(race);
			if (bestTile == null || tile.GetTownCompatibility(race) > bestTile.GetTownCompatibility(race)) bestTile = tile;

			attempts++;
		} while (attempts < tries && attempts < MaxAttempts);

		return bestTile;
	}

	private Tile GetRandomTile(Race race) {
		int attempts = 0;
		Tile tile;
		do {
			tile = RandomTile();
			attempts++;
		} while (tile.location != null || !race.IsValidTile(tile) && attempts < MaxAttempts);

		return tile;
	}

	public void Update() {
		for (int i = 0; i < towns.Count; i++) {
			Town town = towns[i];
			town.Update();
		}

		Days++;
	}
}

public enum MapDrawMode {
	Normal,
	Height,
	Temperature,
	Humidity,
	Region,
	Race,
	Town
}