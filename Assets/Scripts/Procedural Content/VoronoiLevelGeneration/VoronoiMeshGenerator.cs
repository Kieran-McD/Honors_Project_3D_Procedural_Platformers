using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using Vector2 = System.Numerics.Vector2;
using UnityEditor;
using UnityEngine.Networking.PlayerConnection;
using Unity.VisualScripting;


public class VoronoiMeshGenerator : MonoBehaviour
{
    
    public VoronoiDiagram voronoiDiagram;

    public GameObject pathNode;

    public List<GameObject> pathNodeObjects;

    public List<List<Vector3>> regionPlotPoints;

    public GameObject Floor;
    public GameObject Walls;
    public GameObject TestingLinePoint;


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
        Floor.GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        regionPlotPoints = new List<List<Vector3>>();

        regionPlotPoints = GenerateVertices(voronoiDiagram.voronoi);
      

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

    public void GenerateMesh(Voronoi tempVoronoi)
    {
        GeneratePlane(tempVoronoi);
        GenerateWalls();
    }

    public void GeneratePlane(Voronoi tempVoronoi)
    {
        Mesh mesh = new Mesh();
        Floor.GetComponent<MeshFilter>().mesh = mesh;
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

        
        Floor.GetComponent<MeshCollider>().sharedMesh = mesh;
        mesh.RecalculateNormals();
    }

    private void GenerateWalls()
    {
        Mesh mesh = new Mesh();
        Walls.GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        List<Vector3> vertices = GenerateWallVertices();

        mesh.vertices = vertices.ToArray();

        List<Vector3> normals = new List<Vector3>();

        for (int i = 0; i < vertices.Count; i++)
        {
            normals.Add(Vector3.up);
        }

        mesh.normals = normals.ToArray();
        mesh.triangles = GenerateWallTriangles(vertices).ToArray();

        Walls.GetComponent<MeshCollider>().sharedMesh = mesh;
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
        previousNode.transform.localPosition = new Vector3(sites[sites.Count - 1].Coord.X / scaling,Random.Range(0,10), sites[sites.Count - 1].Coord.Y / scaling);
        pathNodeObjects.Add(previousNode.gameObject);
        for (int i = sites.Count-2; i > 0; i--)
        {
            PathNode nextNode = Instantiate(pathNode, this.transform).GetComponent<PathNode>();
            nextNode.transform.localPosition = new Vector3(sites[i].Coord.X / scaling, Random.Range(0, 10), sites[i].Coord.Y / scaling);
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
                
                if (vertces[i][0] == new Vector3(pathNodeObjects[j].transform.localPosition.x, 0, pathNodeObjects[j].transform.localPosition.z))
                {
                    for (int k = 0; k < vertces[i].Count; k++)
                    {
                        Debug.Log("MERJFHBESHAJNC ");
                        vertces[i][k] = new Vector3(vertces[i][k].x, pathNodeObjects[j].transform.localPosition.y, vertces[i][k].z);
                    }
                }
            }
        }
        return vertces;
    }

    public List<Vector3> GenerateWallVertices()
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < pathNodeObjects.Count; i++)
        {
            Debug.Log("AHHHHHHHHHHHHHHHHHHHHHHHHHH");
            List<LineSegment> connectedLines = voronoiDiagram.voronoi.VoronoiBoundarayForSite(new Vector2(pathNodeObjects[i].transform.localPosition.x * scaling, pathNodeObjects[i].transform.localPosition.z * scaling));
            List<Vector2> Neighbours = voronoiDiagram.voronoi.NeighborSitesForSite(new Vector2(pathNodeObjects[i].transform.localPosition.x * scaling, pathNodeObjects[i].transform.localPosition.z * scaling));
            for (int j = 0; j < Neighbours.Count; j++)
            {
                bool checkForValid = true;
                for (int k = 0; k < pathNodeObjects.Count; k++)
                {
                    Debug.Log("OH Yeah Thats How its Done");
                    if (Neighbours[j] == new Vector2(pathNodeObjects[k].transform.localPosition.x*scaling, pathNodeObjects[k].transform.localPosition.z*scaling))
                    {                     
                        checkForValid = false;
                        break;
                    }

                }
                if (!checkForValid) continue;


                List<LineSegment> neighboursLines = voronoiDiagram.voronoi.VoronoiBoundarayForSite(Neighbours[j]);

                for (int k = 0; k < neighboursLines.Count; k++)
                {
                    Debug.Log("Some Lines");
                    for(int w = 0; w < connectedLines.Count; w++)
                    {
                        Debug.Log("LOTS AND LOTS OF LINES");

                        if (neighboursLines[k].p0 == connectedLines[w].p0 && neighboursLines[k].p1 == connectedLines[w].p1)
                        {


                            Vector2 direction = connectedLines[w].p0 - connectedLines[w].p1;
                            Vector2 midPoint = connectedLines[w].p1 + direction;
                            Vector2 directionToCenter = new Vector2(pathNodeObjects[i].transform.localPosition.x * scaling, pathNodeObjects[i].transform.localPosition.z * scaling)-(connectedLines[w].p1 +direction/2);
                             direction = Vector2.Normalize(direction);
                            directionToCenter = Vector2.Normalize(directionToCenter);
                            Vector3 temp = Vector3.Cross(new Vector3(direction.X, 0, direction.Y),new Vector3(directionToCenter.X, 0, directionToCenter.Y));
                            if (temp.y <0)
                            {
                                Debug.Log("WE Got Some WALLS");
                                points.Add(new Vector3(connectedLines[w].p0.X / scaling, 0, connectedLines[w].p0.Y / scaling));
                                points.Add(new Vector3(connectedLines[w].p1.X / scaling, 0, connectedLines[w].p1.Y / scaling));
                                points.Add(new Vector3(connectedLines[w].p0.X / scaling, 10, connectedLines[w].p0.Y / scaling));
                                points.Add(new Vector3(connectedLines[w].p1.X / scaling, 10, connectedLines[w].p1.Y / scaling));
                            }
                            else
                            {
                                Debug.Log("WE Got Some WALLS");
                                points.Add(new Vector3(connectedLines[w].p1.X / scaling, 0, connectedLines[w].p1.Y / scaling));
                                points.Add(new Vector3(connectedLines[w].p0.X / scaling, 0, connectedLines[w].p0.Y / scaling));
                                points.Add(new Vector3(connectedLines[w].p1.X / scaling, 10, connectedLines[w].p1.Y / scaling));
                                points.Add(new Vector3(connectedLines[w].p0.X / scaling, 10, connectedLines[w].p0.Y / scaling));
                            }
                          
                            

                        }
                    }
                    
                }
                
            }
        }

        return points;
    }

    List<int>  GenerateWallTriangles(List<Vector3> vertices)
    {
        List<int> triangles = new List<int>();

        for (int i = 0; i < vertices.Count; i+=4)
        {
            triangles.Add(i + 2);
            triangles.Add(i + 1);
            triangles.Add(i);

            triangles.Add(i + 3);
            triangles.Add(i + 1);
            triangles.Add(i + 2);



        }

        return triangles;
    }
}



