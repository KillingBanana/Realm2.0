public class TownCreationEvent : WorldEvent {
	public TownCreationEvent(int day, WorldEventImportance importance, Town town
	) : base(day, importance, WorldEventType.TownCreation, $"{town} was created") { }
}