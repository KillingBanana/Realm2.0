using System;
using System.Security.Cryptography;
using Sirenix.OdinInspector;
using UnityEngine;

#pragma warning disable 0649

[Serializable]
public class Race {
	public string collectiveName, individualName, adjective;

	[Range(0, 1), FoldoutGroup("Stats")] public float expansionism, hostility;

	[MinMaxSlider(0f, 1f, true), FoldoutGroup("Range")]
	public Vector2 heightRange = new Vector2(0, 1), tempRange = new Vector2(0, 1), humidityRange = new Vector2(0, 1);

	[Range(0, 1), FoldoutGroup("Preferred")]
	public float heightPreferred = .5f, tempPreferred = .5f, humidityPreferred = .5f;

	[SerializeField, Range(0, 1), FoldoutGroup("Weight")]
	private float heightWeight = .33f, tempWeight = .33f, humidityWeight = .33f;

	private float TotalWeight => heightWeight + tempWeight + humidityWeight;
	public float HeightWeight => heightWeight / TotalWeight;
	public float TempWeight => tempWeight / TotalWeight;
	public float HumidityWeight => humidityWeight / TotalWeight;

	[SerializeField, FoldoutGroup("Names")]
	private string[] placeNames, maleFirstNames, femaleFirstNames, lastNames;

	[SerializeField, FoldoutGroup("Names")]
	private Vector2Int placeNameLength = new Vector2Int(1, 2), firstNameLength = new Vector2Int(1, 2), lastNameLength = new Vector2Int(1, 3);

	public bool IsValidTile(Tile tile) => !tile.IsWater && heightRange.Contains(tile.height) && tempRange.Contains(tile.temp) && humidityRange.Contains(tile.humidity);

	public string GetPlaceName() {
		string placeName = "";
		int length = placeNameLength.Random();
		for (int i = 0; i < length; i++) {
			placeName += placeNames[GameController.Random.Next(0, placeNames.Length)];
		}

		return placeName.Capitalize();
	}

	public string GetFirstName(bool isFemale) {
		string firstName = "";
		int length = firstNameLength.Random();
		string[] names = isFemale ? femaleFirstNames : maleFirstNames;
		for (int i = 0; i < length; i++) {
			firstName += names.RandomItem();
		}

		return firstName.Capitalize();
	}

	public string GetLastName() {
		string lastName = "";
		int length = lastNameLength.Random();
		for (int i = 0; i < length; i++) {
			lastName += lastNames.RandomItem();
		}

		return lastName.Capitalize();
	}

	public override string ToString() => collectiveName.Capitalize();
}