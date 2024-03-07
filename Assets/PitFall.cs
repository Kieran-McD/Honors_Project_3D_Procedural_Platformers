using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitFall : MonoBehaviour
{
    public GameObject walls;
    public GameObject lava;
    public GameObject platform;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            Debug.Log("Player Collided");
            FindAnyObjectByType<SpawnPlayer>().Spawn();
        }
    }
}
