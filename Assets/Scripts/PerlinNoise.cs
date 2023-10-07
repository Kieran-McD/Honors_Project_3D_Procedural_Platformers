using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public Renderer renderer;

    public int width = 256;
    public int height = 256;

    public float offSetX = 100f;
    public float offSetY = 100f;

    public float scale = 20;

    public Texture2D perlinTexture;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        perlinTexture = GenerateTexture();
        renderer.material.mainTexture = perlinTexture;
    }



    private void OnValidate()
    {
        perlinTexture = GenerateTexture();
        renderer.sharedMaterial.mainTexture = perlinTexture;
    }



    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Color color = CalculateColour(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        return texture;
    }

    Color CalculateColour(int x, int y)
    {
        float xCoords = (float)x / width * scale + offSetX;
        float yCoords = (float)y / height * scale + offSetY;

        float sample = Mathf.PerlinNoise(xCoords, yCoords);
        return new Color(sample, sample, sample);

    }
}
