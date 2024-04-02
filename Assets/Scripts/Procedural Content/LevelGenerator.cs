using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
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
    int totalPaths = 5;
    int amountPaths = 0;

    [SerializeField]
    int totalPointsForPath = 20;
    [SerializeField]
    Transform levelTransform;

    bool isFinished = false;
    [SerializeField]
    bool reset = false;

    [SerializeField]
    int seed = 0;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Random.InitState(seed);
        pathGenerator.StartPathGenerator(pathGenerator.transform.position, pathGenerator.transform.rotation, totalPointsForPath);
        GenerateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (reset)
        {
            Restart();
        }

        //GenerateLevel();
    }

    public void Restart()
    {
        foreach (Transform child in levelTransform)
        {
            Destroy(child.gameObject);
        }
        UnityEngine.Random.InitState(seed);
        pathGenerator.ClearPathGenerator();
        pathGenerator.StartPathGenerator(pathGenerator.transform.position, pathGenerator.transform.rotation, totalPointsForPath);
        reset = false;
        isFinished = false;
        amountPaths = 0;
        GenerateLevel();
    }

    void GenerateLevel()
    {      
        if (isFinished == true) return;

        while (!isFinished)
        {
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
                amountPaths++;
                if (amountPaths >= totalPaths)
                {
                    isFinished = true;
                    //Checks for a level object to store the goal
                    if (levelTransform != null)
                        ob = Instantiate<GameObject>(goal, levelTransform);
                    else
                        ob = Instantiate<GameObject>(goal);
                    ob.transform.position = exit.position - new Vector3(0, 0,0);
                    ob.transform.rotation = Quaternion.Euler(0, exit.rotation.eulerAngles.y + 90, 0);
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
            else
            {
                while (!pathGenerator.GetIsFinished())
                {
                    Debug.Log("I was here");
                    pathGenerator.UpdateWorm();
                }
            }
        }
    }

    void SplitPath(Vector3 pos, Quaternion rotation)
    {
        GameObject split = Instantiate(Split, pos, rotation);
        split.transform.rotation = Quaternion.Euler(0,rotation.eulerAngles.y,0);
        split.transform.position = pos + split.transform.forward * 1.5f;
        split.transform.localScale = new Vector3(pathGenerator.GetPathWidth(),1, pathGenerator.GetPathWidth());
        split.GetComponent<SplitPathObjects>().BuildSpawnPaths(pathGenerator, totalPointsForPath);
    }

    public void SetTotalPaths(Single paths)
    {
        totalPaths =    ((int)paths);
    }
    public void SetTotalPointsPath(Single points)
    {
        totalPointsForPath =  ((int)points);
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{

    // some declaration missing??

    override public void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            GUILayout.TextField("Dont Press Button To Much Bad Idea");

            LevelGenerator colliderCreator = (LevelGenerator)target;
            if (GUILayout.Button("Generate New Level"))
            {
                colliderCreator.Restart(); // how do i call this?
            }
        }
        else
        {
            GUILayout.TextField("Dont Press Button To Much Bad Idea");

            LevelGenerator colliderCreator = (LevelGenerator)target;

            if (GUILayout.Button("Generate New Level"))
            {
                colliderCreator.Restart(); // how do i call this?
            }
        }
       
        DrawDefaultInspector();
    }
}

#endif
