using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {
	public override void OnInspectorGUI() {
		GameController gameController = (GameController) target;

		bool refreshed = DrawDefaultInspector();

		if (GUILayout.Button("Load Database") && EditorUtility.DisplayDialog("Load Database", "Are you sure you want to revert editor values to database?", "Yes", "Cancel")) gameController.LoadDatabase();

		if (GUILayout.Button("Save Database") && EditorUtility.DisplayDialog("Save Database", "Are you sure you want to overwrite database with editor values?", "Yes", "Cancel")) gameController.SaveDatabase();

		if (refreshed && gameController.autoUpdate || GUILayout.Button("Generate World")) gameController.GenerateWorld();
	}
}