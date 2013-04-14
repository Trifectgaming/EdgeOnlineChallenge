using System.Collections;
using UnityEngine;
using System.Xml.Serialization;

public class ScoreManager : GameSceneObject
{
    public tk2dTextMesh scoreText;
    public int BlockedAdj;
    public int MissAdj;
    public int MotherHitAdj;
    public long totalScore;
    public int blocked;
    public int misses;
    public int motherHits;
    public float scoreUpdateDelay;
    private bool _gameOver;

    protected override void Awake()
    {
        base.Awake();
        Messenger.Default.Register<MotherImpactMessage>(this, OnMotherImpact);
        Messenger.Default.Register<ShieldImpactMessage>(this, OnShieldImpact);
        Messenger.Default.Register<LevelStartMessage>(this, OnLevelStartMessage);
        Messenger.Default.Register<GameWonMessage>(this, OnGameWon);
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
    }

    private void OnGameOver(GameOverMessage obj)
    {
        _gameOver = true;
    }

    private void OnGameWon(GameWonMessage obj)
    {
        _gameOver = true;        
    }

    private void OnMotherImpact(MotherImpactMessage obj)
    {
        motherHits++;
    }

    private void OnShieldImpact(ShieldImpactMessage obj)
    {
        if (obj.WasDeflected)
        {
            blocked++;
        }
        else
        {
            misses++;
        }
    }

    private void OnLevelStartMessage(LevelStartMessage obj)
    {
        _gameOver = false;
        SetScore();
    }

    private void SetScore()
    {
        scoreText.text = "Score: " + totalScore;
        scoreText.Commit();
    }

    protected override void Start ()
	{
        blocked = 0;
        misses = 0;
	    motherHits = 0;
        base.Start();
        if (GameManager.IsEndless)
            StartCoroutine(EndlessScoreUpdate());
	}

    private IEnumerator EndlessScoreUpdate()
    {
        while (true)
        {
            if (_gameOver) break;
            Calculate(false);
            SetScore();
            yield return new WaitForSeconds(scoreUpdateDelay);
            if (_gameOver) break;
        }
    }

    public ScoreInfo Calculate(bool gameOver)
    {
        var blockedPts = blocked*BlockedAdj;
        var missPts = misses*MissAdj;
        var motherHitPts = motherHits*MotherHitAdj;
        var total = blockedPts + missPts + motherHitPts;
        if (GameManager.IsEndless)
        {
            totalScore = total;
        }
        else
        {
            totalScore += total;
        }
        int position;
        if (LeaderBoardManager.CheckHighScore(totalScore, out position) && gameOver)
            LeaderBoardManager.SetHighScore(GameManager.PlayerName, totalScore);
        var result = new ScoreInfo
                   {
                       HitsBlocked = blocked,
                       HitsBlockedPts = blockedPts,
                       HitsMissed = misses,
                       HitsMissedPts = missPts,
                       MotherHits = motherHits,
                       MotherHitsPts = motherHitPts,
                       LevelTotal = total,
                       TotalScore = totalScore,
                       position = position,
                   };
        if (!GameManager.IsEndless)
        {
            blocked = 0;
            misses = 0;
            motherHits = 0;
        }
        return result;
    }
}

public struct ScoreInfo
{
    public int HitsBlocked;
    public int HitsBlockedPts;
    public int HitsMissed;
    public int HitsMissedPts;
    public int MotherHits;
    public int MotherHitsPts;
    public int LevelTotal;
    public int position;
    public long TotalScore;
}
