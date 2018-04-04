using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameController : MonoBehaviour {
	private static GameController Instance => instance ?? (instance = FindObjectOfType<GameController>());

	private static GameController instance;

	private static int Seed => Instance.randomSeed ? Instance.seed = UnityEngine.Random.Range(0, 9999) : Instance.seed;
	[SerializeField] private bool randomSeed;
	[SerializeField] private int seed;

	[SerializeField] private bool screenshots;

	[Header("Map Settings"), SerializeField]
	private bool randomMapSeed;

	[SerializeField] public bool autoUpdate;

	[SerializeField] private MapSettings mapSettings;

	public static Map Map { get; private set; }

	public static Location Location { get; private set; }

	public static Race[] Races {
		get {
			if (Instance.races == null || Instance.races.Length == 0) Instance.LoadDatabase();

			return Instance.races;
		}
	}

	public static Climate[] Climates {
		get {
			if (Instance.climates == null || Instance.climates.Length == 0) Instance.LoadDatabase();

			return Instance.climates;
		}
	}

	[Header("Database"), SerializeField] private Race[] races;
	[SerializeField] private Climate[] climates;

	private static string RacesPath => Application.streamingAssetsPath + "/Database/Races/";
	private static string ClimatesPath => Application.streamingAssetsPath + "/Database/Climates/";

	public static MapDisplay MapDisplay => mapDisplay ?? (mapDisplay = Instance.GetComponent<MapDisplay>());
	public static WorldGenUI WorldGenUI => worldGenUI ?? (worldGenUI = Instance.GetComponent<WorldGenUI>());

	public static WorldCamera WorldCamera => worldCamera ?? (worldCamera = FindObjectOfType<WorldCamera>());
	public static DialogueManager DialogueManager => dialogueManager ?? (dialogueManager = FindObjectOfType<DialogueManager>());

	private static MapDisplay mapDisplay;
	private static WorldGenUI worldGenUI;

	private static WorldCamera worldCamera;
	private static DialogueManager dialogueManager;

	private static AsyncOperation loadingLevel;

	public static Random Random => random ?? (random = new Random(Seed));
	private static Random random;

	private void Awake() {
		DontDestroyOnLoad(this);

		GenerateMap();
	}

	public void OnMapSizeChanged(int i) {
		mapSettings.mapSize = (MapSize) i;
		GenerateMap();
	}

	public void GenerateMap() {
		if (randomMapSeed) mapSettings.seed = Random.Next(0, 99999);
		Map = new Map(mapSettings);
		OnMapUpdated();
	}

	private static void OnMapUpdated() {
		MapDisplay.DrawMap();
		WorldGenUI.OnMapChanged();
		WorldCamera.targetPos = new Vector3(Map.size / 2, Map.size / 2, Map.size / 2);
		if (Instance.screenshots) {
			ScreenCapture.CaptureScreenshot("Screenshots/" + Map.settings.seed + ".png");
		}
	}

	public static IEnumerator LoadLocation(Location location) {
		if (loadingLevel != null && !loadingLevel.isDone) yield break;
		Location = location;
		loadingLevel = SceneManager.LoadSceneAsync("Local", LoadSceneMode.Single);

		while (!loadingLevel.isDone) {
			yield return null;
		}
	}

	public void LoadDatabase() {
		races = LoadFromDirectory<Race>(RacesPath);
		climates = LoadFromDirectory<Climate>(ClimatesPath);
	}

	public void SaveDatabase() {
		SaveToDirectory(races, RacesPath);
		SaveToDirectory(climates, ClimatesPath);
	}

	private static T[] LoadFromDirectory<T>(string path) => Directory.GetFiles(path, "*.json").Select(file => JsonUtility.FromJson<T>(File.ReadAllText(file))).ToArray();

	private static void SaveToDirectory<T>(IEnumerable<T> database, string path) {
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		foreach (T o in database) {
			File.WriteAllText(path + o + ".json", JsonUtility.ToJson(o, true));
		}
	}

	public static Race RandomRace() => Races.RandomItem();
}