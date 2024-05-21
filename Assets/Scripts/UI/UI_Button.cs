using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class UI_Button : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] private Image _image;
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
    }
}
