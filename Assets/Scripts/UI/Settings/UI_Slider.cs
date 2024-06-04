using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Slider : MonoBehaviour
{
    [SerializeField] private string _parameterName;
    [SerializeField] private Slider _slider;

    private void Awake()
    {
        _slider.value = PlayerPrefs.GetFloat(_parameterName);
    }

    public void SetValue(float value)
    {
        AkSoundEngine.SetRTPCValue(_parameterName, value);
        PlayerPrefs.SetFloat(_parameterName, value);
    }
}
