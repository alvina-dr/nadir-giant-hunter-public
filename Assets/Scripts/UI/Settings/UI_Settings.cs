using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Slider _camSensiSlider;
    [SerializeField] private List<CanvasGroup> _subMenuList;

    private void Start()
    {
        _toggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 0 ? false : true;
        _camSensiSlider.value = PlayerPrefs.GetFloat("CamSensi");
    }

    public void SetFullscreeen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    public void SelectPage(CanvasGroup group)
    {
        for (int i = 0; i < _subMenuList.Count; i++)
        {
            if (_subMenuList[i].interactable) HideSubMenu(_subMenuList[i]);
        }
        ShowSubMenu(group);
    }

    public void HideSubMenu(CanvasGroup group)
    {
        group.gameObject.SetActive(false);
        group.interactable = false;
        group.blocksRaycasts = false;
        group.DOFade(0, .3f).OnComplete(() =>
        {
        }).SetUpdate(true);
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
}
