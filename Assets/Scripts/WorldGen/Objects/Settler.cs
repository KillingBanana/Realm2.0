using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Settler : WorldUnit {
	private readonly Town startingTown;
	private Town childTown;
	private Road road;

	private Faction Faction => startingTown.faction;
	private Race Race => startingTown.Race;

	public readonly int population;

	public bool Active { get; private set; } = true;

	public Settler(Town town, Tile goal, int population) : base(town.Tile) {
		startingTown = town;
		this.population = population;

		SetPath(FindPath(goal));
	}

	public void Update() {
		if (path != null) {
			CheckTile();
			Move();
		} else {
			LinkedList<Tile> newPath = FindPath(startingTown.GetTownTile());
			if (newPath != null) {
				SetPath(newPath);
			} else {
				Debug.Log("Path not found, destroying");
				Destroy();
			}
		}
	}

	private LinkedList<Tile> FindPath(Tile goal) {
		return Pathfinding.FindPath(tile, goal, Race);
	}

	private void SetPath(LinkedList<Tile> newPath) {
		path = newPath;
		node = newPath?.First;
	}

	private void CheckTile() {
		if (road != null) { //On the way back
			road.AddTile(tile);
			if (tile == Goal) {
				Destroy();
			}
		} else { //On the way to create a town
			if (tile == Goal) {
				CreateTown();
			}
		}
	}

	private void CreateTown() {
		if (tile.location != null) {
			Debug.Log($"{tile} already contains location, requesting new location");
			SetPath(FindPath(startingTown.GetTownTile()));
			return;
		}

		childTown = new Town(tile, Faction, population, startingTown);
		World.towns.Add(childTown);

		World.log.Add(new TownCreationEvent(World.Days, WorldEventImportance.Interesting, childTown));

		road = new Road(childTown);
		SetPath(FindPath(startingTown.Tile));
	}

	private void Destroy() {
		Active = false;
	}

	public override string ToString() => $"Settlers from {startingTown.Name}";
}