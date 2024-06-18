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
    [SerializeField] private AK.Wwise.Event _sfxStartGame;
    public Image FadeScreen;

    private void Start()
    {
        FadeIn();
        FindObjectOfType<UI_Settings>().UpdateSettings();
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
        FadeOut();
        DOVirtual.DelayedCall(1, () =>
        {
            _sfxStartGame.Post(DataHolder.Instance.gameObject);
            DataHolder.Instance.CurrentDifficulty = (DataHolder.DifficultyMode) difficulty;
            SceneManager.LoadScene("Game");
            if (DataHolder.Instance.Tutorial)
            {
                AkSoundEngine.SetState("Music_State", "Silence");
            }
            else
            {
                AkSoundEngine.SetState("Music_State", "Game");
            }
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