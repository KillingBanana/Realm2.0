using UnityEngine;

public class WorldGenUtility : MonoBehaviour {
	[SerializeField] private AnimationCurve temperatureCurve;
	[SerializeField] private AnimationCurve heightCurve;
	[SerializeField] private float curveToDegreesRatio;
	[SerializeField] private float curveToMetersRatio;

	public int TemperatureToCelsius(float temp) => Mathf.RoundToInt(curveToDegreesRatio * temperatureCurve.Evaluate(temp));

	public int WorldHeightToMeters(float height) => Mathf.RoundToInt(curveToMetersRatio * heightCurve.Evaluate(height));

	public static Vector2Int MeshToWorldPoint(Vector3 pos) {
		int x = Mathf.FloorToInt(pos.x);
		int y = GameController.World.size - Mathf.CeilToInt(pos.z) - 1;

		return new Vector2Int(x, y);
	}

	public static Vector3 WorldToMeshPoint(Vector2Int pos) {
		float x = pos.x + .5f;
		float y = (GameController.MapDisplay.GetHeight(pos.x, pos.y) +
		           GameController.MapDisplay.GetHeight(pos.x + 1, pos.y) +
		           GameController.MapDisplay.GetHeight(pos.x, pos.y + 1) +
		           GameController.MapDisplay.GetHeight(pos.x + 1, pos.y + 1)) / 4;
		float z = GameController.World.size - pos.y - 1.5f;

		return new Vector3(x, y, z);
	}
}