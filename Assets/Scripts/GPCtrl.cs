using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.HighDefinition;

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
    public bool DashPause = false;
    public CustomPassVolume reliefFX;

    private void Update()
    {
        Timer += Time.deltaTime; 
        if (Timer > GeneralData.levelMaxTime)
        {
            //stop monster spawn
            //if no monster then win
            if (TargetableSpotList.Count == 0)
                Win();
        }
    }

    public void Win()
    {
        Debug.Log("WIN");
    }

    public void Loose(EnemyMovement enemy = null)
    {
        Pause = true;
        UICtrl.EndGameMenu.OpenMenu();
        if (enemy != null)
        {
            GameOverCamera.FocusEnemy(enemy);
        }
        Debug.Log("LOOSE");
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
}
