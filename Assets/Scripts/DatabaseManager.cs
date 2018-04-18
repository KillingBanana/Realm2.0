using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class DatabaseManager : MonoBehaviour {
	private static string RacesPath => string.Concat(Application.streamingAssetsPath, "/Database/Races/");
	private static string ClimatesPath => string.Concat(Application.streamingAssetsPath, "/Database/Climates/");

	[Header("Database")] public Race[] races;
	public Climate[] climates;

	[Button]
	public void LoadDatabase() {
		races = LoadFromDirectory<Race>(RacesPath);
		climates = LoadFromDirectory<Climate>(ClimatesPath);
	}

	[Button]
	public void SaveDatabase() {
		SaveToDirectory(races, RacesPath);
		SaveToDirectory(climates, ClimatesPath);
	}

	private static T[] LoadFromDirectory<T>(string path) {
		if (!Directory.Exists(path)) {
			Debug.LogError($"Directory {path} does not exist");
			return null;
		}

		return Directory.GetFiles(path, "*.json").Select(file => JsonUtility.FromJson<T>(File.ReadAllText(file))).ToArray();
	}

	private static void SaveToDirectory<T>(IEnumerable<T> database, string path) {
		//Make sure target directory exists
		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
			Debug.Log($"Created Directory {path}");
		}

		DirectoryInfo directory = new DirectoryInfo(path);

		//Create backup directory of needed
		string backupPath = path + "/Backup/";

		if (!Directory.Exists(backupPath)) Directory.CreateDirectory(backupPath);

		//Delete files in backup directory
		foreach (string file in Directory.EnumerateFiles(backupPath, "*.json")) {
			File.Delete(file);
			File.Delete(file + ".meta");
		}

		//Backup files
		foreach (FileInfo fileInfo in directory.GetFiles("*.json")) {
			string targetPath = backupPath + fileInfo.Name;

			if (File.Exists(targetPath)) {
				File.Delete(targetPath);
				File.Delete(targetPath + ".meta");
			}

			fileInfo.MoveTo(targetPath);
		}

		//Delete files in save directory
		foreach (string file in Directory.EnumerateFiles(path, "*.json")) {
			File.Delete(file);
			File.Delete(file + ".meta");
		}

		//Save objects to files
		foreach (T o in database) {
			File.WriteAllText($"{path}{o}.json", JsonUtility.ToJson(o, true));
		}
	}
}