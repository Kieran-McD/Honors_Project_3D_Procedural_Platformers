using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public PathNode NextNode;
    public List<PathNode> ConnectedNodes;

    public bool isLevel;
    public bool isObstacle;
    public bool isGoal;
    public bool isStart;
    public bool isPitfall;
    public bool isBorder;

    //Used For A* Path Finding
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode cameFromNode;

    public void ResetNode()
    {
        NextNode = null;
        isLevel = false;
        isObstacle = false;
        isGoal = false;
        isStart = false;
        isPitfall = false;
        isBorder = false;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    private void OnDrawGizmos()
    {
        if (ConnectedNodes.Count > 0)
        {
            for (int i = 0; i < ConnectedNodes.Count; i++)
            {
                if (ConnectedNodes[i].isBorder == true || isBorder == true)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, ConnectedNodes[i].transform.position);
                }
                else if (ConnectedNodes[i] == NextNode)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(transform.position, ConnectedNodes[i].transform.position);
                         
                }
                else if(ConnectedNodes[i].NextNode != this)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(transform.position, ConnectedNodes[i].transform.position);
                }
                
            }
        }
    }
}
