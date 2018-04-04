public class Floor : Container {
	public override Room Room { get; }

	public Floor(Room room) : base(room.Name) {
		Room = room;
	}
}