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
    [SerializeField] TextMeshProUGUI _speedText;
    [SerializeField] TextMeshProUGUI _fpsText;
    float _deltaTime = 0;

    private void Awake()
    {
        DM.Input = new DMCustomInput();
    }

    private void Start()
    {
        SetupDebugMenu();
    }

    private void Update()
    {
        _speedText.text = "player speed : " + GPCtrl.Instance.Player.Rigibody.velocity.magnitude.ToString();
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;
        _fpsText.text = "fps : " + fps;
    }

    public void SetupDebugMenu()
    {
        DM.Root.Clear();
        DM.Add("Debug/ReloadScene", action => ReloadScene());
        DM.Add("Debug/Win", action => GPCtrl.Instance.Win());
        DM.Add("Debug/Loose", action => GPCtrl.Instance.Loose());
        DM.Add("Debug/Reset Scoreboard", action => PlayerPrefs.DeleteKey("scoreboard"));
        DM.Add("Grapple/StartCurveBoost", () => GPCtrl.Instance.Player.Data.startCurveBoost, v => GPCtrl.Instance.Player.Data.startCurveBoost = v);
        DM.Add("Grapple/EndCurveBoost", () => GPCtrl.Instance.Player.Data.endCurveBoost, v => GPCtrl.Instance.Player.Data.endCurveBoost = v);
        DM.Add("Debug/BouncingGround", () => GPCtrl.Instance.GeneralData.debugBouncingGround, v => GPCtrl.Instance.GeneralData.debugBouncingGround = v);
        DM.Add("Values/PlayerSpeed", action => _speedText.gameObject.SetActive(!_speedText.gameObject.activeSelf));
        DM.Add("Values/FPS", action => _fpsText.gameObject.SetActive(!_fpsText.gameObject.activeSelf));
        //DN.Notify("Simple notification", 5f);
#if UNITY_EDITOR
        _speedText.gameObject.SetActive(true);
        _fpsText.gameObject.SetActive(true);
#endif
    }

    #region DEBUG FUNCTIONS
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion
}