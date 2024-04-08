using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EndGameMenu : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    public void OpenMenu()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        _canvasGroup.alpha = 1.0f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
}
