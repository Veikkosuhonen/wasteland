using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapGeneration : MonoBehaviour {
    public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ) {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];
        for (int zIndex = 0; zIndex < mapDepth; zIndex++) {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++) {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = (xIndex + offsetX) * scale;
                float sampleZ = (zIndex + offsetZ) * scale;
                // generate noise value using PerlinNoise
                float lacunarity = 1.1f + Fbm(sampleX * 2 + 123f, sampleZ * 2 - 59, 8) * 0.005f;
                float gain = 0.7f + Fbm(sampleX * 4.2f + 53, sampleZ * 4.2f - 19, 8) * 0.05f;
                float noise = Fbm(sampleX, sampleZ, 32, lacunarity, gain)
                    * Fbm(sampleX * 1.1f - 100, sampleZ * 1.1f + 100, 16, 1.1f, 0.75f) * 0.2f
                    + Fbm(sampleX * 2, sampleZ * 2, 32, 1.2f, 0.8f) * 1f
                    + Fbm(sampleX + gain, sampleZ + lacunarity, 20, 1.2f, 0.79f) * 1.8f;

                float g2 = 0.7f + Fbm(sampleX * 2f - 123f, sampleZ + 59, 8) * 0.02f;
                float f = Fbm(sampleX / 3, sampleZ / 3, 4, 1.2f, 0.8f);
                noise -= Mathf.Clamp01(Fbm(sampleZ / 6 + noise, sampleX / 6, 16, 1.2f, g2) * f);

                float f2 = Fbm(sampleX * 3, sampleZ * 3 + 999, 8) * 0.5f;
                noise *= 1f + f2 * f2 * f2 * 0.3f;

                noise /= 2;
                noise *= noise;

                noise += Fbm(sampleX * 16, sampleZ * 16, 8, 1.1f, 0.9f) * 0.07f;

                noise /= 1.5f;

                noise = Mathf.Max(noise, 0.1f);
                
                noiseMap[zIndex, xIndex] = noise;
            }
        }
        return noiseMap;
    }

    private float Fbm(float x, float z, int octaves = 32, float lacunarity = 1.5f, float gain = 0.8f) {

        float s = 0f;
        float amplitude = 1.0f;
        float scale = 1.0f;


        for (int i = 0; i < octaves; i++) {
            s += (Mathf.PerlinNoise(x * scale, z * scale) * 2 - 1) * amplitude;
            scale *= lacunarity;
            amplitude *= gain;

            x += 11f * scale;
            z += 10f;
        }

        return s;
    }
}