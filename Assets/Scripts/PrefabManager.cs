using UnityEngine;

public class PrefabManager : MonoBehaviour {
	private static PrefabManager instance;
	private static PrefabManager Instance => instance ?? (instance = FindObjectOfType<PrefabManager>());

	[SerializeField] private GameObject cube;
	public static GameObject Cube => Instance.cube;
}