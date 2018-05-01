using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class Tile {
	public readonly int x, y;

	public readonly Vector2Int position;

	public readonly float height, temp, humidity;

	private readonly World world;
	public Region region;
	public readonly Climate climate;

	public Location location;
	public Town Town => location as Town;

	public readonly List<Road> roads = new List<Road>();

	private readonly Color color, heightColor, tempColor, humidityColor;

	private static readonly Color
		LowColor = Color.black,
		HighColor = Color.white,
		ColdColor = Color.cyan,
		HotColor = Color.red,
		DryColor = Color.yellow,
		HumidColor = Color.blue;

	public bool IsWater => climate.isWater;

	public Tile(World world, int x, int y, float height, float temp, float humidity) {
		this.world = world;
		this.x = x;
		this.y = y;
		position = new Vector2Int(x, y);
		this.height = height;
		this.temp = temp;
		this.humidity = humidity;

		try {
			climate = GameController.Climates.First(climate => climate.CorrectTile(this));
		}
		catch (Exception) {
			Debug.LogError($"Can't find matching climate for tile (height: {height:F3}, temp: {temp:F3}, humidity: {humidity:F3})");
			throw;
		}

		color = climate.GetColor(height);
		heightColor = IsWater ? Color.black : Color.Lerp(LowColor, HighColor, height);
		tempColor = Color.Lerp(ColdColor, HotColor, temp);
		humidityColor = Color.Lerp(DryColor, HumidColor, humidity);
	}

	public void SetRegion(Region newRegion) {
		if (region != null) {
			Debug.LogWarning($"Region not null, cancelling ({this}, {region}, {newRegion})");
			return;
		}

		region = newRegion;
	}

	private static float GetCompatibility(float param, Vector2 range, float preferred) {
		if (param <= preferred) {
			return (param - range.x) / (preferred - range.x);
		} else {
			return 1 - (param - preferred) / (range.y - preferred);
		}
	}

	public float GetRaceCompatibility(Race race) => IsWater
		? 0
		: race.HeightWeight * GetCompatibility(height, race.heightRange, race.heightPreferred) +
		  race.TempWeight * GetCompatibility(temp, race.tempRange, race.tempPreferred) +
		  race.HumidityWeight * GetCompatibility(humidity, race.humidityRange, race.humidityPreferred);

	public float GetTownCompatibility(Race race) {
		float raceCompatibility = GetRaceCompatibility(race);

		if (raceCompatibility <= 0) return 0;

		Town nearestTown = null;
		int minDistSquared = 64;

		foreach (Town town in world.towns) {
			int dist = DistanceSquared(this, town.tile);
			if (dist < minDistSquared) {
				minDistSquared = dist;
				nearestTown = town;
			}
		}

		if (nearestTown == null) return raceCompatibility;

		float influenceRange = 1 + Mathf.Log(nearestTown.population / 31.25f, 2);

		float distance = Mathf.Sqrt(minDistSquared);

		if (distance >= influenceRange) return raceCompatibility;

		float townCompatiblity = distance / influenceRange;

		return raceCompatibility * townCompatiblity;
	}

	public Color GetColor(MapDrawMode mapDrawMode, float transparency, [CanBeNull] Race race) =>
		mapDrawMode == MapDrawMode.Normal
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
				return IsWater ? color : region.color;
			case MapDrawMode.Race:
				return Color.Lerp(LowColor, HighColor, GetRaceCompatibility(race));
			case MapDrawMode.Town:
				return Color.Lerp(LowColor, HighColor, GetTownCompatibility(race));
			default:
				throw new ArgumentOutOfRangeException(nameof(mapDrawMode), mapDrawMode, null);
		}
	}

	public override string ToString() => $"{climate} tile ({x}, {y})";

	private static int DistanceSquared(Tile tile1, Tile tile2) {
		return (tile1.x - tile2.x) * (tile1.x - tile2.x) + (tile1.y - tile2.y) * (tile1.y - tile2.y);
	}
}