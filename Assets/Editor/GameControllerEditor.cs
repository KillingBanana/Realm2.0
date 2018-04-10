using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {
	public override void OnInspectorGUI() {
		GameController gameController = (GameController) target;

		if (gameController == null) return;

		bool refreshed = DrawDefaultInspector();

		if (refreshed && gameController.generateOnEdit || GUILayout.Button("Generate World")) gameController.GenerateWorld();

		if (GameController.World != null && GUILayout.Button("Step")) gameController.UpdateWorld(true);
		if (GameController.World != null && !gameController.autoUpdateRunning && GUILayout.Button("Start")) gameController.StartAutoUpdate();
		if (GameController.World != null && gameController.autoUpdateRunning && GUILayout.Button("Stop")) gameController.StopAutoUpdate();
	}
}