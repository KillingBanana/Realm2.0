using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour {
	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private MeshCollider meshCollider;
	[SerializeField] private AnimationCurve heightCurve;
	[SerializeField] private Slider transparencySlider;
	[SerializeField] private float heightMultiplier;

	private static World World => GameController.World;

	public void DrawMap() {
		Mesh mapMesh = MeshGenerator.GenerateTerrainMesh(World.HeightMap(), World.settings.Lod, World.size, Mathf.Sqrt(World.size) * heightMultiplier, heightCurve);
		meshFilter.sharedMesh = mapMesh;
		meshCollider.sharedMesh = mapMesh;
		meshFilter.transform.position = new Vector3(World.size / 2, 0, World.size / 2);
		DrawTexture();
	}

	public void DrawTexture() {
		Texture2D mapTexture = GetTexture(WorldGenUI.drawMode, transparencySlider.value);
		meshRenderer.sharedMaterial.mainTexture = mapTexture;
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