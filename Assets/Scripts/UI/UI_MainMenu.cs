using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private UI_Menu _mainMenu;
    [SerializeField] private UI_Settings Settings;
    [SerializeField] private AK.Wwise.Event _sfxStartGame;

    private void Start()
    {
        Settings.UpdateSettings();
        List<UI_Menu> menuList = FindObjectsByType<UI_Menu>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        for (int i = 0; i < menuList.Count; i++)
        {
            menuList[i].CloseMenu(false);
        }
        _mainMenu.OpenMenu(true);
        DataHolder.Instance.Tutorial = true;
    }

    public void StartGame(int difficulty)
    {
        _sfxStartGame.Post(DataHolder.Instance.gameObject);
        DataHolder.Instance.CurrentDifficulty = (DataHolder.DifficultyMode) difficulty;
        SceneManager.LoadScene("Game");
        DataHolder.Instance.Tutorial = true;
        if (DataHolder.Instance.Tutorial)
        {
            AkSoundEngine.SetState("Music_State", "Silence");
        }
        else
        {
            AkSoundEngine.SetState("Music_State", "Game");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}