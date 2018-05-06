using System.Collections.Generic;
using JetBrains.Annotations;
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

	public Town(Tile tile, Faction faction, int population, [CanBeNull] Town parent) : base(tile) {
		if (parent != null && parent.faction != faction) {
			Debug.LogError($"Faction mismatch: {parent}: {parent.faction}, {faction}");
		}

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
			CreateSettlers(GetTownTile());
		}

		population++;

		settlers.RemoveAll(settler => !settler.Active);

		foreach (Settler settler in settlers) {
			settler.Update();
		}
	}

	private void CreateSettlers(Tile goal) {
		int settlerCount = population / 4;

		Settler settler = new Settler(this, goal, settlerCount);
		population -= settlerCount;

		settlers.Add(settler);
	}

	public Tile GetTownTile() {
		float influenceRange = GetInfluenceRange();

		int influenceRangeCeil = (int) influenceRange + 1;

		Tile townTile = null;

		for (int x = -influenceRangeCeil; x <= influenceRangeCeil; x++) {
			for (int y = -influenceRangeCeil; y <= influenceRangeCeil; y++) {
				int distanceSquared = x * x + y * y;
				if (distanceSquared - influenceRange * influenceRange < 2f) {
					Tile newTile = World.GetTile(tile.x + x, tile.y + y);

					if (newTile != null && !newTile.IsWater && (townTile == null || newTile.GetTownCompatibility(Race) > townTile.GetTownCompatibility(Race))) {
						townTile = newTile;
					}
				}
			}
		}

		return townTile;
	}

	public float GetInfluenceRange() => Mathf.Log(population / 15.625f, 2);

	private string GetSize() => population > 2000 ? "city" : (population > 1000 ? "town" : (population > 500 ? "village" : "settlement"));

	public override string ToString() => $"{Name}, {Race.adjective} {GetSize()}";
}