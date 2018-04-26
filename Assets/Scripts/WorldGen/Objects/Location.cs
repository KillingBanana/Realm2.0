using System.Collections.Generic;
using UnityEngine;

public abstract class Location {
	public readonly World world;
	public readonly Tile tile;
	public Region Region => tile.region;
	public Climate Climate => Region.climate;

	public readonly List<Room> rooms = new List<Room>();

	protected Location(World world, Tile tile) {
		this.world = world;
		this.tile = tile;

		if (tile.location == null) {
			tile.location = this;
		} else {
			Debug.LogError($"Adding {this} to non-empty tile ({tile} already contains {tile.location})");
		}
	}
}