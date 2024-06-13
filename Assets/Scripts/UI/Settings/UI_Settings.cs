using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private Toggle _toggleFullScreen;
    [SerializeField] private Toggle _toggleCameraShake;
    [SerializeField] private Toggle _toggleVibration;
    [SerializeField] private List<CanvasGroup> _subMenuList;
    [SerializeField] private List<UI_Button> _buttonList;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Volume_Master"))
            AkSoundEngine.SetRTPCValue("Volume_Master", PlayerPrefs.GetInt("Volume_Master") * 5); //VOLUME MASTER
        if (PlayerPrefs.HasKey("Volume_Music"))
            AkSoundEngine.SetRTPCValue("Volume_Music", PlayerPrefs.GetInt("Volume_Music") * 5); //VOLUME MUSIC
        if (PlayerPrefs.HasKey("Volume_SFX"))
            AkSoundEngine.SetRTPCValue("Volume_SFX", PlayerPrefs.GetInt("Volume_SFX") * 5); //VOLUME SFX
        if (PlayerPrefs.HasKey("Volume_AMB"))
            AkSoundEngine.SetRTPCValue("Volume_AMB", PlayerPrefs.GetInt("Volume_AMB") * 5); //VOLUME AMB
        if (PlayerPrefs.HasKey("Volume_UI"))
            AkSoundEngine.SetRTPCValue("Volume_UI", PlayerPrefs.GetInt("Volume_UI") * 5); //VOLUME UI

        if (PlayerPrefs.HasKey("ScreenResolution")) //SCREEN RESOLUTION
        {
            int indexResolution = PlayerPrefs.GetInt("ScreenResolution");
            Screen.SetResolution(Screen.resolutions[indexResolution].width, Screen.resolutions[indexResolution].height, Screen.fullScreenMode);
        }

        if (PlayerPrefs.HasKey("Fullscreen")) //FULLSCREEN
        {
            Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen") == 0 ? false : true;
        }
        _toggleFullScreen.isOn = PlayerPrefs.GetInt("Fullscreen") == 0 ? false : true;

        if (!PlayerPrefs.HasKey("CameraShake")) PlayerPrefs.SetInt("CameraShake", 1); //CAMERA SHAKE
        _toggleCameraShake.isOn = PlayerPrefs.GetInt("CameraShake") == 0 ? false : true;

        _toggleVibration.isOn = PlayerPrefs.GetInt("Vibration") == 0 ? false : true; //GAMEPAD VIBRATION

        int indexLanguage = 0;
        if (PlayerPrefs.HasKey("Language")) indexLanguage = PlayerPrefs.GetInt("Language");
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[indexLanguage];
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
        int index = 0;
        for (int i = 0; i < _subMenuList.Count; i++)
        {
            if (_subMenuList[i].gameObject.activeSelf) HideSubMenu(_subMenuList[i]);
            if (_subMenuList[i] == group) index = i;
        }
        ShowSubMenu(group);
        ResetNavbar();
        _buttonList[index].Activate();
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
        ResetNavbar();
    }

    public void ResetNavbar()
    {
        for (int i = 0; i < _buttonList.Count; i++)
        {
            _buttonList[i].Deactivate();
        }
    }
}
