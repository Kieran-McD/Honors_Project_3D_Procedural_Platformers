using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public PathNode NextNode;

    public bool isObstacle; 

    private void OnDrawGizmos()
    {
        if (NextNode)
        {
            Gizmos.DrawLine(transform.position, NextNode.transform.position);
        }       
    }
}
