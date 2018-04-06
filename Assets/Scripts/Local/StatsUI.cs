using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour {
	[SerializeField] private Text text;

	private void Update() {
		text.text = GetStatText(LocalManager.PlayerCharacter);
	}

	private static string GetStatText(Character character) {
		return Enum.GetValues(typeof(Stat)).Cast<Stat>().Aggregate("", (current, stat) => $"{current}{stat}: {character.GetStat(stat)}\n");
	}
}