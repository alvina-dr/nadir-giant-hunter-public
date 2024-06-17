using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private float _timer;
    [SerializeField] private TextMeshProUGUI _pageNumber;
    private int _tutoIndex = 0;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _titleTextMeshPro;
    [SerializeField] private TextMeshProUGUI _explanationTextMeshPro;
    [SerializeField] private RectTransform _explanationLayout;
    [SerializeField] private UI_ExplanationEntry _explanationPrefab;

    private void Update()
    {
        if (GPCtrl.Instance.Player.PlayerInput.actions["Jump"].WasPerformedThisFrame())
        {
            NextPage();
            //_timer += Time.unscaledDeltaTime;
            //if (_timer > 2.0f)
            //{
            //    LaunchGame();
            //}
        } else
        {
            _timer = 0;
        }

        if (GPCtrl.Instance.Player.PlayerInput.actions["Attack"].WasPerformedThisFrame())
        {
            PreviousPage();
        }
    }

    public void LaunchGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void SetupTutorialScene()
    {
        GPCtrl.Instance.EnemySpawner.gameObject.SetActive(false);
        TutoPanel.OpenMenu(true);
        GPCtrl.Instance.UICtrl.TimerText.gameObject.SetActive(false);
        GPCtrl.Instance.UICtrl.KillRatioText.gameObject.SetActive(false);
        UpdatePage();
        GPCtrl.Instance.Pause = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GPCtrl.Instance.CameraThirdPerson.InputProvider.enabled = false;
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
        Canvas.ForceUpdateCanvases();
        _explanationLayout.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
        _explanationLayout.gameObject.GetComponent<VerticalLayoutGroup>().enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_explanationLayout);
    }

    public void CloseTutorial()
    {
        TutoPanel.CloseMenu();
        GPCtrl.Instance.Pause = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GPCtrl.Instance.CameraThirdPerson.InputProvider.enabled = true;
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
