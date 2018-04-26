using System.Linq;
using UnityEngine;

public class RoadObject : MonoBehaviour {
	[SerializeField] private LineRenderer lineRenderer;
	private Road road;

	public void Init(Road road) {
		this.road = road;
		name = road.ToString();
	}

	public void Update() {
		lineRenderer.positionCount = road.Tiles.Count;
		lineRenderer.SetPositions(road.Tiles.Select(tile => RoadPosition(tile.position)).ToArray());

		lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, Mathf.Sqrt((float) road.Population / 4000)));
	}

	private static Vector3 RoadPosition(Vector2Int position) => WorldGenUtility.WorldToMeshPoint(position) + Vector3.up * 0.2f;
}