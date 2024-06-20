using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialCtrl : MonoBehaviour
{
    #region Singleton
    public static TutorialCtrl Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            if (DataHolder.Instance.Tutorial)
            {
                SetupTutorialScene();
            } else
            {
                Destroy(gameObject);
            }
        }
    }
    #endregion
    public TutorialData TutorielData;
    [SerializeField] private UI_Menu TutoPanel;
    public Image FadeScreen;
    private float _timer;
    [SerializeField] private TextMeshProUGUI _pageNumber;
    private int _tutoIndex = 0;
    public bool TutoPanelOpen = false;
    [SerializeField] private GameObject _tutoMenu;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _titleTextMeshPro;
    [SerializeField] private TextMeshProUGUI _explanationTextMeshPro;
    [SerializeField] private RectTransform _explanationLayout;
    [SerializeField] private UI_ExplanationEntry _explanationPrefab;
    [SerializeField] private GameObject inputToExitTutorial;
    [SerializeField] private VideoPlayer _videoPlayer;

    private void Start()
    {
        FadeIn();
    }

    private void Update()
    {
        if (GPCtrl.Instance.Player.PlayerInput.actions["Jump"].WasPerformedThisFrame())
        {
            NextPage();
        }

        if (GPCtrl.Instance.Player.PlayerInput.actions["Attack"].WasPerformedThisFrame())
        {
            PreviousPage();
        }

        if (GPCtrl.Instance.Player.PlayerInput.actions["Jump"].IsPressed())
        {
            _timer += Time.unscaledDeltaTime;
            if (_timer > 2.0f)
            {
                _timer = -100;
                LaunchGame();
            }
        } else
        {
            _timer = 0;
        }
    }

    public void LaunchGame()
    {
        FadeOut();
        DOVirtual.DelayedCall(1, () =>
        {
            DataHolder.Instance.Tutorial = false;
            SceneManager.LoadScene("Game");
            AkSoundEngine.SetState("Music_State", "Game");
            AkSoundEngine.SetState("Pause", "Unpaused");
            AkSoundEngine.SetState("SlowMo", "NoSlowMo");
            GPCtrl.Instance.Player.SoundData.AMB_DeathZone_Enter.Stop(GPCtrl.Instance.Player.gameObject);
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

    public void SetupTutorialScene()
    {
        GPCtrl.Instance.EnemySpawner.gameObject.SetActive(false);
        GPCtrl.Instance.UICtrl.MonsterHighIndicator.HideIndicator();
        TutoPanel.OpenMenu(true);
        GPCtrl.Instance.UICtrl.TimerText.gameObject.SetActive(false);
        GPCtrl.Instance.UICtrl.KillRatioText.gameObject.SetActive(false);
        UpdatePage();
        GPCtrl.Instance.Pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GPCtrl.Instance.CameraThirdPerson.InputProvider.enabled = false;
        TutoPanelOpen = true;
    }

    public void NextPage()
    {
        _tutoIndex++;
        if (_tutoIndex >= TutorielData.entries.Count)
        {
            CloseTutorial();
        } else
        {
            UpdatePage();
        }

    }

    public void PreviousPage()
    {
        if (_tutoIndex > 0) _tutoIndex--;
        UpdatePage();
    }

    public void UpdatePage()
    {
        _titleTextMeshPro.text = LocalizationSettings.StringDatabase.GetLocalizedString("StringLocalization", TutorielData.entries[_tutoIndex].TitleKey);
        DestroyExplanationLayout();
        for (int i = 0; i < TutorielData.entries[_tutoIndex].Content.Count; i++)
        {
            UI_ExplanationEntry explanation = Instantiate(_explanationPrefab, _explanationLayout);
            explanation.SetupExplanation(TutorielData.entries[_tutoIndex].Content[i]);
        }
        _pageNumber.text = "<- " + (_tutoIndex + 1).ToString() + "/" + TutorielData.entries.Count + " ->";
        _videoPlayer.clip = TutorielData.entries[_tutoIndex].Video;
        Canvas.ForceUpdateCanvases();
        _explanationLayout.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
        _explanationLayout.gameObject.GetComponent<VerticalLayoutGroup>().enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_explanationLayout);
    }

    public void CloseTutorial()
    {
        TutoPanel.CloseMenu(false);
        gameObject.SetActive(true);
        GPCtrl.Instance.Pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GPCtrl.Instance.CameraThirdPerson.InputProvider.enabled = true;
        inputToExitTutorial.SetActive(true);
        _tutoMenu.SetActive(false);
        TutoPanelOpen = false;
    }

    public void DestroyExplanationLayout()
    {
        for (int i = 0; i < _explanationLayout.childCount; i++)
        {
            Destroy(_explanationLayout.GetChild(i).gameObject);
        }
    }

    public void LaunchRealGame()
    {
        GPCtrl.Instance.EnemySpawner.gameObject.SetActive(true);
    }
}
