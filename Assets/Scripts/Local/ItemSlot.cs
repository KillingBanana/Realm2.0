using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemSlot : MonoBehaviour {
	public virtual Item Item { get; set; }

	[UsedImplicitly]
	public void OnClick() {
		//TODO if (Item != null)Player.DisplayInteractable(Item);
	}
}