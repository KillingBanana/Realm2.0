using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract class Location {
	public readonly Tile tile;
	public Region Region => tile.region;
	public Climate Climate => Region.climate;

	public readonly List<Room> rooms = new List<Room>();

	protected Location(Tile tile) {
		if (tile.location != null) {
			Debug.LogError($"Adding location to non-empty tile ({tile})");
			return;
		}

		this.tile = tile;
		tile.location = this;
	}
}