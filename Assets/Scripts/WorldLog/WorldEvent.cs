public abstract class WorldEvent {
	public readonly int day;
	public readonly string text;
	public readonly WorldEventType type;
	public readonly WorldEventImportance importance;

	protected WorldEvent(int day, WorldEventImportance importance, WorldEventType type, string text) {
		this.day = day;
		this.type = type;
		this.importance = importance;
		this.text = text;
	}
}

public enum WorldEventType {
	SettlerCreation,
	TownCreation,
}

public enum WorldEventImportance {
	Mundane,
	Interesting,
	Important,
	Major
}