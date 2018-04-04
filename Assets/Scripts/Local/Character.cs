using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public sealed class Character : Interactable {
	public readonly bool isPlayer;

	public override Room Room => room;
	private Room room;

	public bool isFemale;
	public Race race;
	private readonly string firstName;
	private readonly string lastName;
	public override string Name => firstName + " " + lastName;

	public readonly Body body;
	public readonly Inventory inventory;
	public readonly Equipment equipment;
	public bool HasItem(Item item) => inventory.Contains(item) || equipment.Contains(item);
	private readonly List<StatModifier> modifiers = new List<StatModifier>();

	public Character(Room room, bool isPlayer = false) : this(room, GameController.RandomRace(), Utility.RandomBool, isPlayer) { }

	public Character(Room room, Race race, bool isFemale, bool isPlayer = false) {
		this.room = room;
		this.race = race;
		this.isFemale = isFemale;
		firstName = race.GetFirstName(isFemale);
		lastName = race.GetLastName();

		this.isPlayer = isPlayer;
		if (isPlayer) ObjectManager.playerCharacter = this;

		inventory = new Inventory(this, 100);
		equipment = new Equipment(this);

		body = new Body(BodyType.Humanoid);
	}

	public int GetStat(Stat stat) {
		int statBase = 0;
		foreach (StatModifier statModifier in modifiers) {
			if (statModifier.stat == stat) statBase += statModifier.value;
		}

		foreach (Equipable equipable in equipment.Equipables) {
			statBase += equipable.modifiers.Where(statModifier => statModifier.stat == stat).Sum(statModifier => statModifier.value);
		}

		return statBase;
	}

	public override void TakeTurn() {
		inventory.TakeTurn();
		equipment.TakeTurn();
	}

	private void Talk(Character character) {
		GameController.DialogueManager.EnqueueSentence(new Sentence(Name, Utility.RandomBool ? "Hi!" : $"Hello, {character.Name}!"));
	}

	public void Attack(Character target) {
		if (isPlayer) {
			//Player.DisplayInteractions("Attack " + Name, target.body.Select(bodyPart => new Interaction(bodyPart.name, () => bodyPart.Attack(this), true)).ToList());
		}
	}
}