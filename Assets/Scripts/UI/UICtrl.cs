using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UICtrl : MonoBehaviour
{
    public UI_Menu EndGameMenu;
    public UI_InputIndication AttackInputIndication;
    public UI_Settings UI_Settings;
    public UI_Menu PauseMenu;

    public void OpenPauseMenu()
    {
        PauseMenu.OpenMenu();
        GPCtrl.Instance.Pause = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void ClosePauseMenu()
    {
        GPCtrl.Instance.Pause = false;
        Time.timeScale = 1;
        PauseMenu.CloseMenu();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CallPause()
    {
        if (GPCtrl.Instance.Pause) ClosePauseMenu();
        else OpenPauseMenu();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void TryAgain()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }
}
