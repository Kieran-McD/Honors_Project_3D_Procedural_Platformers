using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObstacleStorage : ScriptableObject
{
    [System.Serializable]
    public struct AvailableObstacles
    {
        public GameObject obstacle;
        public bool active;

        public AvailableObstacles(GameObject o, bool a)
        {
            obstacle = o;
            active = a;
        }

        public GameObject GetObstacle()
        {
            if (active) return obstacle;
            return null;
        }

        public void SetAcitve(bool a)
        {
            active = a;
            Debug.Log(active);
        }

    }

    public List<AvailableObstacles> obstacles;

    public GameObject GetRandomObstacle()
    {
        List<GameObject> list = new List<GameObject>();
        for(int i = 0; i < obstacles.Count; i++)
        {
            if (obstacles[i].active)
            {
                list.Add(obstacles[i].obstacle);
            }            
        }

        if (list.Count == 0) return null;

        return list[Random.Range(0, list.Count)];
    }

    public void ToggleOnOff(int index)
    {
        if(index < 0 || index >= obstacles.Count)
        {
            return;
        }
        obstacles[index] = new AvailableObstacles(obstacles[index].obstacle, !obstacles[index].active);
    }
}
