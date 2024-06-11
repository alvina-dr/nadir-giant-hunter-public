using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private Toggle _toggleFullScreen;
    [SerializeField] private Toggle _toggleCameraShake;
    [SerializeField] private Toggle _toggleVibration;
    [SerializeField] private Slider _camSensiSlider;
    [SerializeField] private List<CanvasGroup> _subMenuList;

    private void Start()
    {
        _toggleFullScreen.isOn = PlayerPrefs.GetInt("Fullscreen") == 0 ? false : true;
        _toggleCameraShake.isOn = PlayerPrefs.GetInt("CameraShake") == 0 ? false : true;
        _toggleVibration.isOn = PlayerPrefs.GetInt("Vibration") == 0 ? false : true;
        _camSensiSlider.value = PlayerPrefs.GetFloat("CamSensi");
    }

    public void SetFullscreeen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    public void SetCameraShake(bool value)
    {
        PlayerPrefs.SetInt("CameraShake", value ? 1 : 0);
    }

    public void SetVibration(bool value)
    {
        PlayerPrefs.SetInt("Vibration", value ? 1 : 0);
    }

    public void SelectPage(CanvasGroup group)
    {
        for (int i = 0; i < _subMenuList.Count; i++)
        {
            if (_subMenuList[i].gameObject.activeSelf) HideSubMenu(_subMenuList[i]);
        }
        ShowSubMenu(group);
    }

    public void HideSubMenu(CanvasGroup group)
    {
        group.gameObject.SetActive(false);
        group.interactable = false;
        group.blocksRaycasts = false;
        group.alpha = 0f;
    }

    public void ShowSubMenu(CanvasGroup group)
    {
        group.gameObject.SetActive(true);
        group.DOFade(1, .3f).OnComplete(() =>
        {
            group.blocksRaycasts = true;
            group.interactable = true;
        }).SetUpdate(true);
    }

    public void ResetMenu()
    {
        for (int i = 0; i < _subMenuList.Count; i++)
        {
            HideSubMenu(_subMenuList[i]);
        }
    }
}
