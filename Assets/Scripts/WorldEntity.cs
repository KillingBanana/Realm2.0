/// <summary>
/// Any entity that exists in the world.
/// </summary>
public abstract class WorldEntity {
	public abstract Tile Tile { get; }
	public abstract override string ToString();
}