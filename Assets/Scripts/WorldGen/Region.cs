using System.Collections.Generic;
using UnityEngine;

public sealed class Region {
	private string Name => name + " " + climate;
	private readonly string name;

	public readonly Climate climate;
	public bool IsWater => climate.isWater;

	private readonly HashSet<Tile> tiles;
	public int Size => tiles.Count;

	public readonly Color color;

	public Region(Climate climate, HashSet<Tile> tiles) {
		this.climate = climate;

		name = GameController.Races.RandomItem().GetPlaceName();
		color = Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1);

		this.tiles = tiles;
		foreach (Tile tile in tiles) {
			tile.SetRegion(this);
		}
	}

	public string GetSizeName() => tiles.Count > 10000 ? "Huge" : (tiles.Count > 5000 ? "Large" : (tiles.Count > 2000 ? "Medium" : (tiles.Count > 500 ? "Small" : "Tiny")));

	public override string ToString() => Name;
}