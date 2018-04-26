using UnityEngine;

public class TownObject : MonoBehaviour {
	public Town town;

	public void Init(Town town) {
		this.town = town;
		name = town.ToString();
	}

	public void Update() {
		transform.localScale = Mathf.Sqrt((float) town.population / 4000) * Vector3.one;
	}
}