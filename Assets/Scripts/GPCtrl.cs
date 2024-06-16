using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.Rendering.DebugUI;
using System;
using Enemies;

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
    public GameOverCamera GameOverCamera;

    [Header("Difficulty")]
    public EnemySpawnerData EasySpawnerData;
    public EnemyData EasyEnemyData;
    public EnemySpawnerData NormalSpawnerData;
    public EnemyData NormalEnemyData;
    public EnemySpawnerData HardSpawnerData;
    public EnemyData HardEnemyData;

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

    private void Start()
    {
        switch (DataHolder.Instance.CurrentDifficulty)
        {
            case DataHolder.DifficultyMode.Easy:
                EnemySpawner.SpawnerData = EasySpawnerData;
                Player.DifficultyData = Player.EasyDifficultyData;
                break;
            case DataHolder.DifficultyMode.Normal:
                EnemySpawner.SpawnerData = NormalSpawnerData;
                Player.DifficultyData = Player.NormalDifficultyData;

                break;
            case DataHolder.DifficultyMode.Hard:
                EnemySpawner.SpawnerData = HardSpawnerData;
                Player.DifficultyData = Player.HardDifficultyData;
                break;
        }
        Material postProcess = GetPostProcessMaterial();
        if (PlayerPrefs.HasKey("enableHitframeFX"))
            postProcess.SetFloat("_enable_hitrame_FX", PlayerPrefs.GetInt("_enable_hitrame_FX"));
        else postProcess.SetFloat("_enable_hitrame_FX", 1);
    }

    private void Update()
    {
        Material postProcess = GetPostProcessMaterial();
        if (postProcess != null) postProcess.SetFloat("_unscaled_time", postProcess.GetFloat("_unscaled_time") + Time.unscaledDeltaTime);
        UICtrl.MonsterHighIndicator.SetUnscaledTime();
        UICtrl.AttackInputIndicator.SetUnscaledTime();
        if (GameOver) return;
        if (Pause) return;
        Timer += Time.unscaledDeltaTime;
        UICtrl.TimerText.text = DataHolder.Instance.ConvertTimeToMinutes(Timer);
        //UICtrl.SpeedText.text = MathF.Round(Player.Rigibody.velocity.magnitude / 2).ToString() + " m/s";
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
        UICtrl.KillRatioText.text = NumEnemyKilled.ToString() + " / " + EnemySpawner.SpawnerData.NumTotalEnemy.ToString();
        if (NumEnemyKilled >= EnemySpawner.SpawnerData.NumTotalEnemy)
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
            material.SetFloat("_Hit_by_Abyss_Time", 0);
            material.SetFloat("_Timefactor_Hitframe_Attack_Bumper", 1);
            material.SetFloat("_Timefactor_Hitframe_Attack_Dashspot", 1);
            material.SetFloat("_Timefactor_Hitframe_Attack_Weakspot", 1);
            material.SetFloat("_Timefactor_Hitframe_Input_Bumper", 1);
            material.SetFloat("_Timefactor_Hitframe_Input_Dashspot", 1);
            material.SetFloat("_Timefactor_Hitframe_Input_Weakspot", 1);
        }
    }
}