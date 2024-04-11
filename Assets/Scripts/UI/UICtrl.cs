using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UICtrl : MonoBehaviour
{
    public UI_EndGameMenu EndGameMenu;
    public UI_InputIndication AttackInputIndication;
    public UI_Settings UI_Settings;
    public UI_Menu PauseMenu;

    public void OpenPauseMenu()
    {
        PauseMenu.OpenMenu();
        //here pause time
    }

    public void ClosePauseMenu()
    {
        PauseMenu.CloseMenu();
        //make time normal again
    }

    public void CallPause(InputAction.CallbackContext context)
    {
        if (GPCtrl.Instance.Pause) ClosePauseMenu();
        else OpenPauseMenu();
    }
}
