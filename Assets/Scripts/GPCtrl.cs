using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

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
            WeakSpotList = FindObjectsByType<WeakSpot>(FindObjectsSortMode.InstanceID).ToList();
        }
    }

    #endregion

    public GeneralData GeneralData;
    public Player Player;
    public CameraThirdPerson CameraThirdPerson;
    public CameraLock CameraLock;
    public UICtrl UICtrl;
    [ReadOnly]
    public List<WeakSpot> WeakSpotList;
    [ReadOnly]
    public float Timer;
    [ReadOnly]
    public bool Pause = false;
    private void Update()
    {
        Timer += Time.deltaTime; 
        if (Timer > GeneralData.levelMaxTime)
        {
            //stop monster spawn
            //if no monster then win
            if (WeakSpotList.Count == 0)
                Win();
        }
    }

    public void Win()
    {
        Debug.Log("WIN");
    }

    public void Loose()
    {
        Pause = true;
        UICtrl.EndGameMenu.OpenMenu();
        Debug.Log("LOOSE");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
