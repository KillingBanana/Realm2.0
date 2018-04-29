using Sirenix.OdinInspector;
using UnityEngine;

public class WorldCamera : MonoBehaviour {
	[SerializeField] private float zoomSensitivity = 10;

	private const int MouseButtonPan = 2;

	[SerializeField] private float panSensitivity = 1f;

	private Vector3 initialMousePosition;
	private Vector3 initialPosition;

	public Vector3 targetPos;

	[HideInInspector] public bool dragged;

	[Required] public new Camera camera;

	private void Update() {
		if (GameController.World == null) return;

		float mouseWheel = -Input.GetAxis("Mouse ScrollWheel");

		targetPos.y = Mathf.Clamp(targetPos.y + mouseWheel * zoomSensitivity, (float) GameController.World.size / 10, (float) GameController.World.size / 2);

		if (Input.GetMouseButtonDown(MouseButtonPan)) {
			initialMousePosition = Input.mousePosition;
			initialPosition = transform.position;
		}

		if (dragged = Input.GetMouseButton(MouseButtonPan)) {
			Vector3 mousePosDiff = initialMousePosition - Input.mousePosition;
			Vector3 cameraPosDiff = panSensitivity * (targetPos.y / 100) * new Vector3(mousePosDiff.x, 0, mousePosDiff.y);
			targetPos.x = initialPosition.x + cameraPosDiff.x;
			targetPos.z = initialPosition.z + cameraPosDiff.z;
		}

		targetPos.x = Mathf.Clamp(targetPos.x, 0, GameController.World.size);
		targetPos.z = Mathf.Clamp(targetPos.z, 0, GameController.World.size);

		transform.position = (transform.position - targetPos).magnitude > 0.01f
			? Vector3.Lerp(transform.position, targetPos, 0.1f)
			: targetPos;
	}
}