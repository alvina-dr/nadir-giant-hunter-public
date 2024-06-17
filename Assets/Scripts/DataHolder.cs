using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    #region Singleton
    public static DataHolder Instance;
    public enum DifficultyMode
    {
        Easy = 0,
        Normal = 1,
        Hard = 2
    }

    public DifficultyMode CurrentDifficulty;
    public bool Tutorial = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Bank.Load();
            _music.Post(gameObject);
            AkSoundEngine.SetState("Music_State", "MainMenu");
            //Debug.Log("load bank : " + result.ToString());
        }
    }
#endregion

    public RumbleManager RumbleManager;

    [Header("SFX")]
    public AK.Wwise.Event _sfxGoBack;
    public AK.Wwise.Event _music;
    public AK.Wwise.Bank Bank;

    public string ConvertTimeToMinutes(float time)
    {
        string minutes = TimeSpan.FromSeconds(time).Minutes.ToString();
        string seconds = TimeSpan.FromSeconds(time).Seconds.ToString();
        if (seconds.Length == 1) seconds = "0" + seconds;
        string miliseconds = TimeSpan.FromSeconds(time).Milliseconds.ToString();
        if (miliseconds.Length > 2) miliseconds.Substring(0, 2);
        if (miliseconds.Length == 2) miliseconds += "0";
        return (minutes + ":" + seconds + ":" + miliseconds);
    }

    public bool IsUsingGamepad()
    {
        string control = GPCtrl.Instance.Player.PlayerInput.currentControlScheme.ToString();
        if (control == "Gamepad")
        {
            return true;
        } else
        {
            return false;
        }
    }
}