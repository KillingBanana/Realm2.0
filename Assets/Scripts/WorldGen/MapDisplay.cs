using UnityEngine;

public class MapDisplay : MonoBehaviour {

	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private MeshCollider meshCollider;
	[SerializeField] private AnimationCurve heightCurve;
	[SerializeField] private float heightMultiplier;

	public static Texture2D mapTexture;

	private static Map Map => GameController.Map;

	public void DrawMap() {
		Mesh mapMesh = MeshGenerator.GenerateTerrainMesh(Map.HeightMap, Map.settings.Lod, Map.size, Mathf.Sqrt(Map.size) * heightMultiplier, heightCurve);
		meshFilter.sharedMesh = mapMesh;
		meshCollider.sharedMesh = mapMesh;
		meshFilter.transform.position = new Vector3(Map.size / 2, 0, Map.size / 2);
		DrawTexture();
	}

	public void DrawTexture() {
		mapTexture = GameController.Map.GetTexture(WorldGenUI.DrawMode);
		meshRenderer.sharedMaterial.mainTexture = mapTexture;
	}
}