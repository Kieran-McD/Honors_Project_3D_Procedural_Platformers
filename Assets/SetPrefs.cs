using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPrefs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetFloat("Sensitivity", 20f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
