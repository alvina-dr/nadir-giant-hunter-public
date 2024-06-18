using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_OnlineChoice : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _onlineTextList;
    private int _index;
    [SerializeField] private UI_ScoreboardManager _scoreboardManager;

    private void Start()
    {
        _onlineTextList[1].gameObject.SetActive(false);
    }

    public void ButtonLeft()
    {
        _index--;
        if (_index < 0) _index = _onlineTextList.Count - 1;
        UpdateScoreboard();
    }


    public void ButtonRight()
    {
        _index++;
        if (_index >= _onlineTextList.Count) _index = 0;
        UpdateScoreboard();
    }

    public void UpdateScoreboard()
    {
        for (int i = 0; i < _onlineTextList.Count; i++)
        {
            _onlineTextList[i].gameObject.SetActive(false);
        }
        _onlineTextList[_index].gameObject.SetActive(true);
        _onlineTextList[_index].fontSize = 15;
        DOVirtual.DelayedCall(.1f, () =>
        {
            _onlineTextList[_index].fontSize = 30;
        });
        if (_index == 0)
        {
            _scoreboardManager.GetOnlineScoreboard();

        } else
        {
            _scoreboardManager.GetLocalScoreboard();
        }
    }

    private void Update()
    {
        //_onlineTextList[1].fontSize = 30;
    }

    public void ResetChoice()
    {
        _index = 0;
        UpdateScoreboard();
    }
}