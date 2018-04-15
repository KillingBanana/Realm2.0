using System.Linq;
using UnityEngine;

public class SettlerObject : MonoBehaviour {
	public Settler settler;
	[SerializeField] private LineRenderer lineRenderer;
	[SerializeField] private MeshRenderer meshRenderer;

	public void Init(Settler settler) {
		this.settler = settler;
		name = settler.ToString();

		UpdatePosition();
	}

	public void UpdatePosition() {
		transform.position = WorldGenUtility.WorldToMeshPoint(settler.Tile.position);

		if (lineRenderer != null) {
			lineRenderer.positionCount = settler.tiles.Count;
			lineRenderer.SetPositions(settler.tiles.Select(tile => RoadPosition(tile.position)).ToArray());
		}

		if (!settler.active && meshRenderer != null) meshRenderer.enabled = false;
	}

	private static Vector3 RoadPosition(Vector2Int position) => WorldGenUtility.WorldToMeshPoint(position) + Vector3.up * 0.2f;
}