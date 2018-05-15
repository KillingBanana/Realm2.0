public class Relationship {
	public int value;
	public RelationshipStatus status;
}

public enum RelationshipStatus {
	Allies,
	Friendly,
	Neutral,
	Hostile,
	War
}