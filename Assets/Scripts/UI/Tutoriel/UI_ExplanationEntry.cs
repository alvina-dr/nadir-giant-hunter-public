using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class UI_ExplanationEntry : MonoBehaviour
{
    [SerializeField] private UnityEngine.InputSystem.Samples.RebindUI.UI_CallToActionInput _input;
    [SerializeField] private TextMeshProUGUI _simpleText;

    public void SetupExplanation(TutorialData.TutorielText content)
    {
        _simpleText.text = LocalizationSettings.StringDatabase.GetLocalizedString("StringLocalization", content.SimpleTextKey);
        RectTransform rect = _simpleText.transform as RectTransform;
        if (content.Input == null)
        {
            _input.gameObject.SetActive(false);
            rect.sizeDelta = new Vector2(580, rect.sizeDelta.y);
        }
        else
        {
            _input._action = content.Input;
            _input.SetImage();
            rect.sizeDelta = new Vector2(485, rect.sizeDelta.y);
        }
    }
}