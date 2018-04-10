using System.Collections.Generic;
using UnityEngine;

public class Town : Location {
	public string Name { get; }
	public readonly Faction faction;
	public Race Race => faction.race;
	public int population;

	public readonly List<Settler> settlers = new List<Settler>();
	private readonly Town parent;
	private readonly List<Town> childTowns = new List<Town>();

	private int yearsSinceLastSettlers;

	public Town(Tile tile, Faction faction, int population, Town parent) : base(tile) {
		this.faction = faction;
		this.population = population;

		Name = Race.GetPlaceName();

		this.parent = parent;
		parent?.childTowns.Add(this);
	}

	public void Update() {
		yearsSinceLastSettlers++;

		int desiredTowns = population / 1000;

		if (childTowns.Count < desiredTowns && Race.expansionism * yearsSinceLastSettlers >= 1500 * Mathf.Pow(population, -.5f)) {
			yearsSinceLastSettlers = 0;
			CreateSettlers();
		}

		population += (int) (Race.GetTileCompatibility(tile) / 1000 * population);

		foreach (Settler settler in settlers) {
			settler.Update();
		}
	}

	private void CreateSettlers() {
		int settlerCount = population / 4;
		Settler settler = new Settler(this, settlerCount);
		population -= settlerCount;
		settlers.Add(settler);
	}

	private string GetSize() => population > 2000 ? "city" : (population > 1000 ? "town" : (population > 500 ? "village" : "settlement"));

	public override string ToString() => $"{Name}, {Race.adjective} {GetSize()}";
}