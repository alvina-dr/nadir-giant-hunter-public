using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UICtrl : MonoBehaviour
{
    [Header("Menu")]
    public UI_EndGameMenu EndGameMenu;
    public UI_Settings UI_Settings;
    public UI_Menu PauseMenu;
    public Image FadeScreen;

    [Header("Indicators")]
    public UI_MovingIndicator AttackInputIndicator;
    public UI_MovingIndicator MonsterHighIndicator;
    public CanvasGroup PlayerLowIndicator;
    public UI_MovingIndicator SwingRightIndicator;
    public UI_MovingIndicator SwingLeftIndicator;

    [Header("Input")]
    public UI_InputIndicator AttackInput;
    public UI_InputIndicator SwingLeftInput;
    public UI_InputIndicator SwingRightInput;
    public Image WeakSpotContextUI;
    public Image BumperContextUI;
    public Image DashContextUI;

    [Header("In game UI")]
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI KillRatioText;

    public AK.Wwise.Event _pauseMenuMusic;

    [Header("End Game Menu")]
    [SerializeField] private TextMeshProUGUI _endGameMenuTitle;

    public void OpenPauseMenu()
    {
        PauseMenu.OpenMenu();
        GPCtrl.Instance.Pause = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GPCtrl.Instance.CameraThirdPerson.InputProvider.enabled = false;
        AkSoundEngine.SetState("Pause", "Paused");
        _pauseMenuMusic.Post(gameObject);
    }

    public void ClosePauseMenu()
    {
        GPCtrl.Instance.Pause = false;
        UI_Settings.GetComponent<UI_Menu>().CloseMenu(true);
        if (!GPCtrl.Instance.DashPause)
        {
            Time.timeScale = 1;
        }
        PauseMenu.CloseMenu();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GPCtrl.Instance.CameraThirdPerson.InputProvider.enabled = true;
        AkSoundEngine.SetState("Pause", "Unpaused");
        _pauseMenuMusic.Stop(gameObject);
    }

    public void CallPause()
    {
        if (GPCtrl.Instance.GameOver) return;
        if (GPCtrl.Instance.Pause) ClosePauseMenu();
        else OpenPauseMenu();
    }

    public void FadeIn()
    {
        FadeScreen.color = new Color(FadeScreen.color.r, FadeScreen.color.g, FadeScreen.color.b, 1);
        FadeScreen.DOFade(0, 1).SetUpdate(true);
    }

    public void FadeOut()
    {
        FadeScreen.color = new Color(FadeScreen.color.r, FadeScreen.color.g, FadeScreen.color.b, 0);
        FadeScreen.DOFade(1, 1).SetUpdate(true);
    }

    public void BackToMainMenu()
    {
        PauseMenu.GetComponent<CanvasGroup>().interactable = false;
        FadeOut();
        DOVirtual.DelayedCall(1, () =>
        {
            SceneManager.LoadScene("MainMenu");
            Time.timeScale = 1;
            AkSoundEngine.SetState("Music_State", "MainMenu");
            AkSoundEngine.SetState("Pause", "Unpaused");
            AkSoundEngine.SetState("SlowMo", "NoSlowMo");
            GPCtrl.Instance.Player.SoundData.AMB_DeathZone_Enter.Stop(GPCtrl.Instance.Player.gameObject);
        });
    }

    public void TryAgain()
    {
        PauseMenu.GetComponent<CanvasGroup>().interactable = false;
        FadeOut();
        DOVirtual.DelayedCall(1, () =>
        {
            SceneManager.LoadScene("Game");
            if (DataHolder.Instance.Tutorial)
            {
                AkSoundEngine.SetState("Music_State", "Silence");
            }
            else
            {
                AkSoundEngine.SetState("Music_State", "Game");
            }
            Time.timeScale = 1;
            AkSoundEngine.SetState("Pause", "Unpaused");
            AkSoundEngine.SetState("SlowMo", "NoSlowMo");
            GPCtrl.Instance.Player.SoundData.AMB_DeathZone_Enter.Stop(GPCtrl.Instance.Player.gameObject);
        });
    }

    public void OpenEndGameMenu(bool hasWon)
    {
        if (hasWon) EndGameMenu.Win();
        else EndGameMenu.Loose();
        GPCtrl.Instance.Pause = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GPCtrl.Instance.CameraThirdPerson.InputProvider.enabled = false;
        AkSoundEngine.SetState("Music_State", "Silence");
        AkSoundEngine.SetState("Game_State", "Over");
    }

    private void Start()
    {
        FadeIn();
        KillRatioText.text = GPCtrl.Instance.NumEnemyKilled.ToString() + " / " + GPCtrl.Instance.EnemySpawner.SpawnerData.NumTotalEnemy.ToString();
    }
}