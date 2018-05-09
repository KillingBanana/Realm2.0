using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Tile : IHeapItem<Tile> {
	public readonly int x, y;

	public readonly Vector2Int position;

	public readonly float height, temp, humidity;

	public readonly World world;
	public Region region;
	public readonly Climate climate;
	public bool IsWater => climate.isWater;

	private bool IsCoast {
		get {
			for (int i = x - 1; i <= x + 1; i++) {
				for (int j = y - 1; j <= y + 1; j++) {
					Tile tile = world.GetTile(i, j);
					if (tile != null && tile.IsWater) {
						return true;
					}
				}
			}

			return false;
		}
	}

	public Location location;
	public Town Town => location as Town;

	public readonly List<Road> roads = new List<Road>();

	private readonly Color color, heightColor, tempColor, humidityColor;

	#region Pathfinding

	public int gCost, hCost;
	public int FCost => gCost + hCost;

	public Tile parent;

	public int HeapIndex { get; set; }

	#endregion

	private static readonly Color
		LowColor = Color.black,
		HighColor = Color.white,
		ColdColor = Color.cyan,
		HotColor = Color.red,
		DryColor = Color.yellow,
		HumidColor = Color.blue;


	public Tile(World world, int x, int y, float height, float temp, float humidity) {
		this.world = world;
		this.x = x;
		this.y = y;
		position = new Vector2Int(x, y);
		this.height = height;
		this.temp = temp;
		this.humidity = humidity;

		climate = Climate.GetClimate(height, temp, humidity);
		if (climate == null) {
			Debug.LogError($"Can't find matching climate for tile (height: {height:F3}, temp: {temp:F3}, humidity: {humidity:F3})");
			throw new ArgumentException();
		}

		color = climate.GetColor(height);
		heightColor = IsWater ? Color.black : Color.Lerp(LowColor, HighColor, height);
		tempColor = Color.Lerp(ColdColor, HotColor, temp);
		humidityColor = Color.Lerp(DryColor, HumidColor, humidity);
	}

	private static float GetCompatibility(float param, Vector2 range, float preferred) {
		if (param <= preferred) {
			return (param - range.x) / (preferred - range.x);
		}

		return 1 - (param - preferred) / (range.y - preferred);
	}

	public float GetRaceCompatibility([NotNull] Race race) => IsWater
		? 0
		: race.HeightWeight * GetCompatibility(height, race.heightRange, race.heightPreferred) +
		  race.TempWeight * GetCompatibility(temp, race.tempRange, race.tempPreferred) +
		  race.HumidityWeight * GetCompatibility(humidity, race.humidityRange, race.humidityPreferred);

	public float GetTownCompatibility(Race race) {
		if (Town != null) return 0;

		float raceCompatibility = GetRaceCompatibility(race);

		if (raceCompatibility <= 0) return 0;

		const int minDistSquared = 64;

		float townCompatibility = 1f;

		foreach (Town town in world.towns) {
			int dist = GetDistanceSquared(this, town.Tile);

			if (dist < minDistSquared) {
				float influenceRange = town.GetInfluenceRange();

				float distance = Mathf.Sqrt(dist);

				if (distance < influenceRange) townCompatibility *= distance / influenceRange;
			}
		}

		float total = raceCompatibility * townCompatibility;

		return race.likesWater && IsCoast
			? Mathf.Sqrt(total)
			: total;
	}

	public List<Tile> GetNeighbors() {
		List<Tile> neighbors = new List<Tile>();

		for (int i = x - 1; i <= x + 1; i++) {
			for (int j = y - 1; j <= y + 1; j++) {
				Tile tile = world.GetTile(i, j);
				if (tile != null) {
					neighbors.Add(tile);
				}
			}
		}

		return neighbors;
	}

	public Color GetColor(MapDrawMode mapDrawMode, float transparency, [CanBeNull] Race race) =>
		IsWater || mapDrawMode == MapDrawMode.Normal
			? GetColor(MapDrawMode.Normal, race)
			: Color.Lerp(GetColor(MapDrawMode.Normal, race), GetColor(mapDrawMode, race), transparency);

	private Color GetColor(MapDrawMode mapDrawMode, Race race) {
		switch (mapDrawMode) {
			case MapDrawMode.Normal:
				return color;
			case MapDrawMode.Height:
				return heightColor;
			case MapDrawMode.Temperature:
				return tempColor;
			case MapDrawMode.Humidity:
				return humidityColor;
			case MapDrawMode.Region:
				return region.color;
			case MapDrawMode.Race:
				return Color.Lerp(LowColor, HighColor, GetRaceCompatibility(race));
			case MapDrawMode.Town:
				return Color.Lerp(LowColor, HighColor, GetTownCompatibility(race));
			default:
				throw new ArgumentOutOfRangeException(nameof(mapDrawMode), mapDrawMode, null);
		}
	}

	public static float GetDistance(Tile a, Tile b) {
		return a == b ? 0 : Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
	}

	public static int GetDistanceSquared(Tile a, Tile b) {
		return a == b ? 0 : (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
	}

	public int CompareTo(Tile other) {
		int costCompare = FCost.CompareTo(other.FCost);

		if (costCompare == 0) {
			costCompare = hCost.CompareTo(other.hCost);
		}

		return -costCompare;
	}

	public override string ToString() => $"{climate} tile ({x}, {y})";
}