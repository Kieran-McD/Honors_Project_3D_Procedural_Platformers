using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class VoronoiAvailableTerrain : MonoBehaviour
{
    public int width = 512;
    public int height = 512;

    public Texture2D perlinTexture;
    public Renderer rend;
    public VoronoiDiagram diagram;

    public void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetUp()
    {
        perlinTexture = CreateTexture(diagram);
        rend.material.mainTexture = perlinTexture;
    }

    public Texture2D CreateTexture(VoronoiDiagram voronoi)
    {
        Texture2D texture = new Texture2D(width, height);

        Vector2 center = voronoi.voronoi.SiteCoords()[0];
        List<Vector2> vectorOne = new List<Vector2>();
        List<Vector2> vectorTwo = new List<Vector2>();

        
        for (int i = 0; i < voronoi.voronoi.Region(center).Count; i++)
        {
            if(i == voronoi.voronoi.Region(center).Count - 1)
            {
                vectorOne.Add(voronoi.voronoi.Region(center)[voronoi.voronoi.Region(center).Count-1] - center);
                vectorTwo.Add(voronoi.voronoi.Region(center)[0] - center);
                break;
            }

            vectorOne.Add(voronoi.voronoi.Region(center)[i] - center);
            vectorTwo.Add(voronoi.voronoi.Region(center)[i+1] - center);
        }
       
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 point = new Vector2(x, y);
                float a,b;

                float det1, det2, det3;

                bool pointInTriangle = false;

                for(int i = 0; i < vectorOne.Count; i++)
                {
                    
                    det1 = point.X * vectorTwo[i].Y - point.Y * vectorTwo[i].X;
                    det2 = center.X * vectorTwo[i].Y - center.Y * vectorTwo[i].X;
                    det3 = vectorOne[i].X * vectorTwo[i].Y - vectorOne[i].Y * vectorTwo[i].X;
                    a = (det1 - det2) / det3;
                    det1 = point.X * vectorOne[i].Y - point.Y * vectorOne[i].X;
                    det2 = center.X * vectorOne[i].Y - center.Y * vectorOne[i].X;
                    det3 = vectorOne[i].X * vectorTwo[i].Y - vectorOne[i].Y * vectorTwo[i].X;
                    b = -((det1 - det2) / det3);

                    
                    if (a >= 0 && b >= 0 && (a + b) <= 1)
                    {
                        //Debug.Log("Hell yeah this worked like a charm");
                        texture.SetPixel(x, y, new Color(1, 1, 1));
                        pointInTriangle = true;
                        break;
                    }
                    
                }

                if (pointInTriangle) continue;
                else texture.SetPixel(x, y, new Color(0, 0, 0));

            }
        }

        texture.Apply();

        return texture;
    }
}
