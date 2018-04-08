using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {
	public override void OnInspectorGUI() {
		GameController gameController = (GameController) target;

		if (gameController == null) return;

		bool refreshed = DrawDefaultInspector();

		if (refreshed && gameController.autoUpdate || GUILayout.Button("Generate World")) gameController.GenerateWorld();
	}
}