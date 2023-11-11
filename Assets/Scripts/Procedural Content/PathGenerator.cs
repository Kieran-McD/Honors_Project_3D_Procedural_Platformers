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
    public bool isFinished;

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
        tick += 1 * Mathf.Abs(((float)currentWorm.GetComponent<PerlinWormPath>().turnValue)) * Time.deltaTime * 20f;

        if (tick > pointSpawnRate)
        {
            pathPointTransforms.Add(Instantiate<GameObject>(pathPointPrefab, currentWorm.position, currentWorm.rotation, transform).transform);
            tick = 0;
        }

        if(pathPointTransforms.Count > pointsInPath)
        {
            Destroy(currentWorm.gameObject);
            BuildPath();
            isFinished = true;
        }
    }


    void BuildPath()
    {
        
        currentPath = Instantiate(pathPrefab);
        currentPath.GetComponent<Path>().exit.position = pathPointTransforms[pathPointTransforms.Count-1].position;
        currentPath.GetComponent<Path>().exit.rotation = pathPointTransforms[pathPointTransforms.Count - 1].rotation;
        MeshFilter currentMeshFilter = currentPath.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        currentMeshFilter.mesh = mesh;

        mesh.Clear();
        List<Vector3>vertices = GenerateVertices();
        mesh.vertices = vertices.ToArray();
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
        currentWorm = null;
        currentWorm = Instantiate(perlinWormPrefab, pos, rot).transform;
        pathPointTransforms.Clear();
        pathPointTransforms.Add(Instantiate<GameObject>(pathPointPrefab, pos, rot, transform).transform);
        pointSpawnRate = currentWorm.GetComponent<PerlinWormPath>().speed / (currentWorm.GetComponent<PerlinWormPath>().speed * currentWorm.GetComponent<PerlinWormPath>().speed);
        pointsInPath = pathPoints;
    }
    public GameObject GetCurrentPath()
    {
        return currentPath;
    }
}
