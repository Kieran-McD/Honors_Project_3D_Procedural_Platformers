using csDelaunay;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;
using Vector2 = System.Numerics.Vector2;

public class Grid : MonoBehaviour
{
    public List<PathNode> gridObects = new List<PathNode>();
    public GameObject pathNodePrefab;
    public void GenerateGrid(List<Vector2> pathNodes, Voronoi voronoi)
    {
        ClearGrid();

        for (int i = 0; i < pathNodes.Count; i++)
        {
            gridObects.Add(Instantiate<GameObject>(pathNodePrefab, this.transform).GetComponent<PathNode>());
            gridObects[i].transform.localPosition = new Vector3(pathNodes[i].X, 0, pathNodes[i].Y);
            gridObects[i].x = (int)pathNodes[i].X;
            gridObects[i].y = (int)pathNodes[i].Y;

            List<Vector2> vertices = voronoi.Region(pathNodes[i]);

            for(int j = 0; j < vertices.Count; j++)
            {
                if (vertices[j].X == 0 || vertices[j].Y == 0 || vertices[j].X == 512 || vertices[j].Y == 512) gridObects[i].GetComponent<PathNode>().isBorder = true;
            }

        }

        for (int i = 0; i < gridObects.Count; i++)
        {
            List<Vector2> neighbours = voronoi.NeighborSitesForSite(pathNodes[i]);

            for (int j = 0; j < neighbours.Count; j++)
            {
                for (int k = 0; k < gridObects.Count; k++)
                {
                    if (neighbours[j] == new Vector2(gridObects[k].transform.localPosition.x, gridObects[k].transform.localPosition.z))
                    {
                        gridObects[i].ConnectedNodes.Add(gridObects[k]);
                        break;
                    }
                }
            }
        }
    }

    public void ClearGrid()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        gridObects.Clear();
    }

    public void TransformScalePathNodes(float value)
    {
        foreach(PathNode node in gridObects)
        {
            node.transform.localPosition /= value;
        }
    }

    public void ResetGridVariables()
    {
        for (int i = 0; i < gridObects.Count; i++)
        {
            gridObects[i].ResetNode();
        }
    }
    
}
