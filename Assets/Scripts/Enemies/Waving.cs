using Enemies;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static ak.wwise.core;

[Serializable]
public class TransformWaveData
{
    public Transform Transformed;
    [HideInInspector]
    public Vector3 BaseLocalPosition;
    [ReadOnly]
    public Vector3 Position;
    [ReadOnly]
    public Vector3 Transformation;
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
    public AnimationCurve waveTransformsAffect;
}

public class Waving : MonoBehaviour
{
    [TitleGroup("Components")]
    public List<WaveData> waves = new List<WaveData>();
    public List<TransformWaveData> waveTransforms = new List<TransformWaveData>();



    void Start()
    {
        SetupWavePoints();
    }

    
    void Update()
    {
        WavePoints();
        CleanPoints();
        CleanTransformations();
    }

    private void WavePoints()
    {
        foreach (WaveData wave in waves)
        {
            Vector3 waveDirection = transform.forward * wave.data.WaveDirection.z + transform.right * wave.data.WaveDirection.x + transform.up * wave.data.WaveDirection.y;

            for (int i = 0; i < waveTransforms.Count; i++)
            {
                TransformWaveData transformData = waveTransforms[i];
                float offSet = 0;
                float delta = 0;
                delta = wave.data.DeltType == WavingData.DeltaType.Time ? Time.time : delta;
                
                delta += transformData.Transformed.position.y *0.01f* wave.data.WaveWidth;
                


                offSet = wave.data.OndulType == WavingData.OndulationType.Cosinus ? Mathf.Cos(delta * wave.data.WaveSpeed) : offSet;
                offSet = wave.data.OndulType == WavingData.OndulationType.Sinus ? Mathf.Sin(delta * wave.data.WaveSpeed) : offSet;
                
                transformData.Delta = delta;
                transformData.OffSet = offSet;

                float distToFirst = Vector3.Distance(waveTransforms[0].BaseLocalPosition, transformData.Transformed.position);
                float totalDist = Vector3.Distance(waveTransforms[0].BaseLocalPosition, waveTransforms[waveTransforms.Count-1].Transformed.position);
                float influence = wave.waveTransformsAffect.Evaluate(distToFirst/totalDist);
                Vector3 transformation = (waveDirection * offSet * (wave.data.WaveLenght + distToFirst * 0.01f * wave.data.WaveLenghtAccumulation)) * influence;
                Vector3 position = transform.position + transformData.BaseLocalPosition + transformData.Transformation + transformation;
                transformData.Position = position;
                transformData.Transformed.position = position;
                transformData.Transformation += transformation;
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
                for (int i = 0; i < waveTransforms.Count; i++)
                {
                    TransformWaveData transformData = waveTransforms[i];
                    if (transformData.Position != transformData.Transformed.position)
                    {
                        isClean = false;
                        transformData.Transformed.position = transformData.Position;
                    }
                }
            }
        }
    }

    private void CleanTransformations()
    {
        foreach(WaveData wave in waves)
        {
            for (int i = 0; i < waveTransforms.Count; i++)
            {
                TransformWaveData transformData = waveTransforms[i];
                transformData.Transformation = Vector3.zero;
            }
        }
    }

    public void IkHarmCompensateWave(Leg leg)
    {
        foreach (WaveData wave in waves)
        {
            Vector3 waveDirection = transform.forward * wave.data.WaveDirection.z + transform.right * wave.data.WaveDirection.x + transform.up * wave.data.WaveDirection.y;
            float offSet = 0;
            float delta = 0;
            delta = wave.data.DeltType == WavingData.DeltaType.Time ? Time.time : delta;
                
            delta += leg.FirstBone.transform.position.y * 0.01f * wave.data.WaveWidth;
                


            offSet = wave.data.OndulType == WavingData.OndulationType.Cosinus ? Mathf.Cos(delta * wave.data.WaveSpeed) : offSet;
            offSet = wave.data.OndulType == WavingData.OndulationType.Sinus ? Mathf.Sin(delta * wave.data.WaveSpeed) : offSet;
            float distToFirst = Vector3.Distance(waveTransforms[0].BaseLocalPosition, leg.FirstBone.transform.position);

            leg.Target.position -= waveDirection * offSet * (wave.data.WaveLenght + distToFirst * 0.01f * wave.data.WaveLenghtAccumulation);
        }
    }

    private void SetupWavePoints()
    {
        foreach (WaveData wave in waves)
        {
            foreach (TransformWaveData transformData in waveTransforms)
            {
                transformData.BaseLocalPosition = transformData.Transformed.position - transform.position;
            }
        }
    }
}
