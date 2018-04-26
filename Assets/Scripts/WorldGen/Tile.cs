using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile {
	public readonly int x, y;

	public readonly Vector2Int position;

	public readonly float height, temp, humidity;

	private readonly World world;
	public Region region;
	public readonly Climate climate;

	public Location location;
	private Town Town => location as Town;

	public readonly List<Road> roads = new List<Road>();

	private readonly Color color, heightColor, tempColor, humidityColor;

	private static readonly Color
		LowColor = Color.black,
		HighColor = Color.white,
		ColdColor = Color.cyan,
		HotColor = Color.red,
		DryColor = Color.yellow,
		MoistColor = Color.blue;

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
		humidityColor = Color.Lerp(DryColor, MoistColor, humidity);
	}

	public void SetRegion(Region newRegion) {
		if (region != null) {
			Debug.LogWarning($"Region not null, cancelling ({this}, {region}, {newRegion})");
			return;
		}

		region = newRegion;
	}

	private float GetRaceCompatibility(Race race) => IsWater || !race.height.Contains(height) || !race.temp.Contains(temp)
		? 0
		: 2 * (race.HeightWeight * Mathf.Min(height - race.height.x, race.height.y - height) / race.height.Range() +
		       race.TempWeight * Mathf.Min(temp - race.temp.x, race.temp.y - temp) / race.temp.Range() +
		       race.HumidityWeight * Mathf.Min(humidity - race.humidity.x, race.humidity.y - humidity) / race.humidity.Range());

	public float GetTownCompatibility(Race race) {
		float raceCompatibility = GetRaceCompatibility(race);

		if (raceCompatibility < .001f) return 0;

		float townCompatiblity = world.settings.townDistanceFactor.Evaluate(DistanceToTown());

		float totalCompatibility = (raceCompatibility + townCompatiblity) / 2;

		return totalCompatibility;
	}

	private float DistanceToTown() {
		if (Town != null) return 0;

		int minDist = int.MaxValue;

		foreach (Town town in world.towns) {
			int dist = DistanceSquared(this, town.tile);
			if (dist < minDist) {
				minDist = dist;
			}
		}

		return Mathf.Sqrt(minDist);
	}

	public Color GetColor(MapDrawMode mapDrawMode, float transparency) {
		return mapDrawMode == MapDrawMode.Normal
			? GetColor(MapDrawMode.Normal)
			: Color.Lerp(GetColor(MapDrawMode.Normal), GetColor(mapDrawMode), transparency);
	}

	private Color GetColor(MapDrawMode mapDrawMode) {
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
			default:
				throw new ArgumentOutOfRangeException(nameof(mapDrawMode), mapDrawMode, null);
		}
	}

	public override string ToString() => $"{climate} tile ({x}, {y})";

	private static int DistanceSquared(Tile tile1, Tile tile2) {
		return (tile1.x - tile2.x) * (tile1.x - tile2.x) + (tile1.y - tile2.y) * (tile1.y - tile2.y);
	}
}