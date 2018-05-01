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
	[SerializeField, Required] private Dropdown mapDrawModeDropdown, raceDropdown;

	private static World World => GameController.World;

	private MapDisplay mapDisplay;

	private void Awake() {
		mapDisplay = GetComponent<MapDisplay>();

		mapDrawModeDropdown.options = Enum.GetNames(typeof(MapDrawMode)).Select(drawModeName => new Dropdown.OptionData(drawModeName)).ToList();

		OnDrawModeChanged();

		raceDropdown.options = GameController.Races.Select(race => new Dropdown.OptionData(race.collectiveName.Capitalize())).ToList();

		OnRaceChanged();
	}

	[UsedImplicitly]
	public void OnDrawModeChanged() {
		MapDrawMode mapDrawMode = (MapDrawMode) mapDrawModeDropdown.value;

		raceDropdown.gameObject.SetActive(mapDrawMode == MapDrawMode.Race || mapDrawMode == MapDrawMode.Town);

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

		if (mapInfo != null) mapInfo.text = mapText;
	}

	private void Update() {
		if (GameController.Location != null || World == null || GameController.WorldCamera.dragged) return;

		RaycastHit hit;
		if (Physics.Raycast(GameController.WorldCamera.camera.ScreenPointToRay(Input.mousePosition), out hit)) {
			Vector2Int pos = WorldGenUtility.MeshToWorldPoint(hit.point);

			Tile tile = World.GetTile(pos);

			if (tile == null) return;

			string text = $"Position: {pos}" +
			              $"\nHeight: {GameController.WorldGenUtility.WorldHeightToMeters(tile.height)}m ({tile.height:F2})" +
			              $"\nTemp: {GameController.WorldGenUtility.TemperatureToCelsius(tile.temp)}°C ({tile.temp:F2})" +
			              $"\nRegion: {tile.region}";

			if (tile.location != null) text += $"\n{tile.location}";

			Town town = tile.Town;

			if (town != null) text += $"\nPopulation: {town.population}";

			tileInfo.text = text;


			if (Input.GetMouseButtonDown(0)) {
				Debug.Log($"Race Compatibility: {tile.GetRaceCompatibility(mapDisplay.race)}\nTown Compatibility: {tile.GetTownCompatibility(mapDisplay.race)}");
			}

			if (Input.GetMouseButtonDown(1)) {
				Vector3 point = hit.point;
				point.y = GameController.WorldCamera.targetPos.y;
				GameController.WorldCamera.targetPos = point;
			}
		}
	}
}