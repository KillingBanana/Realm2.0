using System.Collections.Generic;
using UnityEngine;

public class Road {
	private readonly Town parent;
	public Town child;

	private readonly List<Tile> tiles = new List<Tile>();
	public IReadOnlyList<Tile> Tiles => tiles.AsReadOnly();

	private readonly string name;

	public int Population => child?.population ?? population;
	private readonly int population;

	public Road(Town parent, int population) {
		this.population = population;

		this.parent = parent;
		parent.roads.Add(this);

		name = $"{parent.Race.GetPlaceName()} Road";

		AddTile(parent.tile);
	}

	public void AddTile(Tile tile) {
//		if (tiles.Contains(tile)) Debug.LogWarning($"{this} already contains {tile}");

		tiles.Add(tile);
		if (!tile.roads.Contains(this)) tile.roads.Add(this);
	}

	public override string ToString() {
		return name;
	}
}