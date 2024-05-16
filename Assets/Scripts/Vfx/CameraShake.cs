using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private float _timer;
    private CinemachineBasicMultiChannelPerlin _perlin;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        StopShake();
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0) StopShake();
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _perlin.m_AmplitudeGain = intensity;
        _timer = time;
    }

    public void StopShake()
    {
        CinemachineBasicMultiChannelPerlin perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _perlin.m_AmplitudeGain = 0;
        _timer = 0;
    }
}
