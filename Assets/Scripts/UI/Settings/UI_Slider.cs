using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_Slider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] public string _parameterName;
    [SerializeField] public bool WwiseSlider;
    [SerializeField] public Slider _slider;
    [SerializeField] public Image _background;
    [SerializeField] public AK.Wwise.Event _sfxHover;
    [SerializeField] public AK.Wwise.Event _sfxClick;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(_parameterName))
        {
            if (WwiseSlider)
                _slider.value = PlayerPrefs.GetInt(_parameterName);
            else
                _slider.value = PlayerPrefs.GetFloat(_parameterName);
        }
        //GetComponent<Toggle>().onValueChanged.AddListener(OnClick);
    }

    public void SetValue(float value) //for any other value
    {
        if (WwiseSlider)
        {
            AkSoundEngine.SetRTPCValue(_parameterName, value * 5);
            PlayerPrefs.SetInt(_parameterName, Mathf.RoundToInt(value));
        } else
        {
            PlayerPrefs.SetFloat(_parameterName, value);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        _background.gameObject.SetActive(true);
        _sfxHover.Post(DataHolder.Instance.gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _background.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelect(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselect(eventData);
    }

    private void OnClick(bool value)
    {
        _sfxClick.Post(DataHolder.Instance.gameObject);
    }
}
