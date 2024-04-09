using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;

public class UI_VolumeSlider : MonoBehaviour
{
    [SerializeField] private string _rtpcParameterName;
    [SerializeField] private Slider _slider;

    private void Awake()
    {
        _slider.value = PlayerPrefs.GetInt(_rtpcParameterName);
    }

    public void SetVolume(float volume)
    {
        AkSoundEngine.SetRTPCValue(_rtpcParameterName, volume * 5);
        PlayerPrefs.SetInt(_rtpcParameterName, Mathf.RoundToInt(volume));
    }
}
