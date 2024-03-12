using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DiscObject
{
    public GameObject Object;
    public float Radius;
}

[CreateAssetMenu]
public class LevelPreset : ScriptableObject
{


    public Material LevelMat;
    public Material WaterMat;
    //public List<GameObject> Objects;
    public List<DiscObject> Objects;
    
    public float PerlinHeightScale = 10f;
    public int ObjectDensity = 500;

    
}
