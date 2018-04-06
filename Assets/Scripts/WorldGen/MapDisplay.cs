using UnityEngine;

public class MapDisplay : MonoBehaviour {

	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private MeshCollider meshCollider;
	[SerializeField] private AnimationCurve heightCurve;
	[SerializeField] private float heightMultiplier;

	public static Texture2D mapTexture;

	private static World World => GameController.World;

	public void DrawMap() {
		Mesh mapMesh = MeshGenerator.GenerateTerrainMesh(World.HeightMap, World.settings.Lod, World.size, Mathf.Sqrt(World.size) * heightMultiplier, heightCurve);
		meshFilter.sharedMesh = mapMesh;
		meshCollider.sharedMesh = mapMesh;
		meshFilter.transform.position = new Vector3(World.size / 2, 0, World.size / 2);
		DrawTexture();
	}

	public void DrawTexture() {
		mapTexture = GameController.World.GetTexture(WorldGenUI.DrawMode);
		meshRenderer.sharedMaterial.mainTexture = mapTexture;
	}
}