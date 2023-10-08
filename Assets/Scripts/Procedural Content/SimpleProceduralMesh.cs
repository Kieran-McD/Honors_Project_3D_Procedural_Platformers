using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SimpleProceduralMesh : MonoBehaviour
{
    Mesh myMesh;
    MeshFilter meshFilter;

    [SerializeField] Vector2 planeSize = new Vector2(1, 1);
    [SerializeField] int planeResolution;
    [SerializeField] float noiseScale = 1f;
    [SerializeField] float WaveSpeed;

    List<Vector3> vertices;
    List<Vector3> normals;
    List<Vector2> uvS;

    List<int> triangles;

    PerlinNoise perlinNoise;

    MeshCollider collider;

    private void Awake()
    {
        myMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = myMesh;
        perlinNoise = GetComponent<PerlinNoise>();
        collider = GetComponent<MeshCollider>();
    }

   


    private void ApplyPerlinNoise()
    {


        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 vertex = vertices[i];

            Vector2 uvPos;
            uvPos.x = uvS[i].x;
            uvPos.y = uvS[i].y;

            float yPos = perlinNoise.perlinTexture.GetPixelBilinear(uvPos.x , uvPos.y).r;
            float finalValue = 0;
            //for(float j = 0; j < yPos; j += 0.2f)
            //{
            //    finalValue += 0.2f;
            //}

            vertex.y = yPos * noiseScale;
            //vertex.y = finalValue * noiseScale;
            vertices[i] = vertex;
        }
    }

    private void GeneratePlane(Vector2 size, int resolution)
    {
        //Generate Vertices
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvS = new List<Vector2>();
        float xPerStep = size.x / resolution;
        float yPerStep = size.y / resolution;
        float xUVStep = 1 / (float)resolution;
        float yUVStep = 1 / (float)resolution;
        for (int y = 0; y< resolution+1; y++)
        {
            for(int x = 0; x < resolution+1; x++)
            {
                vertices.Add(new Vector3(x * xPerStep, 0, y * yPerStep));
                normals.Add(Vector3.up);
                uvS.Add(new Vector2(x * xUVStep, y*yUVStep));
            }
        }
        triangles = new List<int>();

        //Create triangles
        for(int row = 0; row<resolution; row++)
        {
            for (int column = 0; column < resolution; column++)
            {
                int i = (row * resolution) + row + column;
                //First Triangle
                triangles.Add(i);
                triangles.Add(i+(resolution)+1);
                triangles.Add(i + (resolution) + 2);
                //Second Triangle
                triangles.Add(i);
                triangles.Add(i + (resolution) + 2);
                triangles.Add(i + 1);
            }
        }
    }

    void SinWaveAnimation(float time)
    {
        for(int i = 0; i < vertices.Count; i++)
        {
            Vector3 vertex = vertices[i];

            vertex.y += Mathf.Sin(vertex.x + time);
            vertices[i] = vertex;
        }
    }

    private void AssignMesh()
    {
        myMesh.Clear();
        myMesh.vertices = vertices.ToArray();
        myMesh.triangles = triangles.ToArray();
        myMesh.normals = normals.ToArray();
        myMesh.uv = uvS.ToArray();
    }

    private void OnEnable()
    {
        //TestingCreationProceduralMesh();
        planeResolution = Mathf.Clamp(planeResolution, 1, 100);
        GeneratePlane(planeSize, planeResolution);
        ApplyPerlinNoise();
        //SinWaveAnimation(Time.time * WaveSpeed);
        AssignMesh();
        collider.sharedMesh = myMesh;
        
    }

    private void Update()
    {
        ApplyPerlinNoise();
        //SinWaveAnimation(Time.time * WaveSpeed);
        AssignMesh();
        collider.sharedMesh = myMesh;
    }

    private void TestingCreationProceduralMesh()
    {
        var mesh = new Mesh
        {
            name = "Procedural Mesh"
        };

        mesh.vertices = new Vector3[]
        {
            Vector3.zero, Vector3.right, Vector3.up, new Vector3 (1f,1f)
        };

        mesh.triangles = new int[] {
            0,2,1,1,2,3
        };


        mesh.normals = new Vector3[] {
            Vector3.back, Vector3.back, Vector3.back, Vector3.back
        };


        mesh.uv = new Vector2[] {
            Vector2.zero, Vector2.right, Vector2.up, Vector2.one
        };

        mesh.tangents = new Vector4[] {
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f)
        };

        GetComponent<MeshFilter>().mesh = mesh;
    }
}
