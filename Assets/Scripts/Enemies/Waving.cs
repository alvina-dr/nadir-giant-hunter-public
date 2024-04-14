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
    public Vector3 Position;
    [ReadOnly]
    public float Delta;
    [ReadOnly]
    public float OffSet;
    [ReadOnly]
    public Vector3 Added;
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
        CleanPoints();
    }

    private void WavePoints()
    {
        foreach (WaveData wave in waves)
        {
            Vector3 waveDirection = transform.forward * wave.data.WaveDirection.z + transform.right * wave.data.WaveDirection.x + transform.up * wave.data.WaveDirection.y;

            for (int i = 0; i < wave.waveTransforms.Count; i++)
            {
                TransformWaveData transformData = wave.waveTransforms[i];
                float offSet = 0;
                float delta = 0;
                delta = wave.data.DeltType == WavingData.DeltaType.Time ? Time.time : delta;
                if (i != 0)
                {
                    delta += (i+1)*0.1f* wave.data.WaveWidth;
                }


                offSet = wave.data.OndulType == WavingData.OndulationType.Cosinus ? Mathf.Cos(delta * wave.data.WaveSpeed) : offSet;
                offSet = wave.data.OndulType == WavingData.OndulationType.Sinus ? Mathf.Sin(delta * wave.data.WaveSpeed) : offSet;
                
                transformData.Delta = delta;
                transformData.OffSet = offSet;
                Vector3 position = transform.position + transformData.BaseLocalPosition + waveDirection * offSet * (wave.data.WaveLenght + i * wave.data.WaveLenghtAccumulation);
                transformData.Position = position;
                transformData.Transformed.position = position;
            }
        }
    }

    private void CleanPoints()
    {
        bool isClean = false;
        int iteration = 10;
        while (!isClean)
        {
            iteration--;
            if (iteration <= 0)
            {
                break;
            }
            isClean = true;
            foreach (WaveData wave in waves)
            {
                for (int i = 0; i < wave.waveTransforms.Count; i++)
                {
                    TransformWaveData transformData = wave.waveTransforms[i];
                    if (transformData.Position != transformData.Transformed.position)
                    {
                        isClean = false;
                        transformData.Transformed.position = transformData.Position;
                    }
                }
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
