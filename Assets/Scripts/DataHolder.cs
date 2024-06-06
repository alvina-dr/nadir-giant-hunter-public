using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    #region Singleton
    public static DataHolder Instance;

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