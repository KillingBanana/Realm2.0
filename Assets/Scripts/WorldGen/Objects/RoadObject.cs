using System.Linq;
using UnityEngine;

public class RoadObject : DisplayObject<Road> {
	[SerializeField] private LineRenderer lineRenderer;

	protected override void UpdateDisplay() {
		lineRenderer.positionCount = Target.Tiles.Count;
		lineRenderer.SetPositions(Target.Tiles.Select(tile => RoadPosition(tile.position)).ToArray());

		lineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, Mathf.Sqrt((float) Target.Population / 4000)));

		lineRenderer.material.color = Target.town.faction.color;
	}

	private static Vector3 RoadPosition(Vector2Int position) => WorldGenUtility.WorldToMeshPoint(position) + Vector3.up * 0.2f;
}