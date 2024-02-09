using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using Vector2 = System.Numerics.Vector2;
using TMPro.EditorUtilities;


public class VoronoiMeshGenerator : MonoBehaviour
{
    
    public VoronoiDiagram voronoiDiagram;

    public void GeneratePlane()
    {

        if (!voronoiDiagram) { return; }
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        List<List<Vector3>> regionPlotPoints = GenerateVertices(voronoiDiagram.voronoi);

        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i < regionPlotPoints.Count; i++)
        {
            List<Vector3> points = regionPlotPoints[i];
            for (int j = 0; j < points.Count; j++)
            {
                vertices.Add(points[j]);
                Debug.Log("Total Vertices: " + vertices.Count);
            }
        }

        Debug.Log("Total Vertices: " + vertices.Count);

        mesh.vertices = vertices.ToArray();

        List<Vector3> normals = new List<Vector3>();

        for (int i = 0; i < vertices.Count; i++)
        {
            normals.Add(Vector3.up);
        }
        mesh.normals = normals.ToArray();
        mesh.triangles = GenerateTriangles(regionPlotPoints).ToArray();
    }

    public void GeneratePlane(Voronoi tempVoronoi)
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        List<List<Vector3>> regionPlotPoints = GenerateVertices(tempVoronoi);
        
        List<Vector3> vertices = new List<Vector3>();

        for(int i = 0; i < regionPlotPoints.Count; i++)
        {
            List<Vector3> points = regionPlotPoints[i];
            for(int j = 0; j < points.Count; j++)
            {
                vertices.Add(points[j]);
                Debug.Log("Total Vertices: " + vertices.Count);
            }
        }

        Debug.Log("Total Vertices: " + vertices.Count);

        mesh.vertices = vertices.ToArray();

        List<Vector3> normals = new List<Vector3>();

        for(int i = 0; i < vertices.Count; i++)
        {
            normals.Add(Vector3.up);
        }
        mesh.normals = normals.ToArray();
        mesh.triangles = GenerateTriangles(regionPlotPoints).ToArray();
        mesh.colors = GenerateColourRegions(regionPlotPoints).ToArray();

        mesh.RecalculateNormals();
    }

    List<List<Vector3>> GenerateVertices(Voronoi voronoi)
    {    

        List<Vector3> vertices = new List<Vector3>();

        List<Vector2> siteCoords = voronoi.SiteCoords();
        //List<Vector2> plotPoints = voronoi.Region(siteCoords[0]);

        //for (int i = 0; i < plotPoints.Count; i++)
        //{
            
        //    vertices.Add(new Vector3(plotPoints[i].X/512f, 0, plotPoints[i].Y/512f));
        //    Debug.Log("Plot Point: " + i + " Position: " + vertices[i]);
        //}

        //return vertices;


        List<List<Vector3>> regionPlotPoints = new List<List<Vector3>>();

        int totalPointsSoFar = 0;

        for (int i = 0; i < siteCoords.Count; i++)
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(new Vector3(siteCoords[i].X / 512f, 0, siteCoords[i].Y / 512));

            List<Vector2> plotPoints = voronoi.Region(siteCoords[i]);

            for (int j = 0; j < plotPoints.Count; j++)
            {
                points.Add(new Vector3(plotPoints[j].X / 512f, 0, plotPoints[j].Y / 512f));
                Debug.Log("Plot Point: " + (i * plotPoints.Count + j) + " Position: " + points[j]);
            }
            totalPointsSoFar += points.Count;
            regionPlotPoints.Add(points);
        }


        return regionPlotPoints;

    }

    List<Vector3> GenerateNormals()
    {
        List<Vector3> normals = new List<Vector3>();



        return normals;
    }

    //If Vertices generation changes, change triangle generation
    List<int> GenerateTriangles(List<List<Vector3>> vertices)
    {
        List<int> triangles = new List<int>();
        int totalPointsSoFar = 0;

        for(int i = 0; i < vertices.Count; i++)
        {
            List<Vector3> points = vertices[i];
            Debug.Log("Current Region " + i + " Total Points " + points.Count);
            for(int j = 1; j < points.Count; j++)
            {
                triangles.Add(totalPointsSoFar + j);
                triangles.Add(totalPointsSoFar);
                if (j == points.Count - 1)
                {
                    triangles.Add(totalPointsSoFar+1);
                }
                else
                {
                    triangles.Add(totalPointsSoFar + j + 1);
                }
            }

            totalPointsSoFar += points.Count;
            Debug.Log("Total Points So Far: " + totalPointsSoFar);

            ////Generate the first triangle
            //triangles.Add(i);
            //triangles.Add(0);
            //if(i == vertices.Count-1)
            //{
            //    triangles.Add(1);
            //}
            //else
            //{
            //    triangles.Add(i + 1);
            //}
        }

        Debug.Log("Total Triangles: " + triangles.Count);
        return triangles;
    }

    List<Color> GenerateColourRegions(List<List<Vector3>> vertices)
    {

        List<Color> colourRegions = new List<Color>();

        for (int i = 0; i < vertices.Count; i++)
        {
            List<Vector3> points = vertices[i];
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            for (int j = 0; j < points.Count; j++)
            {
                colourRegions.Add(color);
            }


        }
        return colourRegions;
    }

}
