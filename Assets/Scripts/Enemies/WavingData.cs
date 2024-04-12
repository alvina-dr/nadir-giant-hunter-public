using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WavingData", menuName = "ScriptableObjects/WavingData")]
public class WavingData : ScriptableObject
{
    public enum OndulationType
    {
        Cosinus,
        Sinus
    }
    public enum DeltaType
    {
        Position,
        Time
    }

    public float WaveLenght;
    public float WaveSpeed;
    public float WaveWidth;
    public OndulationType OndulType;
    public DeltaType DeltType;
    public Vector3 DeltaDirection;
    public Vector3 WaveDirection;
}
