using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using Vector2 = System.Numerics.Vector2;
using UnityEditor;



public class VoronoiMeshGenerator : MonoBehaviour
{
    
    public VoronoiDiagram voronoiDiagram;

    public GameObject pathNode;

    public List<GameObject> pathNodeObjects;

    public List<List<Vector3>> regionPlotPoints;

    public GameObject Floor;
    public GameObject Walls;
    public GameObject PathNodeStorage;
    public GameObject TestingLinePoint;
    public GameObject PlayerSpawner;

    public PerlinNoise perlinTexture;


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

    public void GenerateMesh()
    {
        //Randomizes the perlin texture for terrain transformation
        perlinTexture.RandomizePerlinTexture();
        //Spawns nodes for the main path of the level
        SpawnPathNodes(voronoiDiagram.PathFindingAStar(voronoiDiagram.voronoi));
        //Generates the plane for the level
        GeneratePlane(voronoiDiagram.voronoi);

        WidenPath();

        //generates the walls for the level
        GenerateWalls();
        //Move the player spawner
        PlayerSpawner.transform.position = pathNodeObjects[pathNodeObjects.Count-1].transform.position;
        //Spawn the player
        PlayerSpawner.GetComponentInChildren<SpawnPlayer>().Spawn();
    }

    public void GeneratePlane(Voronoi tempVoronoi)
    {
        //Set up the mesh to be generated
        Mesh mesh = new Mesh();
        Floor.GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        //Generate a list of vectors to represent the regions of the voronoi noise
        List<List<Vector3>> regionPlotPoints = GenerateVertices(tempVoronoi);

        //Generate colours for each unique region for the mesh
        mesh.colors = GenerateColourRegions(regionPlotPoints).ToArray();

        Debug.Log("Path Nodes: " + pathNodeObjects.Count);      
        if (pathNodeObjects.Count > 0)
        {
            Debug.Log("It Got here");
            regionPlotPoints = MoveVertices(regionPlotPoints);
        }

        //Set up the vertices for the plane into a singular list to be used for the mesh
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

        //apply the perlin noise to the plane
        vertices = ApplyPerlinNoise(vertices);

        //Debug.Log("Total Vertices: " + vertices.Count);
        //Set the vertices for the mesh
        mesh.vertices = vertices.ToArray();

        //Speed storage used to move the vertices up and down
        speed = new float[mesh.vertices.Length];
        
        List<Vector3> normals = new List<Vector3>();
        //Se up normals for the vertices
        for(int i = 0; i < vertices.Count; i++)
        {
            normals.Add(Vector3.up);
        }
        //Set up each of the mesh data
        mesh.normals = normals.ToArray();
        //Set up the triangles for mesh generation
        mesh.triangles = GenerateTriangles(regionPlotPoints).ToArray();
        mesh.colors = GenerateColourRegions(regionPlotPoints).ToArray();

        //Set the mesh collider for the plane
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
        Vector3[] vertices =  Floor.GetComponent<MeshFilter>().mesh.vertices;
    
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

        Floor.GetComponent<MeshCollider>().sharedMesh = Floor.GetComponent<MeshFilter>().mesh;
        Floor.GetComponent<MeshFilter>().mesh.vertices = vertices;
        Floor.GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }

    List<List<Vector3>> GenerateVertices(Voronoi voronoi)
    {    
        //Gets all of the regions vectors for the voronoi diagram
        List<Vector2> siteCoords = voronoi.SiteCoords();
        //Storage used to seperate out all the vectors into regions
        List<List<Vector3>> regionPlotPoints = new List<List<Vector3>>();

        int totalPointsSoFar = 0;

        for (int i = 0; i < siteCoords.Count; i++)
        {
            int randomNum = Random.Range(0, 10);
            List<Vector3> points = new List<Vector3>();
            //Add the center of the region vertic first
            points.Add(new Vector3(siteCoords[i].X / scaling, randomizeHeights ? 0 : 0, siteCoords[i].Y / scaling));
            //Get list of all vectors in the region
            List<Vector2> plotPoints = voronoi.Region(siteCoords[i]);
            //Store all the points in the region
            for (int j = 0; j < plotPoints.Count; j++)
            {
                points.Add(new Vector3(plotPoints[j].X / scaling, randomizeHeights ? 0 : 0, plotPoints[j].Y / scaling));
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
        //List of triangles
        List<int> triangles = new List<int>();
        int totalPointsSoFar = 0;


        for(int i = 0; i < vertices.Count; i++)
        {
            //list of points for a region
            List<Vector3> points = vertices[i];

            //Gets the triangles for each region
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

            bool checkForValid = true;
            for (int k = 0; k < pathNodeObjects.Count; k++)
            {
                
                if (points[0] == new Vector3(pathNodeObjects[k].transform.localPosition.x,0 ,pathNodeObjects[k].transform.localPosition.z))
                {
                    checkForValid = false;
                    for (int j = 0; j < points.Count; j++)
                    {
                        colourRegions.Add(Color.yellow);
                    }
                    break;
                }

            }
            if (!checkForValid) continue;


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
        for (var i = PathNodeStorage.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(PathNodeStorage.transform.GetChild(i).gameObject);
        }

        pathNodeObjects = new List<GameObject>();
        //Spawns in the first node
        PathNode previousNode = Instantiate(pathNode, PathNodeStorage.transform).GetComponent<PathNode>();
        //Set the position for the node
        previousNode.transform.localPosition = new Vector3(sites[sites.Count - 1].Coord.X / scaling, perlinTexture.perlinTexture.GetPixel((int)sites[sites.Count - 1].Coord.X, (int)sites[sites.Count - 1].Coord.Y).r * 10f, sites[sites.Count - 1].Coord.Y / scaling);
        //Add node to list of nodes
        pathNodeObjects.Add(previousNode.gameObject);
        
        //Spawn rest of the nodes
        for (int i = sites.Count-2; i > 0; i--)
        {
            //Spawns the next node
            PathNode nextNode = Instantiate(pathNode, PathNodeStorage.transform).GetComponent<PathNode>();
            //Sets position of node
            nextNode.transform.localPosition = new Vector3(sites[i].Coord.X / scaling, perlinTexture.perlinTexture.GetPixel((int)sites[i].Coord.X, (int)sites[i].Coord.Y).r * 10f, sites[i].Coord.Y / scaling);
            //Store node
            pathNodeObjects.Add(nextNode.gameObject);
            //Sets up the rotation of the node
            nextNode.transform.rotation = Quaternion.LookRotation(previousNode.transform.position - nextNode.transform.position, Vector3.up);
            //Sets the next node to connect to the previous node
            nextNode.NextNode = previousNode;
            //use the next node as the previous node
            previousNode = nextNode;
        }

        
    }
    //This is for fun used to move vertices seperatley from each other
    public List<List<Vector3>> MoveVertices(List<List<Vector3>> vertces)
    {

        for (int i = 0; i < vertces.Count; i++)
        {

            for (int j = 0; j < pathNodeObjects.Count; j++)
            {
                
                if (vertces[i][0] == new Vector3(pathNodeObjects[j].transform.localPosition.x, 0, pathNodeObjects[j].transform.localPosition.z))
                {
                    for (int k = 0; k < vertces[i].Count; k++)
                    {
                        vertces[i][k] = new Vector3(vertces[i][k].x, pathNodeObjects[j].transform.localPosition.y, vertces[i][k].z);
                    }
                }
            }
        }
        return vertces;
    }

    //Generate vertices for the wall
    public List<Vector3> GenerateWallVertices()
    {
        List<Vector3> points = new List<Vector3>();

        //Loops through all path nodes to find each region vector
        for (int i = 0; i < pathNodeObjects.Count; i++)
        {
            //Get all the connected lines attached to the region
            List<LineSegment> connectedLines = voronoiDiagram.voronoi.VoronoiBoundarayForSite(new Vector2(pathNodeObjects[i].transform.localPosition.x * scaling, pathNodeObjects[i].transform.localPosition.z * scaling));
            //Gets all neigbours attached to the current region
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
                    for(int l = 0; l < connectedLines.Count; l++)
                    {
                        Debug.Log("LOTS AND LOTS OF LINES");

                        if (neighboursLines[k].p0 == connectedLines[l].p0 && neighboursLines[k].p1 == connectedLines[l].p1)
                        {

                            //Direction from point p1 to p0
                            Vector2 direction = connectedLines[l].p0 - connectedLines[l].p1;
                            //Direction from the midpoint of p1 and p0 to the center of the region
                            Vector2 directionToCenter = new Vector2(pathNodeObjects[i].transform.localPosition.x * scaling, pathNodeObjects[i].transform.localPosition.z * scaling)-(connectedLines[l].p1 +direction/2);
                            //Normalize directions
                            direction = Vector2.Normalize(direction);
                            directionToCenter = Vector2.Normalize(directionToCenter);
                            //Find the cross product to see if the point is to the left and right
                            Vector3 temp = Vector3.Cross(new Vector3(direction.X, 0, direction.Y),new Vector3(directionToCenter.X, 0, directionToCenter.Y));
                            
                            //Checks to find if p0 is the left vector to generate mesh the right way around
                            if (temp.y <0)
                            {
                                Debug.Log("WE Got Some WALLS");
                                points.Add(new Vector3(connectedLines[l].p0.X / scaling, 0, connectedLines[l].p0.Y / scaling));
                                points.Add(new Vector3(connectedLines[l].p1.X / scaling, 0, connectedLines[l].p1.Y / scaling));
                                points.Add(new Vector3(connectedLines[l].p0.X / scaling, 10, connectedLines[l].p0.Y / scaling));
                                points.Add(new Vector3(connectedLines[l].p1.X / scaling, 10, connectedLines[l].p1.Y / scaling));
                            }
                            else
                            {
                                Debug.Log("WE Got Some WALLS");
                                points.Add(new Vector3(connectedLines[l].p1.X / scaling, 0, connectedLines[l].p1.Y / scaling));
                                points.Add(new Vector3(connectedLines[l].p0.X / scaling, 0, connectedLines[l].p0.Y / scaling));
                                points.Add(new Vector3(connectedLines[l].p1.X / scaling, 10, connectedLines[l].p1.Y / scaling));
                                points.Add(new Vector3(connectedLines[l].p0.X / scaling, 10, connectedLines[l].p0.Y / scaling));
                            }
                          
                            

                        }
                    }
                    
                }
                
            }
        }

        return points;
    }

    public void WidenPath()
    {
        List<GameObject> pathNodeExtras = new List<GameObject>();
        List<GameObject> finalExtras = new List<GameObject>();
        pathNodeExtras.AddRange(pathNodeObjects);
        for(int i = 0; i < pathNodeObjects.Count; i++)
        {
            List<Vector2> neighbours = voronoiDiagram.voronoi.NeighborSitesForSite(new Vector2(pathNodeObjects[i].transform.localPosition.x*scaling, pathNodeObjects[i].transform.localPosition.z*scaling));

            for(int j = 0; j < neighbours.Count; j++)
            {
                bool valid = true;
                for(int k = 0; k < pathNodeExtras.Count; k++)
                {

                    if (neighbours[j] == new Vector2(pathNodeExtras[k].transform.localPosition.x * scaling, pathNodeExtras[k].transform.localPosition.z * scaling))
                    {
                        valid = false;
                    }
                    
                }

                if (valid == false) continue;

                GameObject node = Instantiate<GameObject>(pathNode, PathNodeStorage.transform);
                node.transform.localPosition = new Vector3(neighbours[j].X / scaling, 0 ,neighbours[j].Y / scaling);

                pathNodeExtras.Add(node);
                finalExtras.Add(node);
            }

        }

        pathNodeObjects.AddRange(finalExtras);
    }

    //Get triangles for wall generation
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

    List<Vector3> ApplyPerlinNoise(List<Vector3> vertices)
    {
        
        for(int i = 0; i < vertices.Count; i++)
        {
            float height = perlinTexture.perlinTexture.GetPixel((int)(vertices[i].x * scaling), (int)(vertices[i].z*scaling)).r;
            vertices[i] = new Vector3(vertices[i].x, height * 10, vertices[i].z);
        }

        return vertices;
    }

}

[CustomEditor(typeof(VoronoiMeshGenerator))]
public class VoronoiMeshGeneratorEditor : Editor
{

    // some declaration missing??

    override public void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            GUILayout.TextField("Dont Press Button To Much Bad Idea");

            VoronoiMeshGenerator colliderCreator = (VoronoiMeshGenerator)target;
            if (GUILayout.Button("Generate New Level Mesh"))
            {
                colliderCreator.GenerateMesh(); // how do i call this?
            }
        }
        DrawDefaultInspector();
    }
}


