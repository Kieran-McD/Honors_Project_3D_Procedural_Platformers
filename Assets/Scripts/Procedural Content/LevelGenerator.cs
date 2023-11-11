using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    PathGenerator planeGenerator;
    [SerializeField]
    List<GameObject> obstacle;
    [SerializeField]
    GameObject goal;

    int totalPaths = 5;
    int amountPaths = 0;

    [SerializeField]
    int totalPointsForPath = 20;

    bool isFinished = false;
    

    // Start is called before the first frame update
    void Start()
    {
        planeGenerator.StartPathGenerator(planeGenerator.transform.position, planeGenerator.transform.rotation, totalPointsForPath);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFinished == true) return;

        


        if (planeGenerator.isFinished)
        {
             
            //LOOK INTO BETTER METHOD FOR SPAWNING OBSTABVLES IN
            Transform exit = planeGenerator.GetCurrentPath().GetComponent<Path>().exit.transform;
            GameObject ob; 
            amountPaths++;
            if (amountPaths >= totalPaths)
            {
                isFinished = true;
                ob = Instantiate<GameObject>(goal);
                ob.transform.position = exit.position;
                ob.transform.rotation = Quaternion.Euler(0, exit.rotation.eulerAngles.y, 0);
                return;
            }
            ob = Instantiate<GameObject>(obstacle[Random.Range(0, obstacle.Count)]);
            //ob.transform.position = exit.position + exit.forward * ob.GetComponent<Obstacle>().exit.transform.localPosition.z - exit.up * ob.GetComponent<Obstacle>().exit.transform.localPosition.y;
            ob.transform.position = exit.position - exit.forward * ob.GetComponent<Obstacle>().entry.transform.localPosition.z  - exit.up * ob.GetComponent<Obstacle>().exit.transform.localPosition.y;
            ob.transform.rotation = Quaternion.Euler(0,exit.rotation.eulerAngles.y,0);

            planeGenerator.StartPathGenerator(ob.GetComponent<Obstacle>().exit.position, ob.transform.rotation, totalPointsForPath);

        }
    }
}
