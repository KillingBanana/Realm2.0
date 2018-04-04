using UnityEngine;

public class WorldGenUtility : MonoBehaviour {
	[SerializeField] private AnimationCurve temperatureCurve;
	[SerializeField] private AnimationCurve heightCurve;
	[SerializeField] private float curveToDegreesRatio;
	[SerializeField] private float curveToMetersRatio;

	public int TemperatureToCelsius(float temp) => Mathf.RoundToInt(curveToDegreesRatio * temperatureCurve.Evaluate(temp));

	public int WorldHeightToMeters(float height) => Mathf.RoundToInt(curveToMetersRatio * heightCurve.Evaluate(height));
}