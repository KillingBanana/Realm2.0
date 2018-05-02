using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Town : Location {
	public string Name { get; }

	public readonly Faction faction;
	public Race Race => faction.race;

	public int population;

	public readonly List<Settler> settlers = new List<Settler>();
	public readonly Town parent;
	private readonly List<Town> childTowns = new List<Town>();
	public readonly List<Road> roads = new List<Road>();

	private int yearsSinceLastSettlers;

	private readonly int dx, dy;

	public Town(World world, Tile tile, Faction faction, int population, Town parent, int dx, int dy) : base(world, tile) {
		if (faction == null) {
			Debug.LogError($"Faction Null ({parent})");
		}

		this.faction = faction;
		this.population = population;

		Name = Race.GetPlaceName();

		this.parent = parent;
		this.dx = dx;
		this.dy = dy;
		parent?.childTowns.Add(this);
	}

	public void Update() {
		yearsSinceLastSettlers++;

		int desiredTowns = population / 1000;

		if (childTowns.Count < desiredTowns && Race.expansionism * yearsSinceLastSettlers >= 1500 * Mathf.Pow(population, -.5f)) {
			yearsSinceLastSettlers = 0;
			CreateSettlers();
		}

		population++;

		foreach (Settler settler in settlers) {
			settler.Update();
		}

		settlers.RemoveAll(settler => !settler.Active);
	}

	private void CreateSettlers() {
		int settlerCount = population / 4;
		Settler settler = new Settler(this, settlerCount, dx, dy);
		population -= settlerCount;

		settlers.Add(settler);
	}

	private string GetSize() => population > 2000 ? "city" : (population > 1000 ? "town" : (population > 500 ? "village" : "settlement"));

	public override string ToString() => $"{Name}, {Race.adjective} {GetSize()}";
}