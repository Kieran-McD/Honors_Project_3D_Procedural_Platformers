using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{

    Vector3 startPos;
    float yOffSet = 0.2f;
    float speed = 4f;

    private void Awake()
    {
        startPos = transform.position;
    }

    private void AnimateGoal()
    {
        Vector3 updateYPos;

        updateYPos = new Vector3(0, Mathf.Sin(Time.time * speed) * yOffSet, 0);

        transform.position = startPos + updateYPos;

        transform.rotation =  Quaternion.Euler(0, Time.time * 180, 0);

    }

    // Update is called once per frame
    void Update()
    {
        AnimateGoal();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player")
        {
            return;
        }

        gameObject.SetActive(false);
    }
}
