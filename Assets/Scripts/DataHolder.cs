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
        }
    }
#endregion

    public RumbleManager RumbleManager;

    [Header("SFX")]
    public AK.Wwise.Event _sfxGoBack;

}