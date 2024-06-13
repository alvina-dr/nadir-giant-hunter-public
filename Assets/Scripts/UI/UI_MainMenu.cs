using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private UI_Menu _mainMenu;
    [SerializeField] private AK.Wwise.Event _sfxStartGame;

    private void Start()
    {
        List<UI_Menu> menuList = FindObjectsByType<UI_Menu>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        for (int i = 0; i < menuList.Count; i++)
        {
            menuList[i].CloseMenu(false);
        }
        _mainMenu.OpenMenu(true);
    }

    public void StartGameWithTuto()
    {
        _sfxStartGame.Post(DataHolder.Instance.gameObject);
        SceneManager.LoadScene("Tutoriel");
    }

    public void StartGame(int difficulty)
    {
        _sfxStartGame.Post(DataHolder.Instance.gameObject);
        DataHolder.Instance.CurrentDifficulty = (DataHolder.DifficultyMode) difficulty;
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}