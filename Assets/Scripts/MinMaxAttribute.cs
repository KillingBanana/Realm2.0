// NOTE DONT put in an editor folder

using UnityEngine;

public class MinMaxAttribute : PropertyAttribute {
	public float MinLimit = 0;
	public float MaxLimit = 1;
	public bool ShowEditRange = true;
	public bool ShowDebugValues = false;

	public MinMaxAttribute(float min, float max) {
		MinLimit = min;
		MaxLimit = max;
	}
}