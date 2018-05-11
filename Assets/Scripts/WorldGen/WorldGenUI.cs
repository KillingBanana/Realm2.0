using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

public class WorldGenUI : MonoBehaviour {
	[SerializeField, Required] private Text tileInfo, mapInfo;
	[SerializeField, Required] private Dropdown mapSizeDropdown, mapDrawModeDropdown, raceDropdown;

	private static World World => GameController.World;

	private MapDisplay mapDisplay;

	private void Awake() {
		mapDisplay = GetComponent<MapDisplay>();

		mapSizeDropdown.value = (int) GameController.WorldSettings.worldSize;

		mapDrawModeDropdown.FromList(Enum.GetNames(typeof(MapDrawMode)));

		OnDrawModeChanged();

		raceDropdown.FromList(GameController.Races.Select(race => race.collectiveName.Capitalize()).ToArray());

		OnRaceChanged();
	}

	[UsedImplicitly]
	public void OnDrawModeChanged() {
		MapDrawMode mapDrawMode = (MapDrawMode) mapDrawModeDropdown.value;

		mapDisplay.drawMode = mapDrawMode;

		mapDisplay.DrawTexture();
	}

	[UsedImplicitly]
	public void OnTransparencyChanged(float f) {
		mapDisplay.DrawTexture();
	}

	public void OnRaceChanged() {
		Race race = GameController.Races[raceDropdown.value];

		mapDisplay.race = race;

		mapDisplay.DrawTexture();
	}

	public void OnMapChanged() {
		string mapText = $"Seed: {World.settings.seed}\nPopulation: {World.towns.Sum(t => t.population)}\nDay: {World.Days}";

		foreach (Climate climate in GameController.Climates) {
			List<Region> validRegions = World.regions.Where(region => region.climate == climate).ToList();
			int regionsCount = validRegions.Count;
			if (regionsCount == 0) continue;
			int tilesCount = validRegions.Sum(region => region.Size);

			mapText += $"\n{regionsCount} {climate.name}s ({tilesCount} tiles)";
		}

		mapInfo.text = mapText;
	}

	private void Update() {
		RaycastHit hit;
		if (Physics.Raycast(GameController.WorldCamera.camera.ScreenPointToRay(Input.mousePosition), out hit)) {
			Vector2Int pos = WorldGenUtility.MeshToWorldPoint(hit.point);

			Tile tile = World.GetTile(pos);

			if (tile == null) return;

			float height = GameController.WorldGenUtility.WorldHeightToMeters(tile.height);
			float temp = GameController.WorldGenUtility.TemperatureToCelsius(tile.temp);

			string text = $"Position: {tile.position}\nHeight: {height}m ({tile.height:F3})\nTemp: {temp}°C ({tile.temp:F3})\nRegion: {tile.region}";

			if (tile.location != null) text += $"\n{tile.location}";

			Town town = tile.Town;

			if (town != null) {
				text += $"\nPopulation: {town.population}";
			}

			tileInfo.text = text;

			if (Input.GetMouseButtonDown(1)) {
				GameController.WorldCamera.target = hit.point;
			}
		}
	}
}