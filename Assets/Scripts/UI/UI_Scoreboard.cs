using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using DG.Tweening;

public class UI_Scoreboard : MonoBehaviour
{
    [System.Serializable]
    public class ScoreboardEntry
    {
        public string name;
        public float timer;

        public ScoreboardEntry(string _name, float _timer)
        {
            name = _name;
            timer = _timer;
        }
    }

    public class ScoreboardList
    {
        public List<ScoreboardEntry> entries = new List<ScoreboardEntry>();
    }

    public UI_Menu Menu;
    private ScoreboardList scoreList = new ScoreboardList();
    public TMP_InputField inputField;
    public UI_ScoreEntry scoreEntryPrefab;
    public Transform scoreEntryLayout;

    private void Start()
    {
        if (scoreList.entries.Count == 0)
        {
            if (PlayerPrefs.HasKey("scoreboard"))
            {
                string json = PlayerPrefs.GetString("scoreboard"); // use scoreboard-levelname
                scoreList = JsonUtility.FromJson<ScoreboardList>(json);
            }
        }
    }

    public void AddScoreButton()
    {
        AddScoreToScoreboard(inputField.text, GPCtrl.Instance.Timer);
        ShowScoreboard();
    }

    public void AddScoreToScoreboard(string _name, float _timer)
    {
        ScoreboardEntry entry = new(_name, _timer);
        scoreList.entries.Add(entry);
        SaveScoreboard();
    }

    public void ShowScoreboard()
    {
        for (int i = 0; i < scoreList.entries.Count; i++)
        {
            InstantiateScoreboardEntry(scoreList.entries[i], i);
        }
        Menu.OpenMenu();
    }

    public void HideScoreboard()
    {
        Menu.CloseMenu();
        DOVirtual.DelayedCall(.3f, () =>
        {
            for (int i = 0; i < scoreEntryLayout.childCount; i++)
            {
                Destroy(scoreEntryLayout.GetChild(i).gameObject);
            }
        });
    }

    public void SaveScoreboard()
    {
        scoreList.entries.Sort(SortByScore);
        if (scoreList.entries.Count > GPCtrl.Instance.GeneralData.scoreboardSize)
            scoreList.entries = scoreList.entries.Take(GPCtrl.Instance.GeneralData.scoreboardSize).ToList();
        string json = JsonUtility.ToJson(scoreList);
        PlayerPrefs.SetString("scoreboard", json);
        PlayerPrefs.Save();
    }

    public void InstantiateScoreboardEntry(ScoreboardEntry scoreboardEntry, int rank)
    {
        UI_ScoreEntry scoreEntry = Instantiate(scoreEntryPrefab, scoreEntryLayout);
        scoreEntry.SetScoreEntry(scoreboardEntry, rank);
    }

    static int SortByScore(ScoreboardEntry p1, ScoreboardEntry p2)
    {
        return -p1.timer.CompareTo(p2.timer);
    }
}