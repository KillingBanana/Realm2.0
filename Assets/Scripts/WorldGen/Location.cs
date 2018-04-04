﻿using System.Collections.Generic;
using UnityEngine;

public abstract class Location {
	private readonly Tile tile;
	public Region Region => tile.region;
	public Climate Climate => Region.climate;

	public readonly List<Room> rooms = new List<Room>();

	protected Location(Tile tile) {
		if (tile.location != null) {
			Debug.LogError($"Adding location to non-empty tile ({tile})");
		}

		this.tile = tile;
		tile.location = this;
	}
}