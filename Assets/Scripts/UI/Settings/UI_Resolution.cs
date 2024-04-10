using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Resolution : MonoBehaviour
{
    private Resolution[] _resolutions;
    private int _index;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

    private void Start()
    {
        _resolutions = Screen.resolutions;
        _index = 0;
    }

    public void ButtonLeft()
    {
        _index--;
        if (_index < 0 ) _index = _resolutions.Length - 1;
        UpdateResolution();
    }


    public void ButtonRight()
    {
        _index++;
        if (_index >= _resolutions.Length) _index = 0;
        UpdateResolution();
    }

    public void UpdateResolution()
    {
        _textMeshProUGUI.text = _resolutions[_index].width.ToString() + " x " + _resolutions[_index].height.ToString();
        Screen.SetResolution(_resolutions[_index].width, _resolutions[_index].height, Screen.fullScreenMode);
    }
}
