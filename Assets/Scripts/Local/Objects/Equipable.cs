using System.Linq;
using UnityEngine;

public class Equipable : Item {
	public readonly Slot slot;
	public readonly int slotSize;
	public readonly StatModifier[] modifiers;

	public sealed override string Name {
		get {
			switch (slot) {
				case Slot.Feet:
					return "Boots";
				case Slot.Hand:
					return slotSize > 1 ? "Longsword" : "Sword";
				case Slot.Head:
					return "Helmet";
				case Slot.Legs:
					return "Pants";
				case Slot.Neck:
					return "Necklace";
				case Slot.Torso:
					return "Shirt";
				case Slot.None:
					return "You shouldn't be seeing this.";
				default: return "Error: invalid slot";
			}
		}
	}

	public Equipable(Container container, int size, Slot slot) : base(container, size) {
		this.slot = slot;

		slotSize = slot == Slot.Feet || slot == Slot.Legs || slot == Slot.Hand && Utility.RandomBool ? 2 : 1;
		modifiers = new[] {new StatModifier(Name, "Stat boost from " + Name, Utility.RandomValue<Stat>(), Random.Range(1, 10))};
	}

	protected override string InspectText() => $"{base.InspectText()}\nSlot: {slot}{(slotSize > 1 ? " (x" + slotSize + ")" : "")}{modifiers.Aggregate("", (current, equipableModifier) => "\n" + current + equipableModifier)}";

	private void Equip(Character character) => character.equipment.TryAddItem(this);

	private void Unequip(Character character) => character.equipment.UnequipItem(this);


	protected override void Drop(Character character) {
		Unequip(character);
		base.Drop(character);
	}
}

public enum Slot {
	None,
	Head,
	Torso,
	Legs,
	Feet,
	Hand,
	Neck
}