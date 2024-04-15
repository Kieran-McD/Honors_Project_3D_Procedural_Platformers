using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaeObject : MonoBehaviour
{
    [SerializeField]
    bool shouldScale = false;
    private void Awake()
    {
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        if(shouldScale) transform.localScale *= Random.Range(0.65f, 1f) ;
    }
}
