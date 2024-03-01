using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public PathNode NextNode;
    public List<PathNode> ConnectedNodes;

    public bool isObstacle;
    public bool isGoal;
    public bool isStart;
    public bool isPitfall;

    private void OnDrawGizmos()
    {
        if (ConnectedNodes.Count > 0)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < ConnectedNodes.Count; i++)
            {
                Gizmos.DrawLine(transform.position, ConnectedNodes[i].transform.position);
            }
        }

        if (NextNode)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, NextNode.transform.position);
        }    
    }
}
