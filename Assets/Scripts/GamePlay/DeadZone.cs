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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag != "Player")
        {
            return;
        }

        other.GetComponent<CharacterController>().velocity.Set(0, 0, 0);
        other.transform.root.position = playerSpawn.position;
    }

}
