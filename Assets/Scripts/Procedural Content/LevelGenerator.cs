using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct AvailableObstacles {
        public GameObject obstacle;
        public bool active;

        public GameObject GetObstacle()
        {
            if (active) return obstacle;
            return null;
        }
    }
    [SerializeField]
    PathGenerator pathGenerator;
    [SerializeField]
    List<AvailableObstacles> obstacle;
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

    // Start is called before the first frame update
    void Start()
    {
        pathGenerator.StartPathGenerator(pathGenerator.transform.position, pathGenerator.transform.rotation, totalPointsForPath);
    }

    // Update is called once per frame
    void Update()
    {
        if (reset)
        {
            Restart();
        }

        GenerateLevel();
    }

    public void Restart()
    {
        foreach (Transform child in levelTransform)
        {
            Destroy(child.gameObject);
        }
        if (pathGenerator.GetCurrentWorm()) Destroy(pathGenerator.GetCurrentWorm().gameObject);
        pathGenerator.ClearPathGenerator();
        pathGenerator.StartPathGenerator(pathGenerator.transform.position, pathGenerator.transform.rotation, totalPointsForPath);
        reset = false;
        isFinished = false;
        amountPaths = 0;
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
            amountPaths++;
            if (amountPaths >= totalPaths)
            {
                isFinished = true;
                //Checks for a level object to store the goal
                if (levelTransform != null)
                    ob = Instantiate<GameObject>(goal, levelTransform);
                else
                    ob = Instantiate<GameObject>(goal);
                ob.transform.position = exit.position;
                ob.transform.rotation = Quaternion.Euler(0, exit.rotation.eulerAngles.y, 0);
                return;
            }
            //Gets a random obstacle to spawn
            while (ob == null)
            {
                ob = obstacle[Random.Range(0, obstacle.Count)].GetObstacle();
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

            pathGenerator.StartPathGenerator(ob.GetComponent<Obstacle>().exit.position, ob.transform.rotation, totalPointsForPath);

        }
    }
}
