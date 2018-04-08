using System;
using UnityEngine;

#pragma warning disable 0649

[Serializable]
public class Climate {
	public string name;

	[SerializeField, MinMax(0, 1)] private Vector2 height, temp, humidity;

	public bool isWater;

	[SerializeField] private Gradient colorGradient;

	public Color GetColor(float tileHeight) {
		Color color = colorGradient.Evaluate(Mathf.InverseLerp(height.x, height.y, tileHeight));
		return color;
	}

	public bool CorrectTile(Tile tile) => height.Contains(tile.height) && temp.Contains(tile.temp) && humidity.Contains(tile.humidity);

	public override string ToString() => name;
}