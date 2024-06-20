using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook _virtualCamera;

    private float _currentIntensity;
    private float _intensityAdded;
    private float _currentFrequency;
    private float _frequencyAdded;
    private float _timer;
    private CinemachineBasicMultiChannelPerlin _perlin0;
    private CinemachineBasicMultiChannelPerlin _perlin1;
    private CinemachineBasicMultiChannelPerlin _perlin2;


    private void Start()
    {
        _perlin0 = _virtualCamera.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _perlin1 = _virtualCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _perlin2 = _virtualCamera.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        StopShake();
    }

    private void Update()
    {
        
        _currentIntensity += _intensityAdded;
        _currentFrequency += _frequencyAdded;
        if (_timer > 0)
        {
            _timer -= Time.unscaledDeltaTime;

            if (_timer <= 0) StopShake();
        }

        _perlin0.m_AmplitudeGain = _currentIntensity;
        _perlin1.m_AmplitudeGain = _currentIntensity;
        _perlin2.m_AmplitudeGain = _currentIntensity;
        _perlin0.m_FrequencyGain = _currentFrequency;
        _perlin1.m_FrequencyGain = _currentFrequency;
        _perlin2.m_FrequencyGain = _currentFrequency;
        _currentIntensity = 0;
        _currentFrequency = 0;
    }


    public void SetShakeCamera(float intensity, float frequency = 1)
    {
        if (PlayerPrefs.GetInt("CameraShake") == 0) return;
        _currentIntensity += intensity;
        _currentFrequency += frequency;
    }

    public void ShakeCamera(float intensity, float time , float frequency = 1)
    {
        if (PlayerPrefs.GetInt("CameraShake") == 0) return;
        _intensityAdded = intensity;
        _frequencyAdded = frequency;
        _timer = time;
    }

    public void StopShake()
    {
        _intensityAdded = 0;
        _frequencyAdded = 0;
        _timer = 0;
    }
}
