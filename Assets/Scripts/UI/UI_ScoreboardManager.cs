using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using static UI_Scoreboard;
using TMPro;

public class UI_ScoreboardManager : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> _subMenuList;
    [SerializeField] private UI_Scoreboard EasyScoreboard;
    [SerializeField] private UI_Scoreboard NormalScoreboard;
    [SerializeField] private UI_Scoreboard HardScoreboard;
    public TMP_InputField inputField;

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

    public void AddScoreButton()
    {
        AddScoreToScoreboard(inputField.text, GPCtrl.Instance.Timer);
    }

    public void AddScoreToScoreboard(string _name, float _timer)
    {
        ScoreboardEntry entry = new(_name, _timer);
        switch (DataHolder.Instance.CurrentDifficulty)
        {
            case DataHolder.DifficultyMode.Easy:
                EasyScoreboard.ScoreList.entries.Add(entry);
                EasyScoreboard.SaveScoreboard();
                break;
            case DataHolder.DifficultyMode.Normal:
                NormalScoreboard.ScoreList.entries.Add(entry);
                NormalScoreboard.SaveScoreboard();
                break;
            case DataHolder.DifficultyMode.Hard:
                Debug.Log("save in hard");
                HardScoreboard.ScoreList.entries.Add(entry);
                HardScoreboard.SaveScoreboard();
                break;
        }
        SelectPage(_subMenuList[(int)DataHolder.Instance.CurrentDifficulty]);
    }
}