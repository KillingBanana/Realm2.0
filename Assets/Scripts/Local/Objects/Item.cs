public abstract class Item : Interactable {
	public readonly int size;

	private Container container;
	public override Room Room => container.Room;

	protected Item(Container container, int size) {
		this.container = container;
		this.size = size;
		interactions.Add(new Interaction("Drop", c => c.HasItem(this), Drop, false));
		interactions.Add(new Interaction("Pick up", c => !c.HasItem(this), PickUp, false));
	}

	public void SetContainer(Container newContainer) {
		container.Remove(this);
		container = newContainer;
	}

	private void PickUp(Character character) {
		character.inventory.TryAddItem(this);
	}

	protected virtual void Drop(Character character) => character.Room.floor.TryAddItem(this);

	protected override string InspectText() => Name + "\nSize: " + size;
}