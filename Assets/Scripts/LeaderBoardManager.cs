using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using System.Collections;

public class LeaderBoardManager : MonoBehaviour
{
    public const string Leaderkey = "LeaderBoard";

    public GameMode Mode;
    public bool ScoreReset;
    public UITable NumberColumn;
    public UITable NameColumn;
    public UITable ScoreColumn;
    
    public UILabel Template;

    private string _leaderKey = Leaderkey;
    private static readonly Dictionary<GameMode, List<PlayerScore>> Scores = new Dictionary<GameMode, List<PlayerScore>>();
    private const int LeadersCount = 14;

    void Start ()
    {
        _leaderKey = Mode + _leaderKey;
        Debug.Log("LeaderBoard Started " + _leaderKey);
	    
        
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
        var i = 0;
        for (; i < Scores[Mode].Count; i++)
        {
            var nameLabel = (UILabel) Instantiate(Template);
            nameLabel.text = Scores[Mode][i].Name.PadRight(16,'*');
            nameLabel.name = "Name" + i;
            nameLabel.transform.parent = NameColumn.transform;

            var numberLabel = (UILabel) Instantiate(Template);
            numberLabel.text = (i + 1).ToString(CultureInfo.InvariantCulture);
            numberLabel.name = "Number" + i;
            numberLabel.transform.parent = NumberColumn.transform;
            
            var scoreLabel = (UILabel)Instantiate(Template);
            scoreLabel.text = Scores[Mode][i].Score.ToString(CultureInfo.InvariantCulture);
            scoreLabel.name = "Score" + i;
            scoreLabel.transform.parent = ScoreColumn.transform;
        }
        NameColumn.Reposition();
        NumberColumn.Reposition();
        ScoreColumn.Reposition();
	}

    // Update is called once per frame
	void Update () {
	
	}

    public static bool CheckHighScore(long score, out int position)
    {
        Debug.Log("Checking score of " + score);
        position = -1;
        List<PlayerScore> scores;
        if (!Scores.TryGetValue(GameManager.GameMode, out scores))
        {
            Debug.Log("Did not find GameMode " + GameManager.GameMode);            
            return false;
        }
        Debug.Log("Found " + scores.Count + " for GameMode " + GameManager.GameMode);
        for (int index = 0; index < scores.Count; index++)
        {
            var playerScore = scores[index];
            if (score > playerScore.Score)
            {
                position = index;
                break;
            }
            else
            {
                Debug.Log("Score " + score +" < " + playerScore);    
            }
        }
        return position > -1;
    }

    public static void SetHighScore(string playerName, long score)
    {
        int position;
        if (CheckHighScore(score, out position))
        {
            Debug.Log("Setting score of " + score + " in position " + position);
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
            return "JRH|" + Random.Range(1,10) * 1000 +
                   ",RAP|" + Random.Range(1, 10) * 1000 +
                   ",RG|" + Random.Range(1, 10) * 1000 +
                   ",AG|" + Random.Range(1, 10) * 1000 +
                   ",BLH|" + Random.Range(1, 10) * 1000;
        if (mode == GameMode.Endless)
            return "JRH|" + Random.Range(1, 10) * 1000 +
                   ",RAP|" + Random.Range(1, 10) * 1000 +
                   ",RG|" + Random.Range(1, 10) * 1000 +
                   ",AG|" + Random.Range(1, 10) * 1000 +
                   ",BLH|" + Random.Range(1, 10) * 1000;
        return string.Empty;
    }
}