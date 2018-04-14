using System;
using UnityEngine;

#pragma warning disable 0649

[Serializable]
public class Race {
	public string collectiveName, individualName, adjective;

	[Range(0, 1)] public float expansionism, hostility;

	[MinMax(0, 1)] public Vector2 height, temp, humidity;
	[SerializeField, Range(0, 1)] private float heightWeight, tempWeight, humidityWeight;
	private float TotalWeight => heightWeight + tempWeight + humidityWeight;
	public float HeightWeight => heightWeight / TotalWeight;
	public float TempWeight => tempWeight / TotalWeight;
	public float HumidityWeight => humidityWeight / TotalWeight;

	[SerializeField] private string[] placeNames, maleFirstNames, femaleFirstNames, lastNames;
	[SerializeField] private IntRange placeNameLength, firstNameLength, lastNameLength;

	public bool IsValidTile(Tile tile) => !tile.IsWater && height.Contains(tile.height) && temp.Contains(tile.temp) && humidity.Contains(tile.humidity);

	public string GetPlaceName() {
		string placeName = "";
		int length = placeNameLength.Random;
		for (int i = 0; i < length; i++) {
			placeName += placeNames[GameController.Random.Next(0, placeNames.Length)];
		}

		return placeName.Capitalize();
	}

	public string GetFirstName(bool isFemale) {
		string firstName = "";
		int length = firstNameLength.Random;
		string[] names = isFemale ? femaleFirstNames : maleFirstNames;
		for (int i = 0; i < length; i++) {
			firstName += names[GameController.Random.Next(0, names.Length)];
		}

		return firstName.Capitalize();
	}

	public string GetLastName() {
		string lastName = "";
		int length = lastNameLength.Random;
		for (int i = 0; i < length; i++) {
			lastName += lastNames[GameController.Random.Next(0, lastNames.Length)];
		}

		return lastName.Capitalize();
	}

	public override string ToString() => collectiveName.Capitalize();
}