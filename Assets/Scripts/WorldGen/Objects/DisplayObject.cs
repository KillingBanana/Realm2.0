using UnityEngine;

[ExecuteInEditMode]
public abstract class DisplayObject<T> : MonoBehaviour where T : class {
	public T Target { get; private set; }

	public void Init(T target) {
		Target = target;
		name = target.ToString();
	}

	private void Update() {
		if (Target == null) {
			MapDisplay.SafeDestroy(gameObject);
		} else {
			UpdateDisplay();
		}
	}

	protected abstract void UpdateDisplay();
}