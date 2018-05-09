using Sirenix.OdinInspector;
using UnityEngine;

public class WorldCamera : MonoBehaviour {
	public bool clamp;
	[SerializeField] private float zoomSensitivity = 10, smoothing = 0.1f, panSensitivity = 1f, rotateSensitivity = 1f;

	private float zoom;

	private const int MouseButtonPan = 2, MouseButtonRotate = 0;

	[ReadOnly] public Vector3 target;
	private Vector3 Position => target - transform.forward * zoom;

	[HideInInspector] public new Camera camera;

	private void Awake() {
		camera = GetComponent<Camera>();
	}

	public void Set(Vector3 position) {
		transform.eulerAngles = new Vector3(90, 0, 0);
		zoom = position.y;

		target = new Vector3(position.x, 0, position.z);
	}

	private void Update() {
		if (GameController.World == null) return;

		zoom = Mathf.Clamp(zoom - Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity, GameController.World.size / 10, GameController.World.size / 2);

		if (Input.GetMouseButton(MouseButtonPan)) {
			float x = Input.GetAxisRaw("Mouse X");
			float y = Input.GetAxisRaw("Mouse Y");

			Vector3 movement = Time.deltaTime * panSensitivity * (zoom / 100) * new Vector3(x, 0, y);

			target -= transform.right * movement.x;

			Vector3 movementZ = transform.forward * movement.z;
			float magnitude = movementZ.magnitude;
			movementZ.y = 0;

			target -= magnitude * movementZ.normalized;

			target.x = Mathf.Clamp(target.x, 0, GameController.World.size);
			target.z = Mathf.Clamp(target.z, 0, GameController.World.size);

			transform.position = Position;
		} else if (Input.GetMouseButton(MouseButtonRotate)) {
			float x = Input.GetAxisRaw("Mouse X");
			float y = Input.GetAxisRaw("Mouse Y");

			transform.RotateAround(target, Vector3.up, x * Time.deltaTime * rotateSensitivity);
			transform.RotateAround(target, transform.right, -y * Time.deltaTime * rotateSensitivity);

			Vector3 clampedRotation = transform.eulerAngles;

			if (clampedRotation.x > 180f) clampedRotation.x = 0;

			if (clamp) transform.eulerAngles = clampedRotation;

			transform.position = Position;
		} else {
			transform.position = (transform.position - Position).magnitude > 0.01f
				? Vector3.Lerp(transform.position, Position, smoothing)
				: Position;
		}
	}
}