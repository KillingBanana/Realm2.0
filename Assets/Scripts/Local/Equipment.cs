using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Equipment : Container {
	private readonly Character character;
	public override Room Room => character.Room;

	public override HashSet<Item> Items => new HashSet<Item>(Equipables);
	public HashSet<Equipable> Equipables => character.body.Equipables;

	public Equipment(Character character) : base(character.Name + "'s equipment") {
		this.character = character;
	}

	public override bool TryAddItem(Item item) {
		if (!CanAddItem(item)) return false;
		Equipable equipable = item as Equipable;

		if (equipable == null) return false;

		List<BodyPart> validParts = (from bodyPart in character.body.bodyParts where bodyPart.slot == equipable.slot select bodyPart).ToList();

		if (validParts.Count < equipable.slotSize) return false;

		if (validParts.Count == equipable.slotSize) {
			EquipItem(equipable, validParts);
			return true;
		}

		List<BodyPart> freeParts = validParts.Where(bodyPart => bodyPart.equipable == null).ToList();

		if (freeParts.Count >= equipable.slotSize) {
			EquipItem(equipable, validParts);
			return true;
		}

		return false;
	}

	public override void Remove(Item item) {
		UnequipItem(item);
	}


	private void EquipItem(Equipable equipable, IEnumerable<BodyPart> bodyParts) {
		equipable.SetContainer(this);
		foreach (BodyPart chosenPart in bodyParts) {
			if (chosenPart.equipable != null) RemoveItem(chosenPart);
			chosenPart.equipable = equipable;
		}
	}

	public void UnequipItem(Item item) {
		if (!(item is Equipable)) {
			Debug.LogError($"Trying to remove non-equipable item ({item})");
			return;
		}

		Equipable equipable = (Equipable) item;

		if (!Contains(equipable)) {
			Debug.LogError($"Trying to remove non-equipped item ({item})");
			return;
		}

		foreach (BodyPart bodyPart in character.body.bodyParts) {
			if (bodyPart.equipable == equipable) RemoveItem(bodyPart);
		}
	}

	private void RemoveItem(BodyPart bodyPart) {
		if (!character.inventory.TryAddItem(bodyPart.equipable)) {
			Room.floor.TryAddItem(bodyPart.equipable);
		}

		bodyPart.equipable = null;
	}
}