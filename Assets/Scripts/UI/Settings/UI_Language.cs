using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class UI_Language : MonoBehaviour
{
    private List<UnityEngine.Localization.Locale> _locales;
    private int _index;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

    private void Start()
    {
        _locales = LocalizationSettings.AvailableLocales.Locales;
        if (PlayerPrefs.HasKey("Language"))
            _index = PlayerPrefs.GetInt("Language");
        else _index = 0;
        UpdateLanguage();
    }

    public void ButtonLeft()
    {
        _index--;
        if (_index < 0) _index = _locales.Count - 1;
        UpdateLanguage();
    }


    public void ButtonRight()
    {
        _index++;
        if (_index >= _locales.Count) _index = 0;
        UpdateLanguage();
    }

    public void UpdateLanguage()
    {
        _textMeshProUGUI.text = _locales[_index].LocaleName;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_index];
        PlayerPrefs.SetInt("Language", _index);
    }
}
