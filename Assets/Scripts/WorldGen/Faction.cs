using JetBrains.Annotations;
using UnityEngine;

public class Faction : WorldEntity {
	public string Name { get; }
	public readonly Race race;

	public Town capital;
	public override Tile Tile => capital.Tile;

	public readonly Color color;

	public Faction([NotNull] Race race) {
		this.race = race;
		Name = race.GetPlaceName();
		color = Random.ColorHSV(0, 1, 1, 1, 1, 1);
	}

	public override string ToString() => Name;
}