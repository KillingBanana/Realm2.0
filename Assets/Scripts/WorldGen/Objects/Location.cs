using UnityEngine;

/// <summary>
/// A location on a tile that you can enter.
/// </summary>
public abstract class Location : WorldEntity {
	protected World World => tile.world;

	private readonly Tile tile;
	public override Tile Tile => tile;

	protected Location(Tile tile) {
		this.tile = tile;

		if (tile.location == null) {
			tile.location = this;
		} else {
			Debug.LogError($"Adding location to non-empty tile ({tile} already contains {tile.location})");
		}
	}
}