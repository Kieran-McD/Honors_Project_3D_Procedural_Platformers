using System.Collections;
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

        public GameObject GetObstacle()
        {
            if (active) return obstacle;
            return null;
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
}
