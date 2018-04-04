using System.Collections.Generic;

public class BodyPart {
	public Body body;
	public readonly string name;

	public readonly Slot slot;
	public Equipable equipable;

	public BodyPart parent;
	public readonly HashSet<BodyPart> children = new HashSet<BodyPart>();

	public BodyPartX x;
	public BodyPartY y;
	public BodyPartZ z;

	private readonly HashSet<BodyPartAttribute> bodyPartAttributes;

	public BodyPart(string name, BodyPart parent, Slot slot, BodyPartX x, BodyPartY y, BodyPartZ z, HashSet<BodyPartAttribute> bodyPartAttributes) {
		this.name = name;
		this.slot = slot;
		this.parent = parent;
		parent?.AddChild(this);
		this.x = x;
		this.y = y;
		this.z = z;
		this.bodyPartAttributes = bodyPartAttributes;
	}

	public bool HasAttribute(BodyPartAttribute attribute) => bodyPartAttributes.Contains(attribute);

	public void AddChild(BodyPart bodyPart) {
		if (!children.Contains(bodyPart)) children.Add(bodyPart);
		bodyPart.parent = this;
	}

	public override string ToString() => name;
}

public enum BodyPartX {
	Middle,
	Left,
	Right
}

public enum BodyPartY {
	Middle,
	Top,
	Bottom
}

public enum BodyPartZ {
	Center,
	Front,
	Back
}

public enum BodyPartAttribute {
	Breathing,
	Flying,
	Grasping,
	Limb,
	Seeing,
	Thinking,
	Vital,
	Walking,
}