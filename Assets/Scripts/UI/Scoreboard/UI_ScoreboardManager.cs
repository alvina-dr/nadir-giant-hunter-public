using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using static UI_Scoreboard;
using TMPro;
using System;
using Sirenix.OdinInspector;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

public class UI_ScoreboardManager : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> _subMenuList;
    [SerializeField] private UI_Scoreboard EasyScoreboard;
    [SerializeField] private UI_Scoreboard NormalScoreboard;
    [SerializeField] private UI_Scoreboard HardScoreboard;
    public TMP_InputField inputField;
    [SerializeField] private List<UI_Button> _buttonList;
    
    public event Action OnScoreBeginSendToOnlineLeaderboard; // useful for UI and loading bar (the operation can take some time depending on the internet connection)
    public event Action<bool> OnScoreSentToOnlineLeaderboard; // bool = if the operation was successful or not
    public event Action OnBeginOnlineLeaderboardRequest; // useful for UI and loading bar (the operation can take some time depending on the internet connection)
    public event Action<bool> OnEndOnlineLeaderboardRequest; // bool = if the operation was successful or not

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
        EasyScoreboard.ScoreList.entries.Clear();
        NormalScoreboard.ScoreList.entries.Clear();
        HardScoreboard.ScoreList.entries.Clear();
        EasyScoreboard.DestroyScoreboard();
        NormalScoreboard.DestroyScoreboard();
        HardScoreboard.DestroyScoreboard();
        for (int i = 0; i < _subMenuList.Count; i++)
        {
            HideSubMenu(_subMenuList[i]);
        }
        ResetNavbar();
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
                EasyScoreboard.ScoreList.entries.Clear();
                if (PlayerPrefs.HasKey("scoreboard" + DataHolder.Instance.CurrentDifficulty))
                {
                    string jsonEasy = PlayerPrefs.GetString("scoreboard" + DataHolder.Instance.CurrentDifficulty); // use scoreboard-levelname
                    EasyScoreboard.ScoreList = JsonUtility.FromJson<ScoreboardList>(jsonEasy);
                }
                EasyScoreboard.ScoreList.entries.Add(entry);
                EasyScoreboard.SaveLocalScoreboard();
                break;
            case DataHolder.DifficultyMode.Normal:
                NormalScoreboard.ScoreList.entries.Clear();
                if (PlayerPrefs.HasKey("scoreboard" + DataHolder.Instance.CurrentDifficulty)) {
                    string jsonNormal = PlayerPrefs.GetString("scoreboard" + DataHolder.Instance.CurrentDifficulty); // use scoreboard-levelname
                    NormalScoreboard.ScoreList = JsonUtility.FromJson<ScoreboardList>(jsonNormal);
                }

                NormalScoreboard.ScoreList.entries.Add(entry);
                NormalScoreboard.SaveLocalScoreboard();
                break;
            case DataHolder.DifficultyMode.Hard:
                HardScoreboard.ScoreList.entries.Clear();
                if (PlayerPrefs.HasKey("scoreboard" + DataHolder.Instance.CurrentDifficulty)) {
                    string jsonHard = PlayerPrefs.GetString("scoreboard" + DataHolder.Instance.CurrentDifficulty); // use scoreboard-levelname
                    HardScoreboard.ScoreList = JsonUtility.FromJson<ScoreboardList>(jsonHard);
                }
                HardScoreboard.ScoreList.entries.Add(entry);
                HardScoreboard.SaveLocalScoreboard();
                break;
        }
        StartCoroutine(HandleOnlineLeaderboard(_name, _timer, DataHolder.Instance.CurrentDifficulty));
        SelectPage(_subMenuList[(int)DataHolder.Instance.CurrentDifficulty]);
    }

    public void ResetNavbar()
    {
        for (int i = 0; i < _buttonList.Count; i++)
        {
            _buttonList[i].Deactivate();
        }
    }

    public void GetOnlineScoreboard()
    {
        StartCoroutine(GetOnlineScoreboardCoroutine());
    }

    public IEnumerator GetOnlineScoreboardCoroutine()
    {
        EasyScoreboard.ScoreList.entries.Clear();
        NormalScoreboard.ScoreList.entries.Clear();
        HardScoreboard.ScoreList.entries.Clear();
        yield return GetOnlineLeaderboardData();
        EasyScoreboard.DestroyScoreboard();
        NormalScoreboard.DestroyScoreboard();
        HardScoreboard.DestroyScoreboard();
        EasyScoreboard.CreateScoreboard();
        NormalScoreboard.CreateScoreboard();
        HardScoreboard.CreateScoreboard();
    }

    public void GetLocalScoreboard()
    {
        EasyScoreboard.GetLocalScoreboard();
        NormalScoreboard.GetLocalScoreboard();
        HardScoreboard.GetLocalScoreboard();
    }

    private IEnumerator HandleOnlineLeaderboard(string playerName, float score, DataHolder.DifficultyMode difficultyMode)
    {
        EasyScoreboard.ScoreList.entries.Clear();
        NormalScoreboard.ScoreList.entries.Clear();
        HardScoreboard.ScoreList.entries.Clear();
        yield return SendDataToOnlineLeaderboard(playerName, score, difficultyMode);
        yield return GetOnlineLeaderboardData();
        // Since we have new scores after the online request, we must regenerate the scoreboards (or not, change however you want here)
        EasyScoreboard.DestroyScoreboard();
        NormalScoreboard.DestroyScoreboard();
        HardScoreboard.DestroyScoreboard();
        EasyScoreboard.CreateScoreboard();
        NormalScoreboard.CreateScoreboard();
        HardScoreboard.CreateScoreboard();
    }

    private async Awaitable SendDataToOnlineLeaderboard(string playerName, float score, DataHolder.DifficultyMode difficultyMode)
    {
        OnScoreBeginSendToOnlineLeaderboard?.Invoke();
        
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("No internet connection detected. Unity Gaming Services will not be initialized (no online Leaderboard)",
                this);
            OnScoreSentToOnlineLeaderboard?.Invoke(false);
            return;
        }
        
        // The try catch is necessary since each request can throw an exception and not work at all
        try
        {
            // unsign user if already signed in
            if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut(true);
            }
            var options = new InitializationOptions();
            
            // set a random alphanumeric profile (this is because of how the leaderboard works, 1 user has 1 score, so we create a random user for each score)
            var randomString = Guid.NewGuid().ToString("N")[..8];
            options.SetProfile(randomString);
        
            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            var playerScoreOptions = new AddPlayerScoreOptions
            {
                // in the metadata of the Unity dashboard, every key seems to be ordered alphabetically
                // Also the max size of Metadata is 1024 bytes (1KB)
                Metadata = new Dictionary<string, string> {{"Difficulty", difficultyMode.ToString()}, {"PlayerName", playerName}}
            };
            await LeaderboardsService.Instance.AddPlayerScoreAsync("Leaderboard", score, playerScoreOptions);
            Debug.Log("Successfully added score to online leaderboard !");
            OnScoreSentToOnlineLeaderboard?.Invoke(true);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to add score to online leaderboard, see exception : {e.Message}");
            OnScoreSentToOnlineLeaderboard?.Invoke(false);
        }
    }
    
    private async Awaitable GetOnlineLeaderboardData()
    {
        OnBeginOnlineLeaderboardRequest?.Invoke();
        
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("No internet connection detected. Unity Gaming Services will not be initialized (no online Leaderboard)",
                this);
            OnEndOnlineLeaderboardRequest?.Invoke(false);
            return;
        }

        // The try catch is necessary since each request can throw an exception and not work at all
        try
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            var options = new GetScoresOptions
            {
                IncludeMetadata = true
            };

            LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync("Leaderboard", options);
            leaderboardScoresPage.Results.ForEach(entry =>
            {
                // reconstruct the difficulty and player name from the metadata which is a json string
                var metadata = JsonUtility.FromJson<LeaderboardMetadata>(entry.Metadata);
                if (metadata == null) return;
                
                var scoreboardEntry = new ScoreboardEntry(metadata.PlayerName, (float)entry.Score);
                //Debug.Log($"(Online Leaderboard) Player {metadata.PlayerName} has a score of {entry.Score} in difficulty {metadata.Difficulty}");
                
                // HANDLE THE SCORES HERE HOW YOU WANT
                // For now the scores are added to the scoreboards and the scoreboard are destroyed/recreated again (not sure if it's the best way to do it)
                // Things that need to be handled :
                // - duplicate scores (between already stored local score and online score)
                // - store online scores in playerprefs ?
                switch (metadata.Difficulty)
                {
                    case "Easy":
                        EasyScoreboard.ScoreList.entries.Add(scoreboardEntry);
                        break;
                    case "Normal":
                        NormalScoreboard.ScoreList.entries.Add(scoreboardEntry);
                        break;
                    case "Hard":
                        HardScoreboard.ScoreList.entries.Add(scoreboardEntry);
                        break;
                }
            });
            OnEndOnlineLeaderboardRequest?.Invoke(true);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get online leaderboard data, see exception : {e.Message}");
            OnEndOnlineLeaderboardRequest?.Invoke(false);
            EasyScoreboard.ScoreList.entries.Clear();
            NormalScoreboard.ScoreList.entries.Clear();
            HardScoreboard.ScoreList.entries.Clear();
            EasyScoreboard.DestroyScoreboard();
            NormalScoreboard.DestroyScoreboard();
            HardScoreboard.DestroyScoreboard();
        }
    }

    [Serializable]
    private class LeaderboardMetadata
    {
        public string Difficulty;
        public string PlayerName;
    }
}