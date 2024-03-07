using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelGeneratorVoronoi : MonoBehaviour
{
    [SerializeField]
    PathGenerator pathGenerator;

    [SerializeField]
    ObstacleStorage obstacles;
    [SerializeField]
    GameObject Split;

    [SerializeField]
    GameObject goal;

    [SerializeField]
    List<PathNode> mainPath;

    public bool isGenerationFinished = false;

    [SerializeField]
    int totalPointsForPath = 20;
    [SerializeField]
    Transform levelTransform;

    public bool isFinished = false;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GenerateLevel();
    }

    public void Restart()
    {
        foreach (Transform child in levelTransform)
        {
            Destroy(child.gameObject);
        }

        pathGenerator.ClearPathGenerator();
        pathGenerator.StartPathGenerator();
        isFinished = false;
        
    }

    void GenerateLevel()
    {
        if (isFinished == true) return;
        //Checks if the path has finished generating
        if (pathGenerator.GetIsFinished())
        {
            //Checks for a level object to store the path into
            if (levelTransform != null)
            {
                pathGenerator.GetCurrentPath().transform.parent = levelTransform;
            }

            Transform exit = pathGenerator.GetCurrentPath().GetComponent<Path>().exit.transform;
            GameObject ob = null;

            if (isGenerationFinished)
            {
                isFinished = true;
                //Checks for a level object to store the goal
                if (levelTransform != null)
                    ob = Instantiate<GameObject>(goal, levelTransform);
                else
                    ob = Instantiate<GameObject>(goal);
                ob.transform.position = exit.position;
                ob.transform.rotation = Quaternion.Euler(0, exit.rotation.eulerAngles.y, 0);
                pathGenerator.ClearPathGenerator();
                return;
            }
            //Gets a random obstacle to spawn
            ob = obstacles.GetRandomObstacle();

            if (ob == null)
            {
                pathGenerator.StartPathGenerator(exit.position, exit.rotation, totalPointsForPath);
                return;
            }
            //Checks for a level object to store the obstacle
            if (levelTransform != null)
                ob = Instantiate<GameObject>(ob, levelTransform);
            else
                ob = Instantiate<GameObject>(ob);

            //LOOK INTO BETTER METHOD FOR SPAWNING OBSTABVLES IN
            //ob.transform.position = exit.position + exit.forward * ob.GetComponent<Obstacle>().exit.transform.localPosition.z - exit.up * ob.GetComponent<Obstacle>().exit.transform.localPosition.y;
            ob.transform.position = exit.position - exit.forward * ob.GetComponent<Obstacle>().entry.transform.localPosition.z - exit.up * ob.GetComponent<Obstacle>().exit.transform.localPosition.y;
            ob.transform.rotation = Quaternion.Euler(0, exit.rotation.eulerAngles.y, 0);

            //Start Path generation
            pathGenerator.StartPathGenerator(ob.GetComponent<Obstacle>().exit.position, ob.transform.rotation, totalPointsForPath);

            //SplitPath(exit.position, exit.rotation);

        }
    }

    void SplitPath(Vector3 pos, Quaternion rotation)
    {
        GameObject split = Instantiate(Split, pos, rotation);
        split.transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        split.transform.position = pos + split.transform.forward * 1.5f;
        split.transform.localScale = new Vector3(pathGenerator.GetPathWidth(), 1, pathGenerator.GetPathWidth());
        split.GetComponent<SplitPathObjects>().BuildSpawnPaths(pathGenerator, totalPointsForPath);
    }

}

[CustomEditor(typeof(LevelGeneratorVoronoi))]
public class LevelGeneratorVoronoiEditor : Editor
{

    // some declaration missing??

    override public void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            GUILayout.TextField("Dont Press Button To Much Bad Idea");

            LevelGeneratorVoronoi colliderCreator = (LevelGeneratorVoronoi)target;
            if (GUILayout.Button("Create Diagram"))
            {
                colliderCreator.Restart(); // how do i call this?
            }
        }
        DrawDefaultInspector();
    }
}

