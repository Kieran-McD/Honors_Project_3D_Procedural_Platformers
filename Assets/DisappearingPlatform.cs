using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    Vector3 maxScale;
    [SerializeField]
    float delay = 1f;
    [SerializeField]
    float maxTime = 5f;
    [SerializeField]
    float timeRespawn = 2f;
    [SerializeField]
    BoxCollider collider;

    bool isWorking = false;

    // Start is called before the first frame update
    void Start()
    {
        maxScale = transform.localScale;

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("BEEPD");
        if(other.tag != "Player")
        {
            return;
        }
        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        if (isWorking) yield break;

        isWorking = true;
        yield return new WaitForSeconds(delay);

        float time = 0;
        while(transform.localScale != new Vector3(0, maxScale.y, 0))
        {
            time += Time.deltaTime;
            Vector3 currentScale = Vector3.Lerp(maxScale, new Vector3(0,maxScale.y,0), time / maxTime);
            
            transform.localScale = currentScale;
            
            yield return null;
        }

        collider.enabled = false;
        yield return new WaitForSeconds(timeRespawn);

        transform.localScale = maxScale;
        isWorking = false;

        collider.enabled = true;
        yield return null;
    }
}
