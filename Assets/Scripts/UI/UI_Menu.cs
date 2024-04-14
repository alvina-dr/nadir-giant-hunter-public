using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UI_Menu : MonoBehaviour
{
    [SerializeField] private GameObject _gamepadFocus;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void OpenMenu(bool animated = true)
    {
        gameObject.SetActive(true);
        _canvasGroup.DOFade(1, animated ? .3f : 0).OnComplete(() =>
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            EventSystem.current.SetSelectedGameObject(_gamepadFocus);
        }).SetUpdate(true);

    }

    public void CloseMenu(bool animated = true)
    {
        _canvasGroup.DOFade(0, animated ? .3f : 0).OnComplete(() =>
        {
            gameObject.SetActive(false);
        }).SetUpdate(true);
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        EventSystem.current.SetSelectedGameObject(null);
        if (!animated) gameObject.SetActive(false);
    }
}
