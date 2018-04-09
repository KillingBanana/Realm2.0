using UnityEngine;

public class Faction {
	public string Name { get; }
	public readonly Race race;
	public Town capital;

	public readonly Color color;

	public Faction(Race race) {
		this.race = race;
		Name = race.GetPlaceName();
		color = Random.ColorHSV(0, 1, 1, 1, 1, 1);
	}

	public override string ToString() => Name;
}