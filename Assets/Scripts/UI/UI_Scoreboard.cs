using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
    //public TextMeshProUGUI scoreText;
    public UI_ScoreEntry scoreEntryPrefab;
    public Transform scoreEntryLayout;

    private void Start()
    {
        //scoreText.transform.localScale = Vector3.zero;
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
        Debug.Log(entry.name + " , " + entry.timer.ToString());
        SaveScoreboard();
    }

    //public void ShowTypeNameMenu()
    //{
    //    typeNameMenu.gameObject.SetActive(true);
    //    typeNameMenu.DOFade(1, .3f).OnComplete(() =>
    //    {
    //        scoreText.text = GPCtrl.Instance.currentScore.ToString();
    //        scoreText.transform.DOScale(1.1f, .3f).OnComplete(() =>
    //        {
    //            scoreText.transform.DOScale(1f, .1f);
    //            EventSystem.current.SetSelectedGameObject(keyboardFirstButton);
    //        });
    //    });
    //}

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
        Debug.Log("SAVE SCOREBOARD");
        Debug.Log(json);
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
