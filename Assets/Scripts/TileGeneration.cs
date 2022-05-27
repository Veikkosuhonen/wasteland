using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour {

    [SerializeField]
    NoiseMapGeneration noiseMapGeneration;

    [SerializeField]
    private MeshRenderer tileRenderer;

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private MeshCollider meshCollider;

    [SerializeField]
    AnimationCurve heightCurve;

    public float MapScale;

    public float HeightMultiplier;

    void Start() {
        GenerateTile();
    }

    void GenerateTile() {
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        // calculate the offsets based on the tile position
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        // generate a heightMap using noise
        float[,] heightMap = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, MapScale, offsetX, offsetZ);

        Texture2D tileTexture = BuildTexture(heightMap);
        this.tileRenderer.material.mainTexture = tileTexture;

        UpdateMeshVertices(heightMap);
    }

    private Texture2D BuildTexture(float[,] heightMap) {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);
        Color32[] colorMap = new Color32[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++) {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++) {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];
                // assign as color a shade of grey proportional to the height value
                colorMap[colorIndex] = Color32.Lerp(Color.black, Color.white, height / 2f);
            }
        }
        // create a new texture and set its pixel colors
        Texture2D tileTexture = new(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels32(colorMap);
        tileTexture.Apply();
        return tileTexture;
    }

    private void UpdateMeshVertices(float[,] heightMap) {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;

        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++) {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++) {
                float height = heightCurve.Evaluate(heightMap[zIndex, xIndex]);
                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, height * HeightMultiplier, vertex.z);
                vertexIndex++;
            }
        }

        this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        this.meshCollider.sharedMesh = this.meshFilter.mesh;
    }
}