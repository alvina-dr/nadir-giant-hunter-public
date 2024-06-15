using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ScoreEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textName;
    [SerializeField] private TextMeshProUGUI _textTimer;
    [SerializeField] private TextMeshProUGUI _textRank;

    public void SetScoreEntry(UI_Scoreboard.ScoreboardEntry scoreEntry, int rank)
    {
        _textName.text = scoreEntry.name.ToString();
        _textTimer.text = DataHolder.Instance.ConvertTimeToMinutes(scoreEntry.timer);
        _textRank.text = (rank+1).ToString();
    }
}
