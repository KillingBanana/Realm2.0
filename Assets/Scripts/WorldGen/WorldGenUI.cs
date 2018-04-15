using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

public class WorldGenUI : MonoBehaviour {
	[SerializeField] private Text tileInfo, mapInfo;

	private static World World => GameController.World;

	private MapDisplay mapDisplay;

	private Tile tile;

	public static MapDrawMode drawMode;
	private int mapDrawModesCount;

	private new Camera camera;

	private void Awake() {
		camera = Camera.main;
		mapDisplay = GetComponent<MapDisplay>();
		mapDrawModesCount = Enum.GetValues(typeof(MapDrawMode)).Length;
	}

	[UsedImplicitly]
	public void OnDrawModeChanged(int value) {
		drawMode = value < mapDrawModesCount ? (MapDrawMode) value : 0;
		mapDisplay.DrawTexture();
	}

	[UsedImplicitly]
	public void OnTransparencyChanged(float f) {
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
		if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit)) {
			Vector2Int pos = WorldGenUtility.MeshToWorldPoint(hit.point);

			Tile newTile = World.GetTile(pos);

			if (newTile == null) return;

			if (newTile != tile) {
				tile = newTile;

				string text = $"Position: {pos}" +
				              $"\nHeight: {GameController.WorldGenUtility.WorldHeightToMeters(tile.height)}m ({tile.height:F2})" +
				              $"\nTemp: {GameController.WorldGenUtility.TemperatureToCelsius(tile.temp)}°C ({tile.temp:F2})" +
				              $"\nRegion: {tile.region}";

				if (tile.location != null) text += $"\n{tile.location}";
				Town town = tile.location as Town;
				if (town != null) text += $"\nPopulation: {town.population}";

				tileInfo.text = text;
			}

			if (Input.GetMouseButtonDown(0) && tile.location != null) {
				//StartCoroutine(GameController.LoadLocation(tile.location));
			}

			if (Input.GetMouseButtonDown(1)) {
				Vector3 point = hit.point;
				point.y = GameController.WorldCamera.targetPos.y;
				GameController.WorldCamera.targetPos = point;
			}
		}
	}
}