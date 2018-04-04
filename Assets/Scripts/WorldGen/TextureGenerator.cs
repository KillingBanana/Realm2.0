using UnityEngine;

public static class TextureGenerator {
	public static Texture2D TextureFromColorMap(Color[] colourMap, int width, int height) {
		Texture2D texture = new Texture2D(width, height) {filterMode = FilterMode.Point};
		texture.SetPixels(colourMap);
		texture.Apply();
		return texture;
	}
}