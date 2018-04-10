using System;
using UnityEngine;

public static class MeshGenerator {
	public static Mesh GenerateTerrainMesh(float[,] heightMap, int lod, int meshWorldSize, Func<int, int, float> getHeightFunc) {
		int size = heightMap.GetLength(0);

		int meshStepSize = lod == 0 ? 1 : lod * 2;
		int meshSize = (size - 1) / meshStepSize + 1;

		float meshScale = (float) meshWorldSize / size;
		float topLeftX = (meshWorldSize - 1) / -2f;
		float topLeftZ = (meshWorldSize - 1) / 2f;

		MeshData meshData = new MeshData(meshSize);
		int vertexIndex = 0;

		for (int y = 0; y < size; y += meshStepSize) {
			for (int x = 0; x < size; x += meshStepSize) {
				float height = getHeightFunc.Invoke(x, y);
				float worldX = x * meshScale;
				float worldY = y * meshScale;
				meshData.vertices[vertexIndex] = new Vector3(topLeftX + worldX, height, topLeftZ - worldY);
				meshData.uvs[vertexIndex] = new Vector2(x / (float) size, y / (float) size);

				if (x < size - 1 && y < size - 1) {
					meshData.AddTriangle(vertexIndex, vertexIndex + meshSize + 1, vertexIndex + meshSize);
					meshData.AddTriangle(vertexIndex + meshSize + 1, vertexIndex, vertexIndex + 1);
				}

				vertexIndex++;
			}
		}

		return meshData.CreateMesh();
	}

	private struct MeshData {
		public readonly Vector3[] vertices;
		private readonly int[] triangles;
		public readonly Vector2[] uvs;

		private int triangleIndex;

		public MeshData(int size) {
			vertices = new Vector3[size * size];
			uvs = new Vector2[size * size];
			triangles = new int[(size - 1) * (size - 1) * 6];
			triangleIndex = 0;
		}

		public void AddTriangle(int a, int b, int c) {
			triangles[triangleIndex] = a;
			triangles[triangleIndex + 1] = b;
			triangles[triangleIndex + 2] = c;
			triangleIndex += 3;
		}

		public Mesh CreateMesh() {
			Mesh mesh = new Mesh {
				vertices = vertices,
				triangles = triangles,
				uv = uvs
			};
			mesh.RecalculateNormals();
			return mesh;
		}
	}
}