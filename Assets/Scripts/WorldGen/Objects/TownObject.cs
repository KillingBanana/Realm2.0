using UnityEngine;

public class TownObject : DisplayObject<Town> {
	protected override void UpdateDisplay() {
		transform.localScale = Mathf.Sqrt((float) Target.population / 4000) * Vector3.one;
	}
}