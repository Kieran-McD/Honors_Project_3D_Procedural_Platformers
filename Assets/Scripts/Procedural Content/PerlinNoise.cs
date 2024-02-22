using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public Renderer rend;

    public int width = 512;
    public int height = 512;

    public float offSetX = 100f;
    public float offSetY = 100f;

    public float scale = 20;

    public int octaves;
    public float persistance;
    public float lacunarity;

    public Texture2D perlinTexture;


    private void Start()
    {
        rend = GetComponent<Renderer>();
        perlinTexture = GenerateTexture();
        rend.material.mainTexture = perlinTexture;
    }



    private void OnValidate()
    {
        perlinTexture = GenerateTexture();
        rend.sharedMaterial.mainTexture = perlinTexture;
    }



    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float[,] noiseMap = new float[width, height];

        if(scale <= 0)
        {
            scale = 0.0001f;
        }


        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency;
                    float sampleY = y / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX + offSetX, sampleY + offSetY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x,y] = noiseHeight;
                texture.SetPixel(x, y, CalculateColour(noiseHeight));
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, CalculateColour(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y])));
            }
        }
                texture.Apply();

        return texture;
    }

    public void RandomizePerlinTexture()
    {
        offSetX = Random.Range(-10000, 10000);
        offSetY = Random.Range(-10000, 10000);
        perlinTexture = GenerateTexture();
        rend.sharedMaterial.mainTexture = perlinTexture;
    }

    Color CalculateColour(float colourValue)
    {;
        return new Color(colourValue, colourValue, colourValue);

    }
}
