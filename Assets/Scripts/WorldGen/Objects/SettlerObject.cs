using UnityEngine;

public class SettlerObject : MonoBehaviour {
	public Settler settler;

	public void Init(Settler settler) {
		this.settler = settler;
		name = settler.ToString();
	}

	public void Update() {
		transform.position = WorldGenUtility.WorldToMeshPoint(settler.Tile.position);

		if (!settler.Active) Destroy(gameObject);
	}
}