using System.Collections.Generic;

/// <summary>
/// Any moving unit on the map.
/// </summary>
public abstract class WorldUnit : WorldEntity {
	protected World World => tile.world;

	public Tile tile;
	public override Tile Tile => tile;

	protected LinkedList<Tile> path;
	protected LinkedListNode<Tile> node;
	public Tile Goal => path.Last.Value;

	protected WorldUnit(Tile tile) {
		this.tile = tile;
	}

	protected void Move() {
		node = node?.Next;
		if (node != null) tile = node.Value;
	}
}