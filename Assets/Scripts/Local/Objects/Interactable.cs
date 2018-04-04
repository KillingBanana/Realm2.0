using System.Collections.Generic;
using System.Linq;

public abstract class Interactable : Entity {
	protected Interactable() {
		interactions.Add(new Interaction("Inspect", c => true, Inspect, false));
	}

	protected readonly List<Interaction> interactions = new List<Interaction>();

	public IEnumerable<Interaction> Interactions(Character character) => interactions.Where((interaction, i) => interaction.predicate.Invoke(character));

	public virtual void TakeTurn() { }

	private void Inspect(Character character) => Log.Add(InspectText());

	protected virtual string InspectText() => Name;
}

