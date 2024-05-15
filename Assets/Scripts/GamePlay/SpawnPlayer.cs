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

    private void Update()
    {
        if (spawn)
        {
            spawn = false;
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = transform.position;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (spawn)
        {
           
        }
    }

    public void Spawn()
    {
        spawn = true;
    }
}
