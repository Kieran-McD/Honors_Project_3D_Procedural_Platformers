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

    public void SetUp(List<Vector2> sitePositions)
    {
        perlinTexture = CreateTexture(diagram, sitePositions);
        rend.material.mainTexture = perlinTexture;
    }

    public Texture2D CreateTexture(VoronoiDiagram voronoi, List<Vector2> sites)
    {
        Texture2D texture = new Texture2D(width, height);

        List<Vector2> center = new List<Vector2>();
        List<Vector2> vectorOne = new List<Vector2>();
        List<Vector2> vectorTwo = new List<Vector2>();
        List<int> minXList = new List<int>();
        List<int> minYList = new List<int>();
        List<int> maxXList = new List<int>();
        List<int> maxYList = new List<int>();
        int minX = width, minY = height, maxX = 0, maxY = 0;

        for(int i = 0; i < sites.Count; i++)
        {
            if (sites[i].X < minX) minX = (int)sites[i].X;
            if (sites[i].X > maxX) maxX = (int)sites[i].X;
            if (sites[i].Y < minY) minY = (int)sites[i].Y;
            if (sites[i].Y > maxY) maxY = (int)sites[i].Y;

            center.Add(sites[i]);
            for (int j = 0; j < voronoi.voronoi.Region(center[i]).Count; j++)
            {
                List<Vector2> region = voronoi.voronoi.Region(center[i]);

                if (j == voronoi.voronoi.Region(center[i]).Count - 1)
                {
                    vectorOne.Add(region[region.Count - 1] - center[i]);
                    vectorTwo.Add(region[0] - center[i]);
                    //Gets the max boundaries for each triangle
                    minXList.Add(Mathf.Min(Mathf.Min((int)center[i].X, (int)region[region.Count - 1].X), (int)region[0].X));
                    maxXList.Add(Mathf.Max(Mathf.Max((int)center[i].X, (int)region[region.Count - 1].X), (int)region[0].X));
                    minYList.Add(Mathf.Min(Mathf.Min((int)center[i].Y, (int)region[region.Count - 1].Y), (int)region[0].Y));
                    maxYList.Add(Mathf.Max(Mathf.Max((int)center[i].Y, (int)region[region.Count - 1].Y), (int)region[0].Y));
                    break;
                }

                vectorOne.Add(region[j] - center[i]);
                vectorTwo.Add(region[j + 1] - center[i]);
                //Gets the max boundaries for each triangle
                minXList.Add(Mathf.Min(Mathf.Min((int)center[i].X, (int)region[j].X), (int)region[j + 1].X));
                maxXList.Add(Mathf.Max(Mathf.Max((int)center[i].X, (int)region[j].X), (int)region[j + 1].X));
                minYList.Add(Mathf.Min(Mathf.Min((int)center[i].Y, (int)region[j].Y), (int)region[j + 1].Y));
                maxYList.Add(Mathf.Max(Mathf.Max((int)center[i].Y, (int)region[j].Y), (int)region[j + 1].Y));
            }
        }

        //Old Version very bad
        //for (int x = minX; x < maxX; x++)
        //{
        //    for (int y = minY; y < maxY; y++)
        //    {

        //        Vector2 point = new Vector2(x, y);
        //        //float a,b;

        //        //float det1, det2, det3;

        //        bool pointInTriangle = false;
        //        int currentTotal = 0;
        //        for (int i = 0; i < center.Count; i++)
        //        {

        //            for (int j = currentTotal; j < voronoi.voronoi.Region(center[i]).Count + currentTotal; j++)
        //            {

        //                //det1 = point.X * vectorTwo[j].Y - point.Y * vectorTwo[j].X;
        //                //det2 = center[i].X * vectorTwo[j].Y - center[i].Y * vectorTwo[j].X;
        //                //det3 = vectorOne[j].X * vectorTwo[j].Y - vectorOne[j].Y * vectorTwo[j].X;
        //                //a = (det1 - det2) / det3;
        //                //det1 = point.X * vectorOne[j].Y - point.Y * vectorOne[j].X;
        //                //det2 = center[i].X * vectorOne[j].Y - center[i].Y * vectorOne[j].X;
        //                //det3 = vectorOne[j].X * vectorTwo[j].Y - vectorOne[j].Y * vectorTwo[j].X;
        //                //b = -((det1 - det2) / det3);


        //                //if (a >= 0 && b >= 0 && (a + b) <= 1)
        //                //{
        //                //    //Debug.Log("Hell yeah this worked like a charm");
        //                //    texture.SetPixel(x, y, new Color(1, 1, 1));
        //                //    pointInTriangle = true;
        //                //    break;
        //                //}

        //                if (CheckPointInTriangle(point, center[i], vectorOne[j], vectorTwo[j]))
        //                {
        //                    texture.SetPixel(x, y, new Color(0, 0, 0));
        //                    pointInTriangle = true;
        //                    break;
        //                }
        //            }
        //            if (pointInTriangle) break;
        //            currentTotal += voronoi.voronoi.Region(center[i]).Count;
        //        }

               
        //        //else texture.SetPixel(x, y, new Color(0, 0, 0));
        //    }
        //}




        //New Version so much better
        int currentTotal = 0;
        for (int i = 0; i < center.Count; i++)
        {

            for (int j = 0; j < voronoi.voronoi.Region(center[i]).Count; j++)
            {

                for (int x = minXList[j + currentTotal]; x <= maxXList[j + currentTotal]; x++)
                {
                    for (int y = minYList[j + currentTotal]; y <= maxYList[j + currentTotal]; y++)
                    {

                        Vector2 point = new Vector2(x, y);
                        //det1 = point.X * vectorTwo[j].Y - point.Y * vectorTwo[j].X;
                        //det2 = center[i].X * vectorTwo[j].Y - center[i].Y * vectorTwo[j].X;
                        //det3 = vectorOne[j].X * vectorTwo[j].Y - vectorOne[j].Y * vectorTwo[j].X;
                        //a = (det1 - det2) / det3;
                        //det1 = point.X * vectorOne[j].Y - point.Y * vectorOne[j].X;
                        //det2 = center[i].X * vectorOne[j].Y - center[i].Y * vectorOne[j].X;
                        //det3 = vectorOne[j].X * vectorTwo[j].Y - vectorOne[j].Y * vectorTwo[j].X;
                        //b = -((det1 - det2) / det3);


                        //if (a >= 0 && b >= 0 && (a + b) <= 1)
                        //{
                        //    //Debug.Log("Hell yeah this worked like a charm");
                        //    texture.SetPixel(x, y, new Color(1, 1, 1));
                        //    pointInTriangle = true;
                        //    break;
                        //}
                        //texture.SetPixel(x, y, new Color(0, 0, 0));
                        if (CheckPointInTriangle(point, center[i], vectorOne[j + currentTotal], vectorTwo[j + currentTotal]))
                        {
                            texture.SetPixel(x, y, new Color(0, 0, 0));
                            //pointInTriangle = true;
                            //break;
                        }
                    }
                    //if (pointInTriangle) break;
                   
                }

            }
            currentTotal += voronoi.voronoi.Region(center[i]).Count;
        }
                //else texture.SetPixel(x, y, new Color(0, 0, 0));
            
       

        texture.Apply();

        return texture;
    }

    public bool CheckPointInTriangle(Vector2 point, Vector2 trianglePoint ,Vector2 vectorOne, Vector2 vectorTwo)
    {
        float det1, det2, det3, a, b;

        det1 = point.X * vectorTwo.Y - point.Y * vectorTwo.X;
        det2 = trianglePoint.X * vectorTwo.Y - trianglePoint.Y * vectorTwo.X;
        det3 = vectorOne.X * vectorTwo.Y - vectorOne.Y * vectorTwo.X;
        a = (det1 - det2) / det3;
        det1 = point.X * vectorOne.Y - point.Y * vectorOne.X;
        det2 = trianglePoint.X * vectorOne.Y - trianglePoint.Y * vectorOne.X;
        det3 = vectorOne.X * vectorTwo.Y - vectorOne.Y * vectorTwo.X;
        b = -((det1 - det2) / det3);

        if (a >= 0 && b >= 0 && (a + b) <= 1)
        {
            return true;
        }

        return false;
    }

}
