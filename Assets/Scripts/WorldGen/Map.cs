using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Map {
	private readonly Texture2D texture;
	private readonly Color[] colors;

	public readonly int size;

	public readonly MapSettings settings;

	public float[,] HeightMap { get; private set; }
	private Tile[,] tileMap;

	public readonly List<Region> regions = new List<Region>();
	public List<Civilization> civilizations = new List<Civilization>();
	public List<Town> towns = new List<Town>();

	private readonly Random random;

	public Map(MapSettings settings) {
		this.settings = settings;
		size = settings.Size;
		random = new Random(settings.seed);

		texture = new Texture2D(size, size) {filterMode = FilterMode.Point};
		colors = new Color[size * size];

		GenerateTileMap();
		GenerateRegions();
		GenerateCivs();
	}

	public Tile GetTile(int x, int y) => IsInMap(x, y) ? tileMap[x, y] : null;

	public bool IsInMap(int x, int y) => x >= 0 && x < size && y >= 0 && y < size;

	public Region RandomRegion() {
		Region region = null;
		while (region == null || region.IsWater) {
			region = regions.RandomItem(random);
		}

		return region;
	}

	public Tile RandomTile() => GetTile(random.Next(0, size), random.Next(0, size));

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
				if (tile.region != null) continue;

				List<Tile> tiles = FindRegion(tile, 1);
				Region region = new Region(tile.Climate, tiles);
				regions.Add(region);
			}
		}
	}

	private List<Tile> FindRegion(Tile firstTile, int range) {
		List<Tile> tiles = new List<Tile>();
		Queue<Tile> queue = new Queue<Tile>();
		firstTile.regionPending = true;
		queue.Enqueue(firstTile);

		while (queue.Count > 0) {
			Tile tile = queue.Dequeue();
			tiles.Add(tile);

			for (int j = -range; j <= range; j++) {
				for (int i = -range; i <= range; i++) {
					Tile newTile = GetTile(tile.x + i, tile.y + j);

					if (newTile == null || newTile.regionPending || newTile.Climate != tile.Climate ||
					    newTile.region != null) continue;

					queue.Enqueue(newTile);
					newTile.regionPending = true;
				}
			}
		}

		return tiles;
	}

	private void GenerateCivs() {
		while (civilizations.Count < settings.civilizations) {
			Race race = GameController.RandomRace();
			Civilization civ = new Civilization(this, race);
			civilizations.Add(civ);
			Tile tile = null;
			int attempts = 0;
			while ((tile == null || !race.IsValidTile(tile)) && attempts < 100) {
				tile = RandomTile();
				attempts++;
			}

			if (attempts >= 100) {
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