using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitFall : MonoBehaviour
{
    public GameObject walls;
    public GameObject lava;
    public GameObject platform;

    private void Awake()
    {
        platform.transform.rotation = Quaternion.Euler(0, Random.Range(0,360), 0);
        platform.transform.localScale = new Vector3(Random.Range(1f,1.5f), platform.transform.localScale.y, Random.Range(1f, 1.5f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            Debug.Log("Player Collided");
            FindAnyObjectByType<SpawnPlayer>().Spawn();
        }
    }
}
