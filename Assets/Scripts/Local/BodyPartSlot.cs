using UnityEngine.UI;

public class BodyPartSlot : ItemSlot {
	public BodyPart bodyPart;

	public override Item Item {
		get { return bodyPart.equipable; }
		set { }
	}

	private Text text;

	private void Awake() {
		text = GetComponentInChildren<Text>();
	}

	private void Update() {
		text.text = bodyPart.equipable == null ? bodyPart.name : bodyPart.equipable.Name;
	}
}