using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorielCtrl : MonoBehaviour
{
    #region Singleton
    public static TutorielCtrl Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private float _timer;

    public void Start()
    {
        //show tuto panel
    }

    private void Update()
    {
        if (GPCtrl.Instance.Player.PlayerInput.actions["Attack"].IsPressed())
        {
            _timer += Time.unscaledDeltaTime;
            if (_timer > 2.0f)
            {
                LaunchGame();
            }
        } else
        {
            _timer = 0;
        }
    }

    public void LaunchGame()
    {
        SceneManager.LoadScene("Game");
    }
}
