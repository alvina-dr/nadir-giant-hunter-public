using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{

    [SerializeField] private Toggle _toggle;

    private void Start()
    {
        _toggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 0 ? false : true;
    }

    public void SetFullscreeen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }
}
