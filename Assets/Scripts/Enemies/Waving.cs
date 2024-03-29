using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TransformWaveData
{
    public Transform Transformed;
    public Vector3 BaseLocalPosition;
}

public class WaveData
{

}

public class Waving : MonoBehaviour
{
    [TitleGroup("Components")]
    public List<TransformWaveData> waveTransforms = new List<TransformWaveData>();
    public WavingData WavingData;
    


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void WavePoints()
    {
        foreach (TransformWaveData transformData in waveTransforms)
        {
            transformData.Transformed.localPosition = transformData.BaseLocalPosition;
        }
    }


}
