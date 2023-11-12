using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject perlinWormPrefab;
    [SerializeField]
    GameObject pathPointPrefab;
    [SerializeField]
    GameObject pathPrefab;

    GameObject currentPath;

    Transform currentWorm;
    
    List<Transform> pathPointTransforms = new List<Transform>();

    float tick;
    [SerializeField]
    float pointSpawnRate = 1f;
    [SerializeField]
    float pathWidth = 4f;

    private bool isFinished;

    private int pointsInPath;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWorm.IsUnityNull() || isFinished == true) return;
        tick += Time.deltaTime;
        tick += 1 * Mathf.Abs(((float)currentWorm.GetComponent<PerlinWormPath>().turnValue)) * Time.deltaTime;

        if (tick > pointSpawnRate)
        {
            pathPointTransforms.Add(Instantiate<GameObject>(pathPointPrefab, currentWorm.position, currentWorm.rotation, transform).transform);
            tick = 0;
        }

        if(pathPointTransforms.Count > pointsInPath)
        {
            //Destroy(currentWorm.gameObject);
            currentWorm.GetComponent<PerlinWormPath>().isMoving = false;
            BuildPath();
            isFinished = true;
        }
    }


    void BuildPath()
    {
        //Spawns the path object
        currentPath = Instantiate(pathPrefab);
        //Sets the exit for the path to the the position of the final point generated
        currentPath.GetComponent<Path>().exit.position = pathPointTransforms[pathPointTransforms.Count-1].position;
        currentPath.GetComponent<Path>().exit.rotation = pathPointTransforms[pathPointTransforms.Count - 1].rotation;
        //Sets up the mesh filter and mesh
        MeshFilter currentMeshFilter = currentPath.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        currentMeshFilter.mesh = mesh;

        mesh.Clear();
        //Generates the vertices
        List<Vector3>vertices = GenerateVertices();
        mesh.vertices = vertices.ToArray();
        mesh.normals = GenerateNormals().ToArray();
        //Sets up the indices
        mesh.triangles = GenerateTriangles(vertices).ToArray();


        currentPath.GetComponent<MeshCollider>().sharedMesh = mesh;

        
    }

    List<Vector3> GenerateVertices()
    {
        List<Vector3> vertices = new List<Vector3>();

        //Generate all the vertices along the point
        for(int i = 0; i < pathPointTransforms.Count; i++)
        {
            Vector3 pos = pathPointTransforms[i].position;
            Vector3 right = pathPointTransforms[i].right;
            vertices.Add(pos - (right * pathWidth/2f));
            Instantiate<GameObject>(pathPointPrefab, vertices[vertices.Count-1], Quaternion.identity, transform);
            vertices.Add(pos + (right * pathWidth / 2f));
            Instantiate<GameObject>(pathPointPrefab, vertices[vertices.Count - 1], Quaternion.identity, transform);
        }
        return vertices;        
    }

    List<Vector3> GenerateNormals()
    {
        List<Vector3> normals = new List<Vector3>();

        //Generate all the vertices along the point
        for (int i = 0; i < pathPointTransforms.Count; i++)
        {
            normals.Add(pathPointTransforms[i].up);           
            normals.Add(pathPointTransforms[i].up);          
        }
        return normals;
    }

    //If Vertices generation changes, change triangle generation
    List<int> GenerateTriangles(List<Vector3> vertices)
    {
        List<int> triangles = new List<int>();

        for(int i = 0; i < pathPointTransforms.Count-1; i++)
        {
            //Generate the first triangle
            triangles.Add(i * 2 );
            triangles.Add(i * 2 + 3);
            triangles.Add(i * 2 + 1);

            //Generate the second triangle
            triangles.Add(i * 2 );
            triangles.Add(i * 2 + 2);
            triangles.Add(i * 2 + 3);
        }

        return triangles;
    }

    public void StartPathGenerator(Vector3 pos, Quaternion rot)
    {
        StartPathGenerator(pos, rot, 20);
    }

    public void StartPathGenerator(Vector3 pos, Quaternion rot, int pathPoints)
    {
        isFinished = false;
        tick = 0;
        //CURRENTLY DECIDING IF CURRENT WORM SHOULD BE REPLACED OR REUSED
        //REPLACE
        //currentWorm = null;
        //currentWorm = Instantiate(perlinWormPrefab, pos, rot).transform;
        //REUSE
        if (currentWorm == null)
            currentWorm = Instantiate(perlinWormPrefab, pos, rot).transform;
        currentWorm.position = pos;
        currentWorm.rotation = rot;
        currentWorm.GetComponent<PerlinWormPath>().isMoving = true;

        pathPointTransforms.Clear();
        pathPointTransforms.Add(Instantiate<GameObject>(pathPointPrefab, pos, rot, transform).transform);
        pointSpawnRate = currentWorm.GetComponent<PerlinWormPath>().speed / (currentWorm.GetComponent<PerlinWormPath>().speed * currentWorm.GetComponent<PerlinWormPath>().speed);
        pointsInPath = pathPoints;
    }
    public GameObject GetCurrentPath()
    {
        return currentPath;
    }

    public Transform GetCurrentWorm()
    {
        return currentWorm;
    }

    public void ClearPathGenerator()
    {
        bool skipFirst = false;
        foreach(Transform child in transform)
        {
            if (!skipFirst)
            {
                skipFirst = true;
                continue;
            }
            Destroy(child.gameObject);
        }
        if (currentWorm) Destroy(currentWorm.gameObject);
        currentWorm = null;
    }

    public bool GetIsFinished()
    {
        return isFinished;
    }
}
