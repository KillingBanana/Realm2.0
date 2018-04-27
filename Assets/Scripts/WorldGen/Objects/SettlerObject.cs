using UnityEngine;

[ExecuteInEditMode]
public class SettlerObject : DisplayObject<Settler> {
	protected override void UpdateDisplay() {
		if (!Target.Active) MapDisplay.SafeDestroy(gameObject);

		transform.position = WorldGenUtility.WorldToMeshPoint(Target.Tile.position);
	}
}