using System.Collections.Generic;
using UnityEngine;

public abstract class Location {
	protected World World => tile.world;
	public readonly Tile tile;
	public Region Region => tile.region;
	public Climate Climate => Region.climate;

	public readonly List<Room> rooms = new List<Room>();

	protected Location(Tile tile) {
		this.tile = tile;

		if (tile.location == null) {
			tile.location = this;
		} else {
			Debug.LogError($"Adding location to non-empty tile ({tile} already contains {tile.location})");
		}
	}
}