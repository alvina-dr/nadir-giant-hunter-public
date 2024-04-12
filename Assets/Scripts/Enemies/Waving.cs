using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class TransformWaveData
{
    public Transform Transformed;
    [HideInInspector]
    public Vector3 BaseLocalPosition;
    [ReadOnly]
    public float Delta;
    [ReadOnly]
    public float OffSet;
    [ReadOnly]
    public Vector3 added;
}

[Serializable]
public class WaveData
{
    public WavingData data;
    public List<TransformWaveData> waveTransforms = new List<TransformWaveData>();
}

public class Waving : MonoBehaviour
{
    [TitleGroup("Components")]
    public List<WaveData> waves = new List<WaveData>();



    void Start()
    {
        SetupWavePoints();
    }

    void Update()
    {
        WavePoints();
    }

    private void WavePoints()
    {
        foreach (WaveData wave in waves)
        {
            for (int i = 0; i < wave.waveTransforms.Count; i++)
            {
                TransformWaveData transformData = wave.waveTransforms[i];
                float offSet = 0;
                float delta = 0;
                delta = wave.data.DeltType == WavingData.DeltaType.Time ? Time.time : delta;
                if (i != 0)
                {
                    delta += (wave.waveTransforms[i].Transformed.position - wave.waveTransforms[i - 1].Transformed.position).magnitude*wave.data.WaveWidth;
                }


                offSet = wave.data.OndulType == WavingData.OndulationType.Cosinus ? Mathf.Cos(delta * wave.data.WaveSpeed) : offSet;
                offSet = wave.data.OndulType == WavingData.OndulationType.Sinus ? Mathf.Sin(delta * wave.data.WaveSpeed) : offSet;
                
                transformData.Delta = delta;
                transformData.OffSet = offSet;
                transformData.Transformed.position = transform.position + transformData.BaseLocalPosition + wave.data.WaveDirection * offSet * wave.data.WaveLenght;
            }
        }
    }

    private void SetupWavePoints()
    {
        foreach (WaveData wave in waves)
        {
            foreach (TransformWaveData transformData in wave.waveTransforms)
            {
                transformData.BaseLocalPosition = transformData.Transformed.position - transform.position;
            }
        }
    }
}
