using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Vector2 = System.Numerics.Vector2;
using csDelaunay;
using System.Collections;

public class VoronoiDiagram : MonoBehaviour
{

    // The number of polygons/sites we want
    [Range(5, 1000)]
    public int polygonNumber = 200;

    // This is where we will store the resulting data
    private Dictionary<Vector2, Site> sites;
    private List<Edge> edges;
    public List<Site> pointsForPath;

    public Voronoi voronoi;
    public VoronoiMeshGenerator meshGenerator;

    void Start()
    {
       CreateDiagram();
        if (meshGenerator)
        {
            meshGenerator.SpawnPathNodes(pointsForPath);
            meshGenerator.GenerateMesh(voronoi);
        }
    }


    public void CreateDiagram()
    {
        // Create your sites (lets call that the center of your polygons)
        List<Vector2> points = CreateRandomPoint();

        // Create the bounds of the voronoi diagram
        // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
        // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
        Rectf bounds = new Rectf(0, 0, 512, 512);

        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        voronoi = new Voronoi(points, bounds, 5);

        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);

        sites = new Dictionary<Vector2, Site>();
        edges = new List<Edge>();
        pointsForPath = new List<Site>();


        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;
        pointsForPath = PathFindingAStar(voronoi);

        DisplayVoronoiDiagram();

    }
        IEnumerator waiter()
    {
        //Wait for 4 seconds
        yield return new WaitForSeconds(0.2f);
    }


    private List<Vector2> CreateRandomPoint()
    {

        // Use Vector2f, instead of Vector2
        // Vector2f is pretty much the same than Vector2, but like you could run Voronoi in another thread
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < polygonNumber; i++)
        {
            points.Add(new Vector2(Random.Range(0, 512), Random.Range(0, 512)));
        }

        return points;
    }

    // Here is a very simple way to display the result using a simple bresenham line algorithm
    // Just attach this script to a quad
    private void DisplayVoronoiDiagram()
    {
        Texture2D tx = new Texture2D(512, 512);
        foreach (KeyValuePair<Vector2, Site> kv in sites)
        {
            tx.SetPixel((int)kv.Key.X, (int)kv.Key.Y, Color.red);
        }
        foreach (Edge edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;

            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }

        foreach(Vector2 site in voronoi.SiteCoords())
        {
            foreach(Vector2 otherSite in voronoi.NeighborSitesForSite(site))
            {
                DrawLine(site, otherSite, tx, Color.blue);
            }
        }

        for(int i =0; i< pointsForPath.Count-1; i++)
        {
            DrawLine(pointsForPath[i].Coord, pointsForPath[i + 1].Coord, tx, Color.magenta);
        }
       

        tx.Apply();

        this.GetComponent<Renderer>().material.mainTexture = tx;
    }

    // Bresenham line algorithm
    private void DrawLine(Vector2 p0, Vector2 p1, Texture2D tx, Color c, int offset = 0)
    {
        int x0 = (int)p0.X;
        int y0 = (int)p0.Y;
        int x1 = (int)p1.X;
        int y1 = (int)p1.Y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tx.SetPixel(x0 + offset, y0 + offset, c);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }


    public List<Site> PathFindingAStar(Voronoi voronoi)
    {
        Site startingSite, endSite;
        voronoi.SitesIndexedByLocation.TryGetValue(voronoi.SiteCoords()[Random.Range(0, voronoi.SiteCoords().Count)], out startingSite);
        voronoi.SitesIndexedByLocation.TryGetValue(voronoi.SiteCoords()[Random.Range(0, voronoi.SiteCoords().Count)], out endSite);

        List<Site> result = new List<Site>();
        List<Site> visited = new List<Site>();
        Queue<Site> work = new Queue<Site>();

        startingSite.history = new List<Site>();
        visited.Add(startingSite);
        work.Enqueue(startingSite);

        while (work.Count > 0)
        {
            Site current = work.Dequeue();
            if (current == endSite)
            {
                //Found Node
                result = current.history;
                result.Add(current);
                return result;
            }
            else
            {
                //Didn't find Node
                for (int i = 0; i < current.NeighborSites().Count; i++)
                {
                    Site currentNeighbor = current.NeighborSites()[i];
                    if (!visited.Contains(currentNeighbor))
                    {
                        currentNeighbor.history = new List<Site>(current.history);
                        currentNeighbor.history.Add(current);
                        visited.Add(currentNeighbor);
                        work.Enqueue(currentNeighbor);
                    }
                }
            }
        }
        //Route not found, loop ends
        return null;
    }

    public List<PathNode> GetPathList(float Scaling)
    {

        for(int i = 0; i < pointsForPath.Count; i++)
        {
            
        }


        return new List<PathNode>();
    }

}


[CustomEditor(typeof(VoronoiDiagram))]
public class VoronoiDiagramEditor : Editor
{

    // some declaration missing??

    override public void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            GUILayout.TextField("Dont Press Button To Much Bad Idea");

            VoronoiDiagram colliderCreator = (VoronoiDiagram)target;
            if (GUILayout.Button("Create Diagram"))
            {
                colliderCreator.CreateDiagram(); // how do i call this?
            }
        }    
        DrawDefaultInspector();
    }
}

