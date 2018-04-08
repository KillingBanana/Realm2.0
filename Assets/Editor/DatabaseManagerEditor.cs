using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DatabaseManager))]
public class DatabaseManagerEditor : Editor {
	public override void OnInspectorGUI() {
		DatabaseManager databaseManager = (DatabaseManager) target;

		DrawDefaultInspector();

		if (databaseManager == null) return;

		if (GUILayout.Button("Load Database") && EditorUtility.DisplayDialog("Load Database", "Are you sure you want to revert editor values to database?", "Yes", "Cancel"))
			databaseManager.LoadDatabase();

		if (GUILayout.Button("Save Database") && EditorUtility.DisplayDialog("Save Database", "Are you sure you want to overwrite database with editor values?", "Yes", "Cancel"))
			databaseManager.SaveDatabase();
	}
}