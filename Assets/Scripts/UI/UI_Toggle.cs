using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UI_Toggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image _background;
    [SerializeField] private AK.Wwise.Event _sfxHover;
    [SerializeField] private AK.Wwise.Event _sfxClick;

    private void Awake()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(OnClick);
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
