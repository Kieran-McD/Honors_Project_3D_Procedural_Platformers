using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPlatform : MonoBehaviour
{
    bool direction = true;
    bool isCompleted;

    float speed = 180;

    public void Flip()
    {
        isCompleted = false;
        direction = !direction;
        StartCoroutine(FlipAction());
    }

    IEnumerator FlipAction()
    {
        bool currentDirection = direction;
        while (currentDirection == direction && isCompleted == false)
        {
            switch (direction)
            {
                case false:
                    if(transform.localRotation == Quaternion.Euler(180, 0, 0))
                    {
                        isCompleted = true;
                        break;
                    }
                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(180,0,0), speed * Time.deltaTime);
                    break;
                case true:
                    if (transform.localRotation == Quaternion.Euler(0, 0, 0))
                    {
                        isCompleted = true;
                        break;
                    }
                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(0, 0, 0), speed * Time.deltaTime);
                    break;
            }

            yield return null;
        }

        yield return null;
    }
}
