using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class World {
	private readonly Texture2D texture;
	private readonly Color[] colors;

	public readonly int size;

	public readonly WorldSettings settings;

	public float[,] HeightMap { get; private set; }
	private Tile[,] tileMap;

	public readonly List<Region> regions = new List<Region>();
	public readonly List<Civilization> civilizations = new List<Civilization>();
	public readonly List<Town> towns = new List<Town>();

	private readonly Random random;

	private const int MaxAttempts = 1000;

	public World(WorldSettings settings) {
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		this.settings = settings;
		size = settings.Size;

		random = new Random(settings.seed);

		texture = new Texture2D(size, size) {filterMode = FilterMode.Point};
		colors = new Color[size * size];

		GenerateTileMap();
		GenerateRegions();
		GenerateCivs();

		stopwatch.Stop();
		Debug.Log($"World generation finished in {stopwatch.ElapsedMilliseconds}ms");
	}

	public Tile GetTile(int x, int y) => IsInMap(x, y) ? tileMap[x, y] : null;

	private bool IsInMap(int x, int y) => x >= 0 && x < size && y >= 0 && y < size;

	private Tile RandomTile() => GetTile(random.Next(0, size), random.Next(0, size));

	private void GenerateTileMap() {
		tileMap = new Tile[size, size];

		HeightMap = settings.GenerateHeightMap();
		float[,] tempMap = settings.GenerateTempMap(HeightMap);
		float[,] humidityMap = settings.GenerateHumidityMap();

		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				tileMap[x, y] = new Tile(x, y, HeightMap[x, y], tempMap[x, y], humidityMap[x, y]);
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

					if (newTile != null && !(tiles.Contains(newTile) || queue.Contains(newTile)) && newTile.Climate == tile.Climate) {
						queue.Enqueue(newTile);
					}
				}
			}
		}

		OnRegionFound(firstTile.Climate, tiles);
	}

	private void OnRegionFound(Climate climate, HashSet<Tile> tiles) {
		Region region = new Region(climate, tiles);
		regions.Add(region);
	}

	private void GenerateCivs() {
		while (civilizations.Count < settings.civilizations) {
			Race race = GameController.Races.RandomItem(random);
			Civilization civ = new Civilization(this, race);
			civilizations.Add(civ);

			Tile tile;
			int attempts = 0;

			do {
				tile = RandomTile();
				attempts++;
			} while (tile.location != null || !race.IsValidTile(tile) && attempts < MaxAttempts);

			if (attempts >= MaxAttempts) {
				Debug.Log($"Could not find suitable tile for {race}");
				continue;
			}

			int population = 5000 + (int) (race.GetTileCompatibility(tile) * 5000);
			civ.capital = new Town(tile, civ, population);

			towns.Add(civ.capital);
		}
	}

	public Texture2D GetTexture(MapDrawMode mapDrawMode) {
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				colors[x + size * y] = GetTile(x, y).GetColor(mapDrawMode);
			}
		}

		texture.SetPixels(colors);
		texture.Apply();
		return texture;
	}
}

public enum MapDrawMode {
	Normal,
	Height,
	Temperature,
	Humidity,
	Region
}