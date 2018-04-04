using JetBrains.Annotations;
using UnityEngine;

public class UIPanels : MonoBehaviour {
	[SerializeField] private RectTransform mainPanel;

	private RectTransform currentPanel;

	[UsedImplicitly]
	public void TogglePanel(RectTransform panel) {
		if (panel == currentPanel && mainPanel.gameObject.activeSelf) {
			Camera.main.rect = new Rect(0, 0, 1, 1);
			mainPanel.gameObject.SetActive(false);
		} else {
			Camera.main.rect = new Rect(0, 0, 0.69f, 1);
			mainPanel.gameObject.SetActive(true);
			currentPanel?.gameObject.SetActive(false);
			currentPanel = panel;
			currentPanel.gameObject.SetActive(true);
		}
	}
}