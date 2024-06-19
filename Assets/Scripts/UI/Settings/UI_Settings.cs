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
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        if (PlayerPrefs.HasKey("Volume_Master"))
            AkSoundEngine.SetRTPCValue("Volume_Master", PlayerPrefs.GetInt("Volume_Master") * 5); //VOLUME MASTER
        else
        {
            PlayerPrefs.SetInt("Volume_Master", 100); 
            AkSoundEngine.SetRTPCValue("Volume_Master", 100);
        }

        if (PlayerPrefs.HasKey("Volume_Music"))
            AkSoundEngine.SetRTPCValue("Volume_Music", PlayerPrefs.GetInt("Volume_Music") * 5); //VOLUME MUSIC
        else
        {
            PlayerPrefs.SetInt("Volume_Music", 100);
            AkSoundEngine.SetRTPCValue("Volume_Music", 100);
        }

        if (PlayerPrefs.HasKey("Volume_SFX"))
            AkSoundEngine.SetRTPCValue("Volume_SFX", PlayerPrefs.GetInt("Volume_SFX") * 5); //VOLUME SFX
        else
        {
            PlayerPrefs.SetInt("Volume_SFX", 100);
            AkSoundEngine.SetRTPCValue("Volume_SFX", 100);
        }

        if (PlayerPrefs.HasKey("Volume_AMB"))
            AkSoundEngine.SetRTPCValue("Volume_AMB", PlayerPrefs.GetInt("Volume_AMB") * 5); //VOLUME AMB
        else
        {
            PlayerPrefs.SetInt("Volume_AMB", 100);
            AkSoundEngine.SetRTPCValue("Volume_AMB", 100);
        }

        if (PlayerPrefs.HasKey("Volume_UI"))
            AkSoundEngine.SetRTPCValue("Volume_UI", PlayerPrefs.GetInt("Volume_UI") * 5); //VOLUME UI
        else
        {
            PlayerPrefs.SetInt("Volume_UI", 100);
            AkSoundEngine.SetRTPCValue("Volume_UI", 100);
        }

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
