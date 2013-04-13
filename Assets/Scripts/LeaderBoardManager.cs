using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using System.Collections;

public class LeaderBoardManager : MonoBehaviour
{
    private static readonly Dictionary<GameMode, List<PlayerScore>> Scores = new Dictionary<GameMode, List<PlayerScore>>();
    private const int LeadersCount = 5;
    public const string Leaderkey = "LeaderBoard";

    public GameMode Mode;
    public bool ScoreReset;

    private string _leaderKey = Leaderkey;
    private UILabel[] _nameLabels;
    private UILabel[] _scoreLabels;
    
    void Start ()
    {
        _leaderKey = Mode + _leaderKey;
        Debug.Log("LeaderBoard Started " + _leaderKey);
	    
        _nameLabels = gameObject
	        .GetComponentsInChildren<UILabel>()
	        .Where(t => t.tag == "NameLabel")
	        .OrderBy(t => t.name)
	        .ToArray();

	    _scoreLabels = gameObject
	        .GetComponentsInChildren<UILabel>()
	        .Where(t => t.tag == "ScoreLabel")
	        .OrderBy(t => t.name)
	        .ToArray();

        if (ScoreReset)
        {
            PlayerPrefs.DeleteKey(_leaderKey);
            PlayerPrefs.Save();
            Debug.Log("Default score deleted for " + _leaderKey);
        }

        if (!PlayerPrefs.HasKey(_leaderKey))
	    {
            PlayerPrefs.SetString(_leaderKey, DefaultScores.GetDefaultFor(Mode));
            Debug.Log("Default score created for " + _leaderKey);
	    }
        string scoreString = PlayerPrefs.GetString(_leaderKey);
        Debug.Log("Score string is " + scoreString);
        Scores[Mode] = scoreString.Split(',')
                                  .Select(s =>
                                              {
                                                  var ps = s.Split('|');
                                                  return new PlayerScore(ps[0], ps[1]);
                                              })
                                  .OrderByDescending(p => p.Score)
                                  .ToList();

        for (int i = 0; i < Scores[Mode].Count; i++)
	    {
            _nameLabels[i].text = Scores[Mode][i].Name;
            _scoreLabels[i].text = Scores[Mode][i].Score.ToString(CultureInfo.InvariantCulture);
	    }
	}

    // Update is called once per frame
	void Update () {
	
	}

    public static bool CheckHighScore(long score, out int position)
    {
        position = -1;
        foreach (var playerScore in Scores[GameManager.GameMode])
        {
            position++;
            if (score > playerScore.Score)
                break;
        }
        return position > -1;
    }

    public static void SetHighScore(string playerName, long score)
    {
        int position;
        if (CheckHighScore(score, out position))
        {
            Scores[GameManager.GameMode].Insert(position, new PlayerScore(playerName, score));
            Scores[GameManager.GameMode] = Scores[GameManager.GameMode].Take(LeadersCount).ToList();
            SaveScores();
        }
    }

    public static void SaveScores()
    {
        string scoreString = Scores[GameManager.GameMode].StringJoin(",", LeadersCount);
        PlayerPrefs.SetString(GameManager.GameMode + Leaderkey, scoreString);
        PlayerPrefs.Save();
    }
}

public static class StringExtensions
{
    public static string StringJoin<T>(this IEnumerable<T> source, string seperator, int count)
    {
        var copy = source.Take(count);
        return string.Join(seperator, copy.Select(s => s.ToString()).ToArray());
    }
}

public class PlayerScore
{
    public PlayerScore(string name, string score)
    {
        Name = name;
        long s;
        long.TryParse(score, out s);
        Score = s;
    }

    public string Name;
    public long Score;

    public PlayerScore(string name, long score)
    {
        Name = name;
        Score = score;
    }

    public override string ToString()
    {
        return Name + "|" + Score;
    }
}

public enum GameMode
{
    Story,
    Endless
}

public static class DefaultScores
{
    public static string GetDefaultFor(GameMode mode)
    {
        if (mode == GameMode.Story)
            return "JRH|" + Random.Range(1000,10000) +
                   ",RAP|" + Random.Range(1000, 10000) +
                   ",RG|" + Random.Range(1000, 10000) +
                   ",AG|" + Random.Range(1000, 10000) +
                   ",BLH|" + Random.Range(1000, 10000);
        if (mode == GameMode.Endless)
            return "JRH|" + Random.Range(1000, 100000) +
                   ",RAP|" + Random.Range(1000, 100000) +
                   ",RG|" + Random.Range(1000, 100000) +
                   ",AG|" + Random.Range(1000, 100000) +
                   ",BLH|" + Random.Range(1000, 100000);
        return string.Empty;
    }
}