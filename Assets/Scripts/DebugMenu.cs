using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extDebug.Menu;
using UnityEngine.SceneManagement;
using extDebug.Notifications;
using UnityEngine.UI;
using TMPro;

public class DebugMenu : MonoBehaviour
{
    Color _color;
    float _playerSpeed;
    [SerializeField] TextMeshProUGUI _speedText;
    [SerializeField] TextMeshProUGUI _fpsText;
    float _deltaTime = 0;

    private void Start()
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
        _speedText.text = "player speed : " + GPCtrl.Instance.Player.Rigibody.velocity.magnitude.ToString();
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;
        _fpsText.text = "fps : " + fps;
    }

    public void SetupDebugMenu()
    {
        DM.Root.Clear();
        DM.Add("Print/HelloWorld", action => Debug.Log("Hello World"));
        DM.Add("Debug/ReloadScene", action => ReloadScene());
        DM.Add("Grapple/StartCurveBoost", () => GPCtrl.Instance.Player.Data.startCurveBoost, v => GPCtrl.Instance.Player.Data.startCurveBoost = v);
        DM.Add("Grapple/EndCurveBoost", () => GPCtrl.Instance.Player.Data.endCurveBoost, v => GPCtrl.Instance.Player.Data.endCurveBoost = v);
        DM.Add("Grapple/EndCurveBoost", () => GPCtrl.Instance.Player.Data.endCurveBoost, v => GPCtrl.Instance.Player.Data.endCurveBoost = v);
        DM.Add("Values/PlayerSpeed", action => _speedText.gameObject.SetActive(!_speedText.gameObject.activeSelf));
        DM.Add("Values/FPS", action => _fpsText.gameObject.SetActive(!_fpsText.gameObject.activeSelf));
        //DN.Notify("Simple notification", 5f);
    }

    #region DEBUG FUNCTIONS
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion
}
