using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using AK.Wwise;

public class UI_Button : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] private Image _image;
    [SerializeField] private AK.Wwise.Event _sfxHover;
    [SerializeField] private AK.Wwise.Event _sfxClick;

    private void Awake()
    {
        _image.material = new Material(_image.material);
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.DOScale(1f, .3f).SetUpdate(true);
        _textMeshProUGUI.color = Color.white;
        _image.material.SetInt("_Selected", 0);
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.DOScale(1.1f, .3f).SetUpdate(true);
        _textMeshProUGUI.color = Color.black;
        _image.material.SetInt("_Selected", 1);
        _sfxHover.Post(DataHolder.Instance.gameObject);
    }

    private void OnClick()
    {
        _sfxClick.Post(DataHolder.Instance.gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelect(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselect(eventData);
    }
}
