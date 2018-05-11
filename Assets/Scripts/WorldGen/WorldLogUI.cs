using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class WorldLogUI : MonoBehaviour {
	[SerializeField, Required] private GameObject panel;
	[SerializeField, Required] private Text worldLogText;
	[SerializeField, Required] private Dropdown importanceDropdown;

	private void Awake() {
		importanceDropdown.FromList(Enum.GetNames(typeof(WorldEventImportance)));
	}

	public void UpdateLog() {
		if (panel.activeInHierarchy) {
			WorldEventImportance importance = (WorldEventImportance) importanceDropdown.value;
			worldLogText.text = GameController.World.log.GetLog(importance);
		}
	}

	public void Toggle() {
		panel.SetActive(!panel.activeSelf);
		UpdateLog();
	}
}