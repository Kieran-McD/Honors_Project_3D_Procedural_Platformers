using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelPreset : ScriptableObject
{
    public Material LevelMat;
    public List<GameObject> Objects;
    public float PerlinHeightScale = 10f;
    public int ObjectDensity = 500;
}
