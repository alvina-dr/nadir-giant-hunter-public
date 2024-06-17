using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ExplanationEntry : MonoBehaviour
{
    [SerializeField] private UnityEngine.InputSystem.Samples.RebindUI.UI_CallToActionInput _input;
    [SerializeField] private TextMeshProUGUI _simpleText;

    public void SetupExplanation(TutorialData.TutorielText content)
    {
        if (content.Input == null)
        {
            _input.gameObject.SetActive(false);
        }
        else
        {
            _input._action = content.Input;
            _input.SetImage();
        }
        _simpleText.text = content.SimpleTextKey;
    }
}