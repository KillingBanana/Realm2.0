using System;

public class Interaction {
	public readonly string name;
	public readonly Predicate<Character> predicate;
	private readonly Action<Character> action;
	public readonly bool skipTurn;

	public Interaction(string name, Predicate<Character> predicate, Action<Character> action, bool skipTurn) {
		this.name = name;
		this.predicate = predicate;
		this.action = action;
		this.skipTurn = skipTurn;
	}

	public virtual void Invoke(Character character) => action.Invoke(character);
}