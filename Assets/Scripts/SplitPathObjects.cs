using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SplitPathObjects : MonoBehaviour
{
    public Transform MainPath;
    public Transform SplitPathLeft;
    public Transform SplitPathRight;

    public void BuildSpawnPaths(PathGenerator pathGenerator, int totalPointsInPath)
    {
        pathGenerator.StartPathGenerator(MainPath.position, MainPath.rotation, totalPointsInPath);
        Instantiate(pathGenerator).StartPathGenerator(SplitPathLeft.position, SplitPathLeft.rotation, totalPointsInPath);
        Instantiate(pathGenerator).StartPathGenerator(SplitPathRight.position, SplitPathRight.rotation, totalPointsInPath);
    }
}
