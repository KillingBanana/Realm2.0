public class Faction {
	public string Name { get; }
	public readonly Race race;
	public Town capital;

	public Faction(Race race) {
		this.race = race;
		Name = race.GetPlaceName();
	}

	public override string ToString() => Name;
}