using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UICtrl : MonoBehaviour
{
    public UI_Menu EndGameMenu;
    public UI_InputIndication AttackInputIndication;
    public UI_Settings UI_Settings;
    public UI_Menu PauseMenu;

    [Header("End Game Menu")]
    [SerializeField] private TextMeshProUGUI _endGameMenuTitle;
    public void OpenPauseMenu()
    {
        PauseMenu.OpenMenu();
        GPCtrl.Instance.Pause = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GPCtrl.Instance.CameraThirdPerson.InputProvider.enabled = false;
    }

    public void ClosePauseMenu()
    {
        GPCtrl.Instance.Pause = false;
        Time.timeScale = 1;
        PauseMenu.CloseMenu();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GPCtrl.Instance.CameraThirdPerson.InputProvider.enabled = true;
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

    public void OpenEndGameMenu(bool hasWon)
    {
        _endGameMenuTitle.text = hasWon ? "Victory" : "Game Over";
        EndGameMenu.OpenMenu();
    }
}
