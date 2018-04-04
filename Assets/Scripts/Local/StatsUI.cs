using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour {
	[SerializeField] private Text text;

	private void Update() {
		text.text = GetStatText(ObjectManager.playerCharacter);
	}

	private static string GetStatText(Character character) {
		return Enum.GetValues(typeof(Stat)).Cast<Stat>().Aggregate("", (current, stat) => current + (stat + ": " + character.GetStat(stat)) + "\n");
	}
}