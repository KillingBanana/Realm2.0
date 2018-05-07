using System.Collections.Generic;

public class Road {
	public readonly Town child;

	private readonly List<Tile> tiles = new List<Tile>();
	public IReadOnlyList<Tile> Tiles => tiles.AsReadOnly();

	private readonly string name;

	public int Population => child.population;

	public Road(Town child) {
		this.child = child;
		child.roads.Add(this);

		name = $"{child.Race.GetPlaceName()} Road";

		AddTile(child.tile);
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