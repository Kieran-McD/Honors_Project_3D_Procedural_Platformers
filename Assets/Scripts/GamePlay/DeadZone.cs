using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    Transform playerSpawn;

    private void Start()
    {
        if (!playerSpawn) playerSpawn = FindAnyObjectByType<SpawnPlayer>().transform;
    }

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Did anything collide");

        if (collision.transform.tag == "Player")
        {
            Debug.Log("Player Collided");
            FindAnyObjectByType<SpawnPlayer>().Spawn();
        }
    }

}
