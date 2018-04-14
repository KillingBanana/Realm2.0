using System.Collections;
using System.Diagnostics;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class GameController : MonoBehaviour {
	private static GameController Instance => instance ?? (instance = FindObjectOfType<GameController>());

	private static GameController instance;

	private static readonly Stopwatch Stopwatch = new Stopwatch();

	private static int Seed => Instance.randomSeed ? Instance.seed = UnityEngine.Random.Range(0, 9999) : Instance.seed;
	[SerializeField] private bool randomSeed;
	[SerializeField] private int seed;

	[SerializeField] private bool screenshots;

	[Header("World Settings"), SerializeField]
	private bool startAutoUpdate;

	[SerializeField] private bool randomMapSeed;

	public bool generateOnEdit;

	[SerializeField, Range(0.01f, 1)] private float secondsPerStep;
	[HideInInspector] public bool autoUpdateRunning;

	[SerializeField] private WorldSettings worldSettings;

	public static World World { get; private set; }

	public static Location Location { get; private set; }

	public static Race[] Races {
		get {
			if (DatabaseManager.races == null || DatabaseManager.races.Length == 0) DatabaseManager.LoadDatabase();

			return DatabaseManager.races;
		}
	}

	public static Climate[] Climates {
		get {
			if (DatabaseManager.climates == null || DatabaseManager.climates.Length == 0) DatabaseManager.LoadDatabase();

			return DatabaseManager.climates;
		}
	}

	private static MapDisplay mapDisplay;
	public static MapDisplay MapDisplay => mapDisplay ?? (mapDisplay = Instance.GetComponent<MapDisplay>());

	private static WorldGenUI worldGenUI;
	public static WorldGenUI WorldGenUI => worldGenUI ?? (worldGenUI = Instance.GetComponent<WorldGenUI>());

	private static WorldGenUtility worldGenUtility;
	public static WorldGenUtility WorldGenUtility => worldGenUtility ?? (worldGenUtility = Instance.GetComponent<WorldGenUtility>());

	private static WorldCamera worldCamera;
	public static WorldCamera WorldCamera => worldCamera ?? (worldCamera = FindObjectOfType<WorldCamera>());

	private static DialogueManager dialogueManager;
	public static DialogueManager DialogueManager => dialogueManager ?? (dialogueManager = FindObjectOfType<DialogueManager>());

	private static DatabaseManager databaseManager;
	private static DatabaseManager DatabaseManager => databaseManager ?? (databaseManager = Instance.GetComponent<DatabaseManager>());

	private static Random random;
	public static Random Random => random ?? (random = new Random(Seed));

	private static AsyncOperation loadingLevel;

	private void Awake() {
		DontDestroyOnLoad(this);

		GenerateWorld();
	}

	[UsedImplicitly]
	public void OnWorldSizeChanged(int i) {
		worldSettings.worldSize = (WorldSize) i;
		GenerateWorld();
	}

	public void GenerateWorld() {
		StopAutoUpdate();

		random = new Random(Seed);
		if (randomMapSeed) worldSettings.seed = Random.Next(0, 999999);

		World = new World(worldSettings);
		World.Generate();

		if (startAutoUpdate && Application.isPlaying) StartAutoUpdate();

		OnWorldUpdated(true);
	}

	[UsedImplicitly]
	public void UpdateWorld(bool updateDisplay) {
		World.Update();
		if (updateDisplay) OnWorldUpdated(false);
	}

	private static void OnWorldUpdated(bool reset) {
		Stopwatch.Start();

		MapDisplay.DrawMap(reset);
		WorldGenUI.OnMapChanged();

		if (reset) WorldCamera.targetPos = new Vector3(World.size / 2, World.size / 2, World.size / 2);

		if (Instance.screenshots) {
			ScreenCapture.CaptureScreenshot("Screenshots/" + World.settings.seed + ".png");
		}

		Stopwatch.Stop();
		if (Instance.worldSettings.benchmark) Debug.Log($"Display time: {Stopwatch.ElapsedMilliseconds}ms");
		Stopwatch.Reset();
	}

	public void StartAutoUpdate() {
		StopAutoUpdate();
		StartCoroutine(nameof(AutoUpdate));
		autoUpdateRunning = true;
	}

	public void StopAutoUpdate() {
		StopCoroutine(nameof(AutoUpdate));
		autoUpdateRunning = false;
	}

	private IEnumerator AutoUpdate() {
		while (World != null) {
			yield return new WaitForSeconds(secondsPerStep);
			UpdateWorld(true);
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
}