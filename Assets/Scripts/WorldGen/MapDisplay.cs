using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour {
	[SerializeField] private Transform parent;
	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private MeshCollider meshCollider;
	[SerializeField] private AnimationCurve heightCurve;
	[SerializeField] private Slider transparencySlider;
	[SerializeField] private float heightMultiplier;

	private float HeightMultiplier => Mathf.Sqrt(World.size) * heightMultiplier;

	private static World World => GameController.World;

	private readonly Dictionary<Town, GameObject> townObjects = new Dictionary<Town, GameObject>();

	public void DrawMap(bool reset) {
		if (reset) {
			Mesh mapMesh = MeshGenerator.GenerateTerrainMesh(World.heightMap, World.settings.Lod, World.size, GetHeight);
			meshFilter.sharedMesh = mapMesh;
			meshCollider.sharedMesh = mapMesh;
			meshFilter.transform.position = new Vector3(World.size / 2, 0, World.size / 2);
		}

		DrawTexture();
		DisplayObjects(reset);
	}

	public float GetHeight(int x, int y) => heightCurve.Evaluate(GameController.World.GetTile(x, y).height) * HeightMultiplier;

	public void DrawTexture() {
		Texture2D mapTexture = GetTexture(WorldGenUI.drawMode, transparencySlider.value);
		meshRenderer.sharedMaterial.mainTexture = mapTexture;
	}

	private void DisplayObjects(bool reset) {
		if (reset) {
			List<Transform> transforms = parent.Cast<Transform>().ToList();

			foreach (Transform transform1 in transforms) {
				Debug.Log(transform1.name);
				Destroy(transform1.gameObject);
			}
		}

		DisplayTowns(reset);
	}

	private static void Destroy(GameObject o) {
		if (Application.isPlaying) {
			Object.Destroy(o);
		} else {
			DestroyImmediate(o);
		}
	}

	private void DisplayTowns(bool reset) {
		if (reset) townObjects.Clear();

		foreach (Town town in GameController.World.towns) {
			GameObject o;
			if (townObjects.ContainsKey(town)) {
				o = townObjects[town];
			} else {
				o = InstantiateOnMap(PrefabManager.Cube, town.tile.position);
				o.name = town.Name;
				townObjects.Add(town, o);
			}

			o.transform.localScale = Mathf.Sqrt((float) town.population / 4000) * Vector3.one;
		}
	}

	private T InstantiateOnMap<T>(T t, Vector2Int position) where T : Object {
		T instance = Instantiate(t, WorldGenUtility.WorldToMeshPoint(position), Quaternion.identity, parent);
		return instance;
	}

	private static Texture2D GetTexture(MapDrawMode mapDrawMode, float transparency) {
		Color[] colors = new Color[World.size * World.size];

		Texture2D texture = new Texture2D(World.size, World.size) {filterMode = FilterMode.Point};

		for (int x = 0; x < World.size; x++) {
			for (int y = 0; y < World.size; y++) {
				colors[x + World.size * y] = World.GetTile(x, y).GetColor(mapDrawMode, transparency);
			}
		}

		texture.SetPixels(colors);
		texture.Apply();
		return texture;
	}
}