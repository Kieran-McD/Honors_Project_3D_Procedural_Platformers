using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    public SpawnPlayer spawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Death")
        {
            spawn.Spawn();
            Debug.Log("Collision");
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
    }
}
