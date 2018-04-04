using Boo.Lang;
using UnityEngine;

public class Room : MonoBehaviour {
	public virtual string Name { get; }
	public readonly Location location;
	public readonly Floor floor;

	public readonly List<Character> characters = new List<Character>();
	public readonly List<Item> items = new List<Item>();
	public readonly List<Interactable> interactables = new List<Interactable>();

	public Room(string name, Location location) {
		Name = name;
		this.location = location;
		floor = new Floor(this);
	}
}