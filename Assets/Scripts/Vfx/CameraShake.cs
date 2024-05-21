using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook _virtualCamera;

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
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0) StopShake();
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        _perlin0.m_AmplitudeGain = intensity;
        _perlin1.m_AmplitudeGain = intensity;
        _perlin2.m_AmplitudeGain = intensity;
        _timer = time;
    }

    public void StopShake()
    {
        _perlin0.m_AmplitudeGain = 0;
        _perlin1.m_AmplitudeGain = 0;
        _perlin2.m_AmplitudeGain = 0;
        _timer = 0;
    }
}
