using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using Vector2 = System.Numerics.Vector2;
using UnityEditor;


public class VoronoiMeshGenerator : MonoBehaviour
{
    
    public VoronoiDiagram voronoiDiagram;

    public GameObject pathNode;

    List<GameObject> pathNodeObjects;

    [SerializeField]
    float scaling = 1f;

    float[] speed;

    public bool moveVertices;
    public bool randomizeHeights;

    private void Update()
    {
        if(moveVertices)UpdateVertices();
    }

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
            int randomNum = Random.Range(0, 10);
            for (int j = 0; j < points.Count; j++)
            {
                vertices.Add(points[j]);
                //vertices.Add(new Vector3(points[j].x, randomNum, points[j].z));
                //Debug.Log("Total Vertices: " + vertices.Count);
            }
        }

        //Debug.Log("Total Vertices: " + vertices.Count);

        mesh.vertices = vertices.ToArray();
        speed = new float[mesh.vertices.Length];
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
        
        Debug.Log("Path Nodes: " + pathNodeObjects.Count);
        if (pathNodeObjects.Count > 0)
        {
            Debug.Log("It Got here");
            regionPlotPoints = MoveVertices(regionPlotPoints);
        }

        List<Vector3> vertices = new List<Vector3>();

        for(int i = 0; i < regionPlotPoints.Count; i++)
        {
            List<Vector3> points = regionPlotPoints[i];
            for(int j = 0; j < points.Count; j++)
            {
                vertices.Add(points[j]);
                //Debug.Log("Total Vertices: " + vertices.Count);
            }
        }

        //Debug.Log("Total Vertices: " + vertices.Count);

        mesh.vertices = vertices.ToArray();
        speed = new float[mesh.vertices.Length];
        List<Vector3> normals = new List<Vector3>();

        for(int i = 0; i < vertices.Count; i++)
        {
            normals.Add(Vector3.up);
        }
        mesh.normals = normals.ToArray();
        mesh.triangles = GenerateTriangles(regionPlotPoints).ToArray();
        mesh.colors = GenerateColourRegions(regionPlotPoints).ToArray();

        
        GetComponent<MeshCollider>().sharedMesh = mesh;
        mesh.RecalculateNormals();
    }

    void UpdateVertices()
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
    
        for(int i = 0; i < vertices.Length; i++)
        {
            if (speed[i] == 0f)
            {
                speed[i] = 5f;
            }

            if (speed[i] >0f && vertices[i].y > 10f)
            {
                speed[i] = -5f;
            }
            else if (speed[i] < 0f && vertices[i].y < 0f)
            {
                speed[i] = 5f;
            }
            vertices[i] = new Vector3(vertices[i].x, vertices[i].y + Time.deltaTime * speed[i], vertices[i].z);
        }

        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshFilter>().mesh.vertices = vertices;
        GetComponent<MeshFilter>().mesh.RecalculateNormals();
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
            int randomNum = Random.Range(0, 10);
            List<Vector3> points = new List<Vector3>();
            points.Add(new Vector3(siteCoords[i].X / scaling, randomizeHeights ? randomNum : 0, siteCoords[i].Y / scaling));

            List<Vector2> plotPoints = voronoi.Region(siteCoords[i]);

            for (int j = 0; j < plotPoints.Count; j++)
            {
                points.Add(new Vector3(plotPoints[j].X / scaling, randomizeHeights ? randomNum : 0, plotPoints[j].Y / scaling));
                //Debug.Log("Plot Point: " + (i * plotPoints.Count + j) + " Position: " + points[j]);
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
            //Debug.Log("Current Region " + i + " Total Points " + points.Count);
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
            //Debug.Log("Total Points So Far: " + totalPointsSoFar);

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

        //Debug.Log("Total Triangles: " + triangles.Count);
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

    public void SpawnPathNodes(List<Site> sites)
    {
        pathNodeObjects = new List<GameObject>();
        PathNode previousNode = Instantiate(pathNode, this.transform).GetComponent<PathNode>();
        previousNode.transform.localPosition = new Vector3(sites[sites.Count - 1].Coord.X / scaling, 0, sites[sites.Count - 1].Coord.Y / scaling);
        pathNodeObjects.Add(previousNode.gameObject);
        for (int i = sites.Count-2; i > 0; i--)
        {
            PathNode nextNode = Instantiate(pathNode, this.transform).GetComponent<PathNode>();
            nextNode.transform.localPosition = new Vector3(sites[i].Coord.X / scaling, 0, sites[i].Coord.Y / scaling);
            pathNodeObjects.Add(nextNode.gameObject);
            nextNode.transform.rotation = Quaternion.LookRotation(previousNode.transform.position - nextNode.transform.position, Vector3.up);
            
            nextNode.NextNode = previousNode;
            previousNode = nextNode;
        }

    }
    public List<List<Vector3>> MoveVertices(List<List<Vector3>> vertces)
    {

        for (int i = 0; i < vertces.Count; i++)
        {
            //Debug.Log("MERJFHBESHAJNC ");
            for (int j = 0; j < pathNodeObjects.Count; j++)
            {

                if (vertces[i][0] == pathNodeObjects[j].transform.localPosition)
                {
                    for (int k = 0; k < vertces[i].Count; k++)
                    {
                        Debug.Log("MERJFHBESHAJNC ");
                        vertces[i][k] = new Vector3(vertces[i][k].x, 10, vertces[i][k].z);
                    }
                }
            }
        }
        return vertces;
    }
}



