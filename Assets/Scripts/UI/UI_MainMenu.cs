using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private UI_Menu _mainMenu;
    [SerializeField] private AK.Wwise.Event _sfxStartGame;

    private void Awake()
    {
        List<UI_Menu> menuList = FindObjectsByType<UI_Menu>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        for (int i = 0; i < menuList.Count; i++)
        {
            menuList[i].CloseMenu(false);
        }
        _mainMenu.OpenMenu(true);

        //load all settings from player prefs

        if (PlayerPrefs.HasKey("ScreenResolution"))
        {
            int index = PlayerPrefs.GetInt("ScreenResolution");
            Screen.SetResolution(Screen.resolutions[index].width, Screen.resolutions[index].height, Screen.fullScreenMode);
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen") == 0 ? false : true;
        }

        if (!PlayerPrefs.HasKey("CameraShake")) PlayerPrefs.SetInt("CameraShake", 1);
    }

    public void StartGame()
    {
        _sfxStartGame.Post(DataHolder.Instance.gameObject);
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
