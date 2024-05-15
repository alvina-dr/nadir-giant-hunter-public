using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extDebug.Menu;
using UnityEngine.SceneManagement;
using extDebug.Notifications;
public class DebugMenu : MonoBehaviour
{
    Color _color;
    float _playerSpeed;
    string _wtf = "blabla";
    private void Awake()
    {
        SetupDebugMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (DM.IsVisible) DM.Back();
            else DM.Open();
        }
        _wtf = GPCtrl.Instance.Player.Rigibody.velocity.magnitude.ToString();
        DN.Notify("Speed : " + _wtf, 1);
    }

    public void SetupDebugMenu()
    {
        DM.Root.Clear();
        DM.Add("Print/HelloWorld", action => Debug.Log("Hello World"));
        DM.Add("Debug/ReloadScene", action => ReloadScene());
        DM.Add("Grapple/StartCurveBoost", () => GPCtrl.Instance.Player.Data.startCurveBoost, v => GPCtrl.Instance.Player.Data.startCurveBoost = v);
        DM.Add("Grapple/EndCurveBoost", () => GPCtrl.Instance.Player.Data.endCurveBoost, v => GPCtrl.Instance.Player.Data.endCurveBoost = v);
        DM.Add("Grapple/EndCurveBoost", () => GPCtrl.Instance.Player.Data.endCurveBoost, v => GPCtrl.Instance.Player.Data.endCurveBoost = v);
        //DM.Add("Values/Color", () => _color, v => _color = v, order: 18).SetPrecision(2);
        DM.Add("Values/PlayerSpeed", () => _playerSpeed, v => _playerSpeed = GPCtrl.Instance.Player.Rigibody.velocity.magnitude);
        DM.Add("Values/Blabla", () => _wtf);
        DN.Notify("Simple notification", 5f);
    }

    #region DEBUG FUNCTIONS
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion
}
