using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    PathGenerator planeGenerator;
    [SerializeField]
    GameObject obstacle;

    int totalPaths = 5;
    int amountPaths = 0;

    bool isFinished = false;
    

    // Start is called before the first frame update
    void Start()
    {
        planeGenerator.StartPathGenerator(planeGenerator.transform.position, planeGenerator.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFinished == true) return;

        if(totalPaths > 5)
        {
            isFinished = true;
        }

        if (planeGenerator.isFinished)
        {
            Transform exit = planeGenerator.GetCurrentPath().GetComponent<Path>().exit.transform;
            GameObject ob = Instantiate<GameObject>(obstacle);
            ob.transform.position = exit.position;
            ob.transform.rotation = Quaternion.Euler(0,exit.rotation.eulerAngles.y,0);

            planeGenerator.StartPathGenerator(ob.GetComponent<Obstacle>().exit.position, ob.transform.rotation);
            amountPaths++;
        }
    }
}
