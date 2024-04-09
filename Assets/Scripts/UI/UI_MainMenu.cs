using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private UI_Menu _mainMenu;

    private void Awake()
    {
        List<UI_Menu> menuList = FindObjectsByType<UI_Menu>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        for (int i = 0; i < menuList.Count; i++)
        {
            menuList[i].CloseMenu(false);
        }
        _mainMenu.OpenMenu(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
