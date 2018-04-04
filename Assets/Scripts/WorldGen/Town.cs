using UnityEngine;

public class Town : Location {
	public string Name { get; }
	public readonly Civilization civilization;
	public Race Race => civilization.race;
	public int population;

	public Town(Tile tile, Civilization civilization, int population) : base(tile) {
		this.civilization = civilization;
		this.population = population;

		Name = Race.GetPlaceName();

		tile.customColor = Color.black;
	}

	private string GetSize() => population > 5000 ? "city" : (population > 1000 ? "town" : (population > 500 ? "village" : "settlement"));

	public override string ToString() => $"{Name}, {Race.adjective} {GetSize()}";
}