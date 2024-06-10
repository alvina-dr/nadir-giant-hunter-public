using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EndGameMenu : MonoBehaviour
{
    [SerializeField] private UI_Menu _menu;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private UI_Button _buttonNext;
    [SerializeField] private UI_Button _buttonTryAgain;
    [SerializeField] private UI_Button _buttonMainMenu;
    [SerializeField] private AK.Wwise.Event _sfxWin;
    [SerializeField] private AK.Wwise.Event _sfxLoose;

    public void Win()
    {
        _resultText.text = "Victory";
        _scoreText.gameObject.SetActive(true);
        _scoreText.text = Math.Round(GPCtrl.Instance.Timer, 2, MidpointRounding.AwayFromZero).ToString();
        _buttonNext.gameObject.SetActive(true);
        //if score high enough to be in leaderboard then don't show those buttons
        _nameInputField.gameObject.SetActive(true);
        _buttonTryAgain.gameObject.SetActive(false);
        _buttonMainMenu.gameObject.SetActive(false);
        _menu.ChangeGamepadFirstFocus(_nameInputField.gameObject);
        _menu.OpenMenu();
        _sfxWin.Post(DataHolder.Instance.gameObject);
    }

    public void Loose()
    {
        _resultText.text = "Game over";
        _scoreText.gameObject.SetActive(false);
        _buttonNext.gameObject.SetActive(false);
        _nameInputField.gameObject.SetActive(false);
        _buttonTryAgain.gameObject.SetActive(true);
        _buttonMainMenu.gameObject.SetActive(true);
        _menu.ChangeGamepadFirstFocus(_buttonTryAgain.gameObject);
        _menu.OpenMenu();
        _sfxLoose.Post(DataHolder.Instance.gameObject);
    }
}