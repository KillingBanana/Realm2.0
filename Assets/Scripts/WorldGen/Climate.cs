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

	private bool CorrectTile(float tileHeight, float tileTemp, float tileHumidity) => height.Contains(tileHeight) && temp.Contains(tileTemp) && humidity.Contains(tileHumidity);

	public static Climate GetClimate(float height, float temp, float humidity) {
		foreach (Climate climate in GameController.Climates) {
			if (climate.CorrectTile(height, temp, humidity)) return climate;
		}

		return null;
	}

	public override string ToString() => name;
}