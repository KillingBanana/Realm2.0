using System.Linq;

public class Inventory : Container {
	public readonly Character character;
	public override Room Room => character.Room;

	public readonly int maxSize;

	public int FreeSpace => maxSize - TakenSpace;
	public int TakenSpace => Items.Sum(item => item.size);

	public Inventory(Character character, int maxSize) : base(character.Name + "'s inventory") {
		this.character = character;
		this.maxSize = maxSize;
	}

	public override bool CanAddItem(Item item) => base.CanAddItem(item) && FreeSpace >= item.size;
}