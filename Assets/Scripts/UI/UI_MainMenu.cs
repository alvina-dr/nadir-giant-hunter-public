using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private UI_Menu _mainMenu;
    [SerializeField] private UI_Settings Settings;
    [SerializeField] private AK.Wwise.Event _sfxStartGame;
    public Image FadeScreen;

    private void Start()
    {
        FadeIn();
        Settings.UpdateSettings();
        List<UI_Menu> menuList = FindObjectsByType<UI_Menu>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        for (int i = 0; i < menuList.Count; i++)
        {
            menuList[i].CloseMenu(false);
        }
        _mainMenu.OpenMenu(true);
        DataHolder.Instance.Tutorial = true;
        //Debug.LogError("Open console");
    }

    public void StartGame(int difficulty)
    {
        FadeOut();
        _sfxStartGame.Post(DataHolder.Instance.gameObject);
        DataHolder.Instance.Tutorial = true;
        if (DataHolder.Instance.Tutorial)
        {
            AkSoundEngine.SetState("Music_State", "Silence");
        }
        else
        {
            AkSoundEngine.SetState("Music_State", "Game");
        }
        DOVirtual.DelayedCall(1, () =>
        {
            DataHolder.Instance.CurrentDifficulty = (DataHolder.DifficultyMode) difficulty;
            SceneManager.LoadScene("Game");
        });
    }

    public void FadeIn()
    {
        FadeScreen.color = new Color(FadeScreen.color.r, FadeScreen.color.g, FadeScreen.color.b, 1);
        FadeScreen.DOFade(0, 1);
    }

    public void FadeOut()
    {
        FadeScreen.color = new Color(FadeScreen.color.r, FadeScreen.color.g, FadeScreen.color.b, 0);
        FadeScreen.DOFade(1, 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}