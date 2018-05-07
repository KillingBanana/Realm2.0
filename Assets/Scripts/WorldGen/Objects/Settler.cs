using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Settler {
	private readonly World world;
	private readonly Town startingTown;
	private Town childTown;
	private Road road;

	private Faction Faction => startingTown.faction;
	private Race Race => startingTown.Race;

	public readonly int population;

	private LinkedList<Tile> path;
	private LinkedListNode<Tile> node;
	public Tile tile;
	public Tile Goal => path.Last.Value;

	public bool Active { get; private set; } = true;

	public Settler(Town town, Tile goal, int population) {
		startingTown = town;
		this.population = population;
		world = town.tile.world;

		tile = town.tile;

		SetPath(Pathfinding.FindPath(town.tile, goal, Race));
	}

	public void Update() {
		if (path != null) {
			CheckTile();
			ProcessPath();
		} else {
			LinkedList<Tile> newPath = Pathfinding.FindPath(tile, startingTown.GetTownTile(), Race);
			if (newPath != null) {
				SetPath(newPath);
			} else {
				Debug.Log("Path not found, destroying");
				Destroy();
			}
		}
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

	private void ProcessPath() {
		node = node?.Next;
		if (node != null) tile = node.Value;
	}

	private void CreateTown() {
		if (tile.location != null) {
			Debug.Log($"{tile} already contains location, requesting new location");
			SetPath(Pathfinding.FindPath(tile, startingTown.GetTownTile(), Race));
			return;
		}

		childTown = new Town(tile, Faction, population, startingTown);
		world.towns.Add(childTown);

		road = new Road(childTown);
		SetPath(Pathfinding.FindPath(tile, startingTown.tile, Race));
	}

	private void Destroy() {
		Active = false;
	}

	public override string ToString() => $"Settlers from {startingTown}";
}