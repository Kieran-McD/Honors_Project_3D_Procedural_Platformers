using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject player;
    bool spawn;
    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (spawn)
        {
            spawn = false;
            player.transform.position = transform.position;
        }
    }

    public void Spawn()
    {
        spawn = true;
    }
}
