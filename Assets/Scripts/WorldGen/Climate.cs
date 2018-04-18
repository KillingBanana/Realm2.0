using System;
using Sirenix.OdinInspector;
using UnityEngine;

#pragma warning disable 0649

[Serializable]
public class Climate {
	public string name;

	[SerializeField] private Gradient colorGradient;

	[MinMaxSlider(0f, 1f, true)] public Vector2 height, temp, humidity;

	public bool isWater;

	public Color GetColor(float tileHeight) {
		Color color = colorGradient.Evaluate(Mathf.InverseLerp(height.x, height.y, tileHeight));
		return color;
	}

	public bool CorrectTile(Tile tile) => height.Contains(tile.height) && temp.Contains(tile.temp) && humidity.Contains(tile.humidity);

	public override string ToString() => name;
}