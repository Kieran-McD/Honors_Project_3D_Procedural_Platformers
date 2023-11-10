using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
   [System.Serializable]
   struct PosTimer
    {
        [SerializeField] public Transform pos;
        [SerializeField] public float timer;
    }

    [SerializeField] List<PosTimer> posList;
    [SerializeField] float speed;
    private int currentId;
    private bool reverse = false;
    private float timer;
    [SerializeField]
    private bool isLooping = false;


    private PosTimer nextPos;
    // Start is called before the first frame update
    void Start()
    {
        currentId= 0;
        int id = 0;
        foreach (PosTimer t in posList)
        {
            if (id == currentId)
            {
                nextPos = t; 
            }
            id++;
        }
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {

        nextID();

        transform.position = Vector3.MoveTowards(transform.position, nextPos.pos.position, speed * Time.deltaTime);

    }

    void nextID()
    {
        if (transform.position == nextPos.pos.position)
        {

            if (timer >= nextPos.timer)
            {
                timer = 0;
                if (currentId == posList.Capacity-1)
                {

                    if (isLooping)
                    {
                        currentId = -1;
                    }
                    else
                    {
                        reverse = true;
                    }
                }
                else if (currentId == 0)
                {
                    if (isLooping)
                    {

                    }
                    else
                    {
                        reverse = false;
                    }
                }
                if (reverse)
                {
                    currentId--;
                }
                else
                {
                    currentId++;
                }

                int id = 0;
                foreach (PosTimer t in posList)
                {
                    
                    if (id == currentId)
                    {
                       nextPos = t;
                    }
                    id++;
                }
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

}
