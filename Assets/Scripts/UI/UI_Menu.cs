using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;

public class UI_Menu : MonoBehaviour
{
    [SerializeField] private GameObject _gamepadFocus;
    [SerializeField] private CanvasGroup _canvasGroup;
    public UnityEvent GoBackEvent;

    private void Awake()
    {
        GoBackEvent.AddListener(GoBack);
    }

    public void GoBack()
    {
        DataHolder.Instance._sfxGoBack.Post(DataHolder.Instance.gameObject);
    }

    public void OpenMenu(bool animated = true)
    {
        gameObject.SetActive(true);
        _canvasGroup.DOFade(1, animated ? .3f : 0).OnComplete(() =>
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            if (Gamepad.current != null & Gamepad.all.Count > 0)
                EventSystem.current.SetSelectedGameObject(_gamepadFocus);
        }).SetUpdate(true);

    }

    public void CloseMenu(bool animated = true)
    {
        if (!animated)
        {
            gameObject.SetActive(false);
            _canvasGroup.alpha = 0;
        }
        else
        {
            _canvasGroup.DOFade(0, animated ? .3f : 0).OnComplete(() =>
            {
                gameObject.SetActive(false);
            }).SetUpdate(true);
        }
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ChangeGamepadFirstFocus(GameObject focusGameObject)
    {
        _gamepadFocus = focusGameObject;
    }

    void Update()
    {
        //to get back focus for gamepad after you use the mouse
        if (Gamepad.current == null || Gamepad.all.Count == 0) return;
        var gamepadButtonPressed = Gamepad.current.allControls.Any(x => x is ButtonControl button && x.IsPressed() && !x.synthetic);
        if (EventSystem.current.currentSelectedGameObject == null && gamepadButtonPressed)
        {
            EventSystem.current.SetSelectedGameObject(_gamepadFocus);
        }

        if (_canvasGroup.interactable)
        {
            var uiModule = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
            var cancel = uiModule.cancel.action;

            if (cancel.WasPressedThisFrame())
            {
                GoBackEvent?.Invoke();
            }
        }
    }
}
