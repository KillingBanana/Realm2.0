using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {
	private GameController gameController;

	public override void OnInspectorGUI() {
		gameController = (GameController) target;

		bool refreshed = DrawDefaultInspector();

		if (GUILayout.Button("Load Database") && EditorUtility.DisplayDialog("Load Database", "Are you sure you want to revert editor values to database?", "Yes", "No")) gameController.LoadDatabase();

		if (GUILayout.Button("Save Database") && EditorUtility.DisplayDialog("Save Database", "Are you sure you want to overwrite database with editor values?", "Yes", "No")) gameController.SaveDatabase();

		if (refreshed && gameController.autoUpdate || GUILayout.Button("Generate World")) {
			gameController.GenerateMap();
		}
	}
}