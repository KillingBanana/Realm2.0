public abstract class Entity {
	public abstract string Name { get; }
	public abstract Room Room { get; }
	public Location Location => Room.location;

	public sealed override string ToString() => Name;
}