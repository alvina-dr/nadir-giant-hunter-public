using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.Rendering.DebugUI;
using System;

public class GPCtrl : MonoBehaviour
{
    #region Singleton
    public static GPCtrl Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            TargetableSpotList = FindObjectsByType<TargetableSpot>(FindObjectsSortMode.InstanceID).ToList();
        }
    }
    #endregion

    public GeneralData GeneralData;
    public Player Player;
    public UICtrl UICtrl;
    public EnemySpawner EnemySpawner;

    [Header("Camera")]
    public CameraThirdPerson CameraThirdPerson;
    public CameraLock CameraLock;
    public GameOverCamera GameOverCamera;

    [ReadOnly]
    public List<TargetableSpot> TargetableSpotList;
    [ReadOnly]
    public float Timer;
    [ReadOnly]
    public bool Pause = false;
    public bool GameOver = false;
    public bool DashPause = false;
    public CustomPassVolume reliefFX;
    public int NumEnemyKilled = 0;

    private void Update()
    {
        if (GameOver) return;
        if (Pause) return;
        Timer += Time.unscaledDeltaTime;
        double timerText = Math.Round(GPCtrl.Instance.Timer, 2, MidpointRounding.AwayFromZero);
        UICtrl.TimerText.text = timerText.ToString();
    }

    public void Win()
    {
        Debug.Log("WIN");
        GameOver = true;
        UICtrl.OpenEndGameMenu(true);
    }

    public void Loose(EnemyMovement enemy = null)
    {
        GameOver = true;
        Player.SoundData.SFX_Hunter_Death.Post(Player.gameObject);
        UICtrl.OpenEndGameMenu(false);
        if (enemy != null)
        {
            GameOverCamera.FocusEnemy(enemy);
        }
        Debug.Log("LOOSE");
    }

    public void AddKilledEnemy()
    {
        NumEnemyKilled++;
        if (NumEnemyKilled > EnemySpawner.SpawnerData.NumTotalEnemy)
        {
            Win();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [Button]
    public void Shake()
    {
        CameraThirdPerson.CameraShake.ShakeCamera(5, .5f);
    }

    public Material GetPostProcessMaterial()
    {
        foreach (var pass in reliefFX.customPasses)
        {
            if (pass is FullScreenCustomPass f)
                return f.fullscreenPassMaterial;
        }
        Debug.LogError("Custom error : No full screen pass material found in post process.");
        return null;
    }

    private void OnDestroy()
    {
        Material material = GetPostProcessMaterial();
        if (material != null)
        {
            material.SetFloat("_strength", 0);
        }
    }
}