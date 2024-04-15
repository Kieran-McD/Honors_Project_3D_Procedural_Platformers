using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{

    Vector3 startPos;
    float yOffSet = 0.2f;
    float speed = 4f;

    TextMeshProUGUI text;

    public Transform collectable;
    public UnityEvent func;

    private void Start()
    {
        startPos = collectable.localPosition;
        text = FindFirstObjectByType<YouWinText>().GetComponent<TextMeshProUGUI>();
        text.color = text.color - new Color(0, 0, 0, 1f);
    }

    private void AnimateGoal()
    {
        Vector3 updateYPos;

        updateYPos = new Vector3(0, Mathf.Sin(Time.time * speed) * yOffSet, 0);

        collectable.localPosition = startPos + updateYPos;

        collectable.rotation =  Quaternion.Euler(0, Time.time * 180, 0);

    }

    IEnumerator DisplayText()
    {
        float time = 0;
        while(text.color.a < 1f)
        {
            time += Time.deltaTime;
            text.color = text.color + new Color(0, 0, 0, Mathf.Lerp(0, 1, time / 1000f));
            yield return null;
        }

        yield return null;
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

        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(DisplayText());
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        //func.Invoke();      
    }
}
