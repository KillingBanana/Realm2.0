public class StatModifier {
	public string name;
	public string description;
	public Stat stat;
	public int value;

	public StatModifier(string name, string description, Stat stat, int value) {
		this.name = name;
		this.description = description;
		this.stat = stat;
		this.value = value;
	}

	public override string ToString() => stat + ": " + value;
}

public enum Stat {
	Strength,
	Agility,
	Endurance,
	Intelligence,
	Charisma
}