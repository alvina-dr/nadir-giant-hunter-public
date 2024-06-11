using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using DG.Tweening;
using System;

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

    [System.Serializable]
    public class ScoreboardList
    {
        public List<ScoreboardEntry> entries = new List<ScoreboardEntry>();
    }

    public ScoreboardList ScoreList = new ScoreboardList();
    public UI_ScoreEntry scoreEntryPrefab;
    public Transform scoreEntryLayout;
    public DataHolder.DifficultyMode Difficulty;

    private void Awake()
    {
        if (ScoreList.entries.Count == 0)
        {
            if (PlayerPrefs.HasKey("scoreboard" + Difficulty))
            {
                Debug.Log("log in difficulty : " + Difficulty.ToString());
                string json = PlayerPrefs.GetString("scoreboard" + Difficulty); // use scoreboard-levelname
                ScoreList = JsonUtility.FromJson<ScoreboardList>(json);
            }
        }
        CreateScoreboard();
    }

    public void CreateScoreboard()
    {
        for (int i = 0; i < ScoreList.entries.Count; i++)
        {
            InstantiateScoreboardEntry(ScoreList.entries[i], i);
            Debug.Log("add scoreboard entry");
        }
    }

    public void DestroyScoreboard()
    {
        for (int i = 0; i < scoreEntryLayout.childCount; i++)
        {
            Destroy(scoreEntryLayout.GetChild(i).gameObject);
            Debug.Log("destroy scoreboard entry");
        }
    }

    public void SaveScoreboard()
    {
        ScoreList.entries.Sort(SortByScore);
        if (ScoreList.entries.Count > GPCtrl.Instance.GeneralData.scoreboardSize)
            ScoreList.entries = ScoreList.entries.Take(GPCtrl.Instance.GeneralData.scoreboardSize).ToList();
        string json = JsonUtility.ToJson(ScoreList);
        PlayerPrefs.SetString("scoreboard" + Difficulty, json);
        Debug.Log("json : " + json);
        PlayerPrefs.Save();
        DestroyScoreboard();
        CreateScoreboard();
    }

    public void InstantiateScoreboardEntry(ScoreboardEntry scoreboardEntry, int rank)
    {
        UI_ScoreEntry scoreEntry = Instantiate(scoreEntryPrefab, scoreEntryLayout);
        scoreEntry.SetScoreEntry(scoreboardEntry, rank);
    }

    static int SortByScore(ScoreboardEntry p1, ScoreboardEntry p2)
    {
        return p1.timer.CompareTo(p2.timer);
    }
}