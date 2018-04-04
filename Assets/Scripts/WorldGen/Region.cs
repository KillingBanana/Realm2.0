using System.Collections.Generic;
using UnityEngine;

public sealed class Region {
	private string Name => name + " " + climate;
	private readonly string name;

	public readonly Climate climate;
	public bool IsWater => climate.isWater;

	private readonly List<Tile> tiles;
	public int Size => tiles.Count;

	public readonly Color color;

	public Region(Climate climate, List<Tile> tiles) {
		this.climate = climate;
		name = GameController.RandomRace().GetPlaceName();
		color = Random.ColorHSV(0, 1);
		foreach (Tile tile in tiles) {
			Add(tile);
		}

		this.tiles = tiles;
	}

	private bool Contains(Tile tile) => tiles != null && tiles.Contains(tile);

	private void Add(Tile tile) {
		if (Contains(tile)) return;

		tile.region?.Remove(tile);
		tile.SetRegion(this);
	}

	private void Remove(Tile tile) {
		if (!Contains(tile)) return;

		tiles.Remove(tile);
	}

	public string GetSize() => tiles.Count > 10000 ? "Huge" : (tiles.Count > 5000 ? "Large" : (tiles.Count > 2000 ? "Medium" : (tiles.Count > 500 ? "Small" : "Tiny")));

	public override string ToString() => Name;
}