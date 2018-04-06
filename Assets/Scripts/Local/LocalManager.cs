using UnityEngine;

public static class LocalManager {
	private static Character playerCharacter;

	public static Character PlayerCharacter {
		get {
			if (playerCharacter == null) {
				Debug.LogError("Player character not set");
			}

			return playerCharacter;
		}
		set { playerCharacter = value; }
	}
}