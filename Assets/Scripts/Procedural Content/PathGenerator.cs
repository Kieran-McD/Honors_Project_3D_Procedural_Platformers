using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PathGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject perlinWormPrefab;
    [SerializeField]
    GameObject pathPointPrefab;
    [SerializeField]
    GameObject pathPrefab;
    [SerializeField]
    public Transform pathGenratorStartPoint;

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
        //UpdateWorm();
    }

    public void UpdateWorm()
    {
        if (currentWorm.IsUnityNull() || isFinished == true) return;
        currentWorm.GetComponent<PerlinWorm>().PerlinWormLogic();
        tick += 1;
        tick += 1 * Mathf.Abs(((float)currentWorm.GetComponent<PerlinWormPath>().turnValue));

        Debug.DrawRay(currentWorm.position + Vector3.up * 2, Vector3.down);
        if (tick > pointSpawnRate)
        {
            Transform currentPoint = Instantiate<GameObject>(pathPointPrefab, currentWorm.position, currentWorm.rotation, transform).transform;
            pathPointTransforms.Add(currentPoint);
            tick = 0;
        }

        if (pathPointTransforms.Count > pointsInPath)
        {
            PerlinWormFinished();
        }
    }

    public void PerlinWormFinished()
    {
        //Destroy(currentWorm.gameObject);
        currentWorm.GetComponent<PerlinWormPath>().isMoving = false;
        BuildPath();
        isFinished = true;
    }

    void BuildPath()
    {
        //Spawns the path object
        currentPath = Instantiate(pathPrefab);
        //Sets the exit for the path to the the position of the final point generated
        currentPath.GetComponent<Path>().exit.position = pathPointTransforms[pathPointTransforms.Count - 1].position;
        currentPath.GetComponent<Path>().exit.rotation = pathPointTransforms[pathPointTransforms.Count - 1].rotation;
        //Sets up the mesh filter and mesh
        MeshFilter currentMeshFilter = currentPath.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        currentMeshFilter.mesh = mesh;

        mesh.Clear();
        //Generates the vertices
        List<Vector3> vertices = GenerateVertices();
        mesh.vertices = vertices.ToArray();
        mesh.normals = GenerateNormals(vertices).ToArray();
        //Sets up the indices
        mesh.triangles = GenerateTriangles(vertices).ToArray();


        currentPath.GetComponent<MeshCollider>().sharedMesh = mesh;


    }

    List<Vector3> GenerateVertices()
    {
        List<Vector3> vertices = new List<Vector3>();

        Vector3 pos = new Vector3();
        Vector3 right = new Vector3();

        //Generate all the vertices along the point
        for (int i = 0; i < pathPointTransforms.Count; i++)
        {
            pos = pathPointTransforms[i].position;
            right = pathPointTransforms[i].right;
            vertices.Add(pos - (right * pathWidth / 2f));
            Instantiate<GameObject>(pathPointPrefab, vertices[vertices.Count - 1], Quaternion.identity, transform);
            vertices.Add(pos + (right * pathWidth / 2f));
            Instantiate<GameObject>(pathPointPrefab, vertices[vertices.Count - 1], Quaternion.identity, transform);

            pos = pos - new Vector3(0,0.5f,0);

            vertices.Add(pos + (right * pathWidth / 2f));
            vertices.Add(pos - (right * pathWidth / 2f));

            pos = pos + new Vector3(0, 0.5f, 0);

            vertices.Add(pos - (right * pathWidth / 2f));
            vertices.Add(pos + (right * pathWidth / 2f));

            pos = pos - new Vector3(0, 0.5f, 0);

            vertices.Add(pos + (right * pathWidth / 2f));
            vertices.Add(pos - (right * pathWidth / 2f));
        }

        //GENERATE VERTICES FOR FORWARD AND BACK VERTICES
        //Back Vertices
        pos = pathPointTransforms[0].position;
        right = pathPointTransforms[0].right;
        vertices.Add(pos - (right * pathWidth / 2f));
        vertices.Add(pos + (right * pathWidth / 2f));

        pos = pos - new Vector3(0, 0.5f, 0);

        vertices.Add(pos + (right * pathWidth / 2f));
        vertices.Add(pos - (right * pathWidth / 2f));

        //Forward Vertices
        pos = pathPointTransforms[pathPointTransforms.Count-1].position;
        right = pathPointTransforms[pathPointTransforms.Count - 1].right;
        vertices.Add(pos - (right * pathWidth / 2f));
        vertices.Add(pos + (right * pathWidth / 2f));

        pos = pos - new Vector3(0, 0.5f, 0);

        vertices.Add(pos + (right * pathWidth / 2f));
        vertices.Add(pos - (right * pathWidth / 2f));


        return vertices;
    }

    List<Vector3> GenerateNormals(List<Vector3> vertices)
    {
        List<Vector3> normals = new List<Vector3>();

        //Generate all the normals along the point
        for (int i = 0; i < pathPointTransforms.Count; i++)
        {
            normals.Add(pathPointTransforms[i].up);
            normals.Add(pathPointTransforms[i].up);

            normals.Add(-pathPointTransforms[i].up);
            normals.Add(-pathPointTransforms[i].up);

            normals.Add(-pathPointTransforms[i].right);
            normals.Add(pathPointTransforms[i].right);

            normals.Add(pathPointTransforms[i].right);
            normals.Add(-pathPointTransforms[i].right);
        }

        //GENERATE NORMALS FOR FORWAD AND BACK OF PATH
        normals.Add(-pathPointTransforms[0].forward);
        normals.Add(-pathPointTransforms[0].forward);
        normals.Add(-pathPointTransforms[0].forward);
        normals.Add(-pathPointTransforms[0].forward);
        normals.Add(pathPointTransforms[pathPointTransforms.Count-1].forward);
        normals.Add(pathPointTransforms[pathPointTransforms.Count-1].forward);
        normals.Add(pathPointTransforms[pathPointTransforms.Count - 1].forward);
        normals.Add(pathPointTransforms[pathPointTransforms.Count - 1].forward);
        return normals;
    }

    //If Vertices generation changes, change triangle generation
    List<int> GenerateTriangles(List<Vector3> vertices)
    {
        List<int> triangles = new List<int>();

        for (int i = 0; i < pathPointTransforms.Count - 1; i++)
        {

            //GENERATE TOP QAUD
            //Generate top first Triangle
            triangles.Add(i * 8);
            triangles.Add(i * 8 + 9);
            triangles.Add(i * 8 + 1);

            //Generate the second triangle
            triangles.Add(i * 8);
            triangles.Add(i * 8 + 8);
            triangles.Add(i * 8 + 9);

            //GENERATE BOTTOM QUAD
            //Generate the first triangle
            triangles.Add(i * 8 + 2);
            triangles.Add(i * 8 + 11);
            triangles.Add(i * 8 + 3);

            //Generate the second triangle
            triangles.Add(i * 8 + 2);
            triangles.Add(i * 8 + 10);
            triangles.Add(i * 8 + 11);

            //GENERATE LEFT QUAD
            //Generate the first triangle
            triangles.Add(i * 8 + 4);
            triangles.Add(i * 8 + 15);
            triangles.Add(i * 8 + 12);

            //Generate the second triangle
            triangles.Add(i * 8 + 4);
            triangles.Add(i * 8 + 7);
            triangles.Add(i * 8 + 15);

            //GENERATE Right QUAD
            //Generate the first triangle
            triangles.Add(i * 8 + 5);
            triangles.Add(i * 8 + 13);
            triangles.Add(i * 8 + 14);

            //Generate the second triangle
            triangles.Add(i * 8 + 5);
            triangles.Add(i * 8 + 14);
            triangles.Add(i * 8 + 6);
        }

        //GENERATE BACK QUAD
        int index = pathPointTransforms.Count;
        //Generate top first Triangle
        triangles.Add(index * 8);
        triangles.Add(index * 8 + 1);
        triangles.Add(index * 8 + 2);

        //Generate the second triangle
        triangles.Add(index * 8);
        triangles.Add(index * 8 + 2);
        triangles.Add(index * 8 + 3);

        //GENERATE FRONT QUAD
        //Generate top first Triangle
        triangles.Add(index * 8 + 4);
        triangles.Add(index * 8 + 6);
        triangles.Add(index * 8 + 5);

        //Generate the second triangle
        triangles.Add(index * 8 + 4);
        triangles.Add(index * 8 + 7);
        triangles.Add(index * 8 + 6);


        return triangles;
    }

    public void StartPathGenerator()
    {
        StartPathGenerator(pathGenratorStartPoint.position, pathGenratorStartPoint.rotation, int.MaxValue);

        if (pathGenratorStartPoint.GetComponent<PathNode>())
        {
            currentWorm.GetComponent<PerlinWormPath>().followPoint = pathGenratorStartPoint.GetComponent<PathNode>().NextNode.transform;
        }
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
        {
            currentWorm = Instantiate(perlinWormPrefab, pos, rot).transform;
            currentWorm.GetComponent<PerlinWormPath>().pathGenerator = this;
        }
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
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if (currentWorm) Destroy(currentWorm.gameObject);
        currentWorm = null;
    }

    public bool GetIsFinished()
    {
        return isFinished;
    }

    public float GetPathWidth()
    {
        return pathWidth;
    }
}
#if UNITY_EDITOR
    [CustomEditor(typeof(PathGenerator))]
    public class PathGeneratorEditor : Editor
    {

        // some declaration missing??

        override public void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                GUILayout.TextField("Dont Press Button To Much Bad Idea");

                PathGenerator colliderCreator = (PathGenerator)target;
                if (GUILayout.Button("Start Generation"))
                {
                    colliderCreator.StartPathGenerator(); // how do i call this?
                }
            }
            DrawDefaultInspector();
        }
    }

#endif
