using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;
public class PoissonDiskSampling : MonoBehaviour
{
    public static List<Vector2> RandomPoints(float width, float height, float TotalPoints)
    {
        List<Vector2> points = new List<Vector2>();
       for(int i = 0; i < TotalPoints; i++)
         {
            points.Add(new Vector2(Random.Range(0, width), Random.Range(0, height)));
        }

        return points;
    }
}
