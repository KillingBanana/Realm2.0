using System;
using System.Linq;
using UnityEngine;

public class Tile {
	public readonly int x, y;
	public readonly float height, temp, humidity;

	public Region region;
	public Location location;

	public readonly Climate climate;

	public Color customColor = Color.clear;
	private readonly Color color, heightColor, tempColor, humidityColor;

	private static readonly Color
		LowColor = Color.black,
		HighColor = Color.white,
		ColdColor = Color.cyan,
		HotColor = Color.red,
		DryColor = Color.yellow,
		MoistColor = Color.blue;

	public bool IsWater => climate.isWater;

	public Tile(int x, int y, float height, float temp, float humidity) {
		this.x = x;
		this.y = y;
		this.height = height;
		this.temp = temp;
		this.humidity = humidity;
		climate = GameController.Climates.FirstOrDefault(climate => climate.CorrectTile(this));

		if (climate == null) {
			Debug.LogError($"Can't find matching climate for tile (height: {height:F3}, temp: {temp:F3}, humidity: {humidity:F3})");
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

	public Color GetColor(MapDrawMode mapDrawMode, float transparency) {
		return mapDrawMode == MapDrawMode.Normal
			? GetColor(MapDrawMode.Normal)
			: Color.Lerp(GetColor(MapDrawMode.Normal), GetColor(mapDrawMode), transparency);
	}

	private Color GetColor(MapDrawMode mapDrawMode) {
		switch (mapDrawMode) {
			case MapDrawMode.Normal:
				return customColor != Color.clear ? customColor : color;
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
}