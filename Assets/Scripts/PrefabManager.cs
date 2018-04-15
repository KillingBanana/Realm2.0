using UnityEngine;

public class PrefabManager : MonoBehaviour {
	private static PrefabManager instance;
	private static PrefabManager Instance => instance ?? (instance = FindObjectOfType<PrefabManager>());

	[SerializeField] private GameObject town;
	public static GameObject Town => Instance.town;

	[SerializeField] private SettlerObject settler;
	public static SettlerObject Settler => Instance.settler;
}