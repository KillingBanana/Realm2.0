using System;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class WorldSettings {
	public bool benchmark;
	public WorldSize worldSize;
	private WorldSize lastWorldSize;

	[SerializeField] [Range(0, 2)] private int lodMultiplier;

	public int Size {
		get {
			if (cachedSize == -1 || lastWorldSize != worldSize) {
				cachedSize = GetSize(worldSize);
				lastWorldSize = worldSize;
			}

			return cachedSize;
		}
	}

	private int cachedSize = -1;

	public int Lod => Size / 256 * lodMultiplier;

	[Range(0, 999999)] public int seed;

	[Header("Heightmap"), SerializeField] private NoiseSettings heightSettings;

	[Range(0, 10)] public float falloffA;
	[Range(1, 10)] public float falloffB;
	[Range(0, 1)] public float falloffMultiplier;

	[Header("Temperature")] [Range(0, 10)] public float tempA;

	[Range(0.5f, 1.5f)] public float tempB;
	[Range(0, 1)] public float maxTempHeight = .33f, heightTempMultiplier;

	[Header("Humidity"), SerializeField] private NoiseSettings humiditySettings;
	[Range(0, 1)] public float maxHumidityHeight = .33f, heightHumidityMultiplier;

	[Header("Civilizations")] public bool drawRoads;
	[Range(1, 10)] public int factions;
	[Range(0, 1)] public float townDistanceFactor;

	public void GenerateHeightMap(float[,] map) {
		float[,] heightMap = GenerateNoiseMap(Size, seed, heightSettings);
		float[,] falloffMap = GenerateFalloffMap(Size, falloffA, falloffB);
		for (int y = 0; y < Size; y++) {
			for (int x = 0; x < Size; x++) {
				map[x, y] = Mathf.Clamp01(heightMap[x, y] - falloffMap[x, y] * falloffMultiplier);
			}
		}
	}

	public float[,] GenerateHumidityMap(float[,] heightMap) {
		float[,] humidityMap = GenerateNoiseMap(Size, seed / 2, humiditySettings);

		for (int y = 0; y < Size; y++) {
			for (int x = 0; x < Size; x++) {
				float humidity = (heightMap[x, y] * heightMap[x, y] - maxHumidityHeight * maxHumidityHeight) * heightHumidityMultiplier;

				humidityMap[x, y] = Mathf.Clamp01(humidityMap[x, y] - humidity);
			}
		}

		return humidityMap;
	}

	public float[,] GenerateTempMap(float[,] heightMap) {
		float[,] tempMap = new float[Size, Size];
		for (int y = 0; y < Size; y++) {
			for (int x = 0; x < Size; x++) {
				float heightTemp = Mathf.Abs(heightMap[x, y] * heightMap[x, y] - maxTempHeight * maxTempHeight) * heightTempMultiplier;
				float latitudeTemp = 1 - Mathf.Abs(Size - y) / (float) Size;

				float temp = Evaluate(Mathf.Clamp01(latitudeTemp - heightTemp), tempA, tempB);
				tempMap[x, y] = temp;
			}
		}

		return tempMap;
	}

	private static float[,] GenerateNoiseMap(int size, int seed, NoiseSettings noiseSettings) {
		float[,] noiseMap = new float[size, size];
		Random random = new Random(seed);
		Vector2[] octaveOffsets = new Vector2[noiseSettings.octaves];
		float amplitude = 1;
		for (int i = 0; i < noiseSettings.octaves; i++) {
			float offsetX = random.Next(-99999, 99999);
			float offsetY = random.Next(-99999, 99999);
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
			amplitude *= noiseSettings.persistance;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;
		float halfSize = size / 2f;
		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;
				for (int i = 0; i < noiseSettings.octaves; i++) {
					float sampleX = (x - halfSize + octaveOffsets[i].x) / noiseSettings.scale * frequency;
					float sampleY = (y - halfSize + octaveOffsets[i].y) / noiseSettings.scale * frequency;
					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;
					amplitude *= noiseSettings.persistance;
					frequency *= noiseSettings.lacunarity;
				}

				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				}

				if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}

				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
			}
		}

		return noiseMap;
	}

	private static float[,] GenerateFalloffMap(int size, float falloffA, float falloffB) {
		float[,] map = new float[size, size];
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				float x = i / (float) size * 2 - 1;
				float y = j / (float) size * 2 - 1;
				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				float falloff = Evaluate(value, falloffA, falloffB);
				map[i, j] = falloff;
			}
		}

		return map;
	}

	private static float Evaluate(float value, float a, float b) => Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));

	private static int GetSize(WorldSize worldSize) {
		switch (worldSize) {
			case WorldSize.Tiny:
				return 33;
			case WorldSize.Small:
				return 65;
			case WorldSize.Medium:
				return 129;
			case WorldSize.Large:
				return 257;
			case WorldSize.Huge:
				return 513;

			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}

public enum WorldSize {
	Tiny,
	Small,
	Medium,
	Large,
	Huge
}

[Serializable]
public struct NoiseSettings {
	[Range(1, 4)] public int octaves;
	[Range(0, 1)] public float persistance;
	[Range(1, 5)] public float lacunarity;
	[Range(10, 500)] public int scale;
}