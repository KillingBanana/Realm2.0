public class Civilization {
	public string Name { get; }
	public readonly Race race;
	public Town capital;

	public readonly float[,] influence;

	public Civilization(World world, Race race) {
		this.race = race;
		Name = race.GetPlaceName();
		influence = new float[world.size, world.size];
	}

	public override string ToString() => Name;
}