public class Civilization {
	public string Name { get; }
	public readonly Race race;
	public Town capital;

	public readonly float[,] influence;

	public Civilization(Map map, Race race) {
		this.race = race;
		Name = race.GetPlaceName();
		influence = new float[map.size, map.size];
	}

	public override string ToString() => Name;
}