using System.Collections.Generic;
using UnityEngine;

public class Town : Location {
	public string Name { get; }
	public readonly Faction faction;
	public Race Race => faction.race;
	public int population;

	private readonly List<Settler> settlers = new List<Settler>();

	private int yearsSinceLastSettlers = 0;

	public Town(Tile tile, Faction faction, int population) : base(tile) {
		this.faction = faction;
		this.population = population;

		Name = Race.GetPlaceName();

		tile.customColor = Color.black;
	}

	public void Update() {
		yearsSinceLastSettlers++;

		if (population > 500 && yearsSinceLastSettlers >= 1250 * Mathf.Pow(population, -.5f)) {
			yearsSinceLastSettlers = 0;
			CreateSettlers();
		}

		population += (int) (Race.GetTileCompatibility(tile) / 1000 * population);

		foreach (Settler settler in settlers) {
			settler.Update();
		}
	}

	private void CreateSettlers() {
		Settler settler = new Settler(this, population / 10);
		population -= population / 10;
		settlers.Add(settler);
	}

	private string GetSize() => population > 2000 ? "city" : (population > 1000 ? "town" : (population > 500 ? "village" : "settlement"));

	public override string ToString() => $"{Name}, {Race.adjective} {GetSize()}";
}