using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public PathNode NextNode;

    public bool isObstacle;
    public bool isGoal;
    public bool isStart;

    private void OnDrawGizmos()
    {
        if (NextNode)
        {
            Gizmos.DrawLine(transform.position, NextNode.transform.position);
        }       
    }
}
