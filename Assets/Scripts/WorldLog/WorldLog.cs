using System.Collections.Generic;

public class WorldLog {
	private readonly World world;

	private readonly List<WorldEvent> events = new List<WorldEvent>();

	public WorldLog(World world) {
		this.world = world;
	}

	public void Add(WorldEvent worldEvent) {
		events.Add(worldEvent);
	}

	public string GetLog() {
		string str = "";

		for (int day = 0; day <= world.Days; day++) {
			bool eventFound = false;
			foreach (WorldEvent worldEvent in events) {
				if (worldEvent.day == day) {
					if (!eventFound) {
						str += $"On {GetDay(day)},";
						eventFound = true;
					}

					str += $" {worldEvent.text},";
				}
			}

			if (eventFound) {
				str = str.Remove(str.Length - 1);
				str += ".\n";
			}
		}

		return str;
	}

	private static string GetDay(int day) {
		return $"day {day}";
	}
}