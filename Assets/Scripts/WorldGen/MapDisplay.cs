using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

public class MapDisplay : MonoBehaviour {
	[SerializeField, Required] private Transform townsParent, settlersParent, roadsParent;
	[SerializeField, Required] private MeshFilter meshFilter;
	[SerializeField, Required] private MeshRenderer meshRenderer;
	[SerializeField, Required] private MeshCollider meshCollider;
	[SerializeField, Required] private Slider transparencySlider;
	[SerializeField] private AnimationCurve heightCurve;
	[SerializeField] private float heightMultiplier;

	[HideInInspector] public MapDrawMode drawMode;
	[HideInInspector] public Race race;

	private float HeightMultiplier => Mathf.Sqrt(World.size) * heightMultiplier;

	private static World World => GameController.World;

	private readonly Dictionary<Town, TownObject> townObjects = new Dictionary<Town, TownObject>();
	private readonly Dictionary<Settler, SettlerObject> settlerObjects = new Dictionary<Settler, SettlerObject>();
	private readonly Dictionary<Road, RoadObject> roadObjects = new Dictionary<Road, RoadObject>();

	public void DrawMap(bool reset) {
		if (reset) {
			Mesh mapMesh = MeshGenerator.GenerateTerrainMesh(World.heightMap, World.settings.Lod, World.size, GetHeight);
			meshFilter.sharedMesh = mapMesh;
			meshCollider.sharedMesh = mapMesh;
			meshFilter.transform.position = new Vector3(World.size / 2, 0, World.size / 2);

			DrawTexture();
		}

		DisplayObjects(reset);
	}

	public float GetHeight(int x, int y) {
		Tile tile = GameController.World.GetTile(x, y);
		return tile == null ? 0 : heightCurve.Evaluate(tile.height) * HeightMultiplier;
	}

	public void DrawTexture() {
		Texture2D mapTexture = GetTexture(meshRenderer.sharedMaterial.mainTexture as Texture2D);
		meshRenderer.sharedMaterial.mainTexture = mapTexture;
	}

	private void DisplayObjects(bool reset) {
		if (reset) DestroyChildren();

		DisplayTowns(reset);
		DisplaySettlers(reset);
		DisplayRoads(reset);
	}

	private void DisplayTowns(bool reset) {
		if (reset) townObjects.Clear();

		foreach (Town town in GameController.World.towns) {
			if (!townObjects.ContainsKey(town)) {
				TownObject townObject = InstantiateOnMap(PrefabManager.Town, town.Tile.position, townsParent);
				townObject.Init(town);

				townObjects.Add(town, townObject);
			}
		}
	}

	private void DisplaySettlers(bool reset) {
		if (reset) settlerObjects.Clear();

		foreach (Town town in GameController.World.towns) {
			foreach (Settler settler in town.settlers) {
				if (!settlerObjects.ContainsKey(settler)) {
					SettlerObject settlerObject = InstantiateOnMap(PrefabManager.Settler, settler.tile.position, settlersParent);
					settlerObject.Init(settler);

					settlerObjects.Add(settler, settlerObject);
				}
			}
		}
	}

	private void DisplayRoads(bool reset) {
		if (reset) roadObjects.Clear();

		foreach (Town town in GameController.World.towns) {
			foreach (Road road in town.roads) {
				if (!roadObjects.ContainsKey(road)) {
					RoadObject roadObject = InstantiateOnMap(PrefabManager.Road, town.Tile.position, roadsParent);
					roadObject.Init(road);

					roadObjects.Add(road, roadObject);
				}
			}
		}
	}

	private void DestroyChildren() {
		Transform[] children = townsParent.Cast<Transform>().Concat(settlersParent.Cast<Transform>()).Concat(roadsParent.Cast<Transform>()).ToArray();

		foreach (Transform child in children) {
			SafeDestroy(child.gameObject);
		}
	}

	public static void SafeDestroy(GameObject o) {
		if (Application.isPlaying) {
			Destroy(o);
		} else {
			DestroyImmediate(o);
		}
	}

	private static T InstantiateOnMap<T>(T t, Vector2Int position, Transform parent) where T : Object {
		T instance = Instantiate(t, WorldGenUtility.WorldToMeshPoint(position), Quaternion.identity, parent);
		return instance;
	}

	private Texture2D GetTexture(Texture2D texture) {
		Color[] colors = new Color[World.squareSize];


		for (int x = 0; x < World.size; x++) {
			for (int y = 0; y < World.size; y++) {
				colors[x + World.size * y] = World.GetTile(x, y).GetColor(drawMode, transparencySlider.value, race);
			}
		}

		if (texture == null || texture.height != World.size || texture.width != World.size) {
			texture = new Texture2D(World.size, World.size) {filterMode = FilterMode.Point};
		}

		texture.SetPixels(colors);
		texture.Apply();

		return texture;
	}
}