using System.Collections;
using Vector2 = System.Numerics.Vector2;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices.WindowsRuntime;

public class PathFinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public List<PathNode> currentPath = new List<PathNode>();

    public VoronoiDiagram diagram;
    public Grid grid;
    List<PathNode> openList;
    List<PathNode> closedList;

    private void Start()
    {
        
    }
    //Generates Grid and Path for A* Algoritm
    public void Generate()
    {
        currentPath.Clear();
        currentPath = null;
        while (currentPath == null)
        {
            grid.GenerateGrid(diagram.voronoi.SiteCoords(), diagram.voronoi);
            currentPath = FindPath();
            grid.TransformScalePathNodes(4f);
        }  
    }

    //Generates Grid and Path random movement
    public void Generate(int totalPoints)
    {
        currentPath.Clear();
        currentPath = null;
        while (currentPath == null)
        {
            grid.GenerateGrid(diagram.voronoi.SiteCoords(), diagram.voronoi);
            currentPath = FindPath(totalPoints);
            grid.TransformScalePathNodes(4f);
        }
    }

    public PathFinding(VoronoiDiagram d)
    {
        diagram = d;
        List<Vector2> sites =  diagram.voronoi.SiteCoords();
        for(int i = 0; i < sites.Count; i++)
        {
            
        }
    }
    //Find Path For A* Algorithm
    private List<PathNode> FindPath()
    {
        
        PathNode startNode = grid.gridObects[Random.Range(0, grid.gridObects.Count)];
        PathNode endNode = grid.gridObects[Random.Range(0, grid.gridObects.Count)];
        while (startNode.isBorder || startNode == endNode) startNode = grid.gridObects[Random.Range(0, grid.gridObects.Count)];
        while (endNode.isBorder || startNode == endNode) endNode = grid.gridObects[Random.Range(0, grid.gridObects.Count)];

        startNode.isStart = true;
        endNode.isGoal = true;
        startNode.isStart = true;
        endNode.isGoal = true;
        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for(int i = 0; i < grid.gridObects.Count; i++)
        {
            PathNode pathNode = grid.gridObects[i];
            pathNode.gCost = float.MaxValue;
            pathNode.CalculateFCost();
            pathNode.cameFromNode = null;
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if(currentNode == endNode)
            {
                //Reached Goal
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);


            foreach(PathNode neighbourNode in currentNode.ConnectedNodes)
            {
                if (neighbourNode.isBorder) continue;
                if (closedList.Contains(neighbourNode)) continue;

                float tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }


        //Out of nodes in open list
        return null;
    }

    //Finds Path for random movement
    private List<PathNode> FindPath(int totalPaths)
    {

        PathNode startNode = grid.gridObects[Random.Range(0, grid.gridObects.Count)];
        while (startNode.isBorder) startNode = grid.gridObects[Random.Range(0, grid.gridObects.Count)];

        startNode.isStart = true;
        startNode.isLevel = true;

        for (int i = 0; i < grid.gridObects.Count; i++)
        {
            PathNode pathNode = grid.gridObects[i];
            pathNode.cameFromNode = null;
        }

        PathNode currentNode = startNode;
        int currentPathPoint = 0;
        while(currentPathPoint < totalPaths)
        {

            List<PathNode> availableNodes = new List<PathNode>();
            for(int i = 0; i < currentNode.ConnectedNodes.Count; i++)
            {
                PathNode temp = currentNode.ConnectedNodes[i];
             
                if (!temp.isBorder && !temp.isLevel)
                {
                    availableNodes.Add(temp);
                }
                temp.isBorder = true;
            }

            if (availableNodes.Count == 0) break;

            currentNode.isLevel = true;

            PathNode nextNode = availableNodes[Random.Range(0, availableNodes.Count)];
            nextNode.cameFromNode = currentNode;
            currentPathPoint++;



            currentNode = nextNode;
        }

        if (currentPathPoint >= totalPaths-1)
        {
            return CalculatePath(currentNode);
        }
        //Out of nodes in open list
        return null;
    }


    private List<PathNode> CalculatePath(PathNode endNode)
    {

        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        endNode.isGoal = true;
        while(currentNode.cameFromNode != null) {
            currentNode.isLevel = true;
            path.Add(currentNode.cameFromNode);
            currentNode.cameFromNode.NextNode = currentNode;
            currentNode = currentNode.cameFromNode;           
        }

        path.Reverse();

        return path;
    }
    

    private float CalculateDistanceCost(PathNode a, PathNode b)
    {
        float xDistance = Mathf.Abs(a.x - b.x);
        float yDistance = Mathf.Abs(a.y - b.y);
        float remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for(int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(PathFinding))]
public class PathFindingEditor : Editor
{

    // some declaration missing??

    override public void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            GUILayout.TextField("Dont Press Button To Much Bad Idea");

            PathFinding colliderCreator = (PathFinding)target;
            if (GUILayout.Button("Generate Grid"))
            {
                colliderCreator.Generate(); // how do i call this?
            }
        }
        DrawDefaultInspector();
    }
}
#endif
