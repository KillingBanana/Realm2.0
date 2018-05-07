using Sirenix.OdinInspector;
using UnityEngine;

public class PrefabManager : MonoBehaviour {
	private static PrefabManager instance;
	private static PrefabManager Instance => instance ?? (instance = FindObjectOfType<PrefabManager>());

	[SerializeField, AssetsOnly] private TownObject town;
	public static TownObject Town => Instance.town;

	[SerializeField, AssetsOnly] private SettlerObject settler;
	public static SettlerObject Settler => Instance.settler;

	[SerializeField, AssetsOnly] private RoadObject road;
	public static RoadObject Road => Instance.road;
}