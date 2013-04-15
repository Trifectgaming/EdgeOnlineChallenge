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
        Messenger.Default.Register<LevelRetryMessage>(this, OnRetry);
    }

    private void OnRetry(LevelRetryMessage obj)
    {
        if (GameManager.IsEndless)
        {
            Start();
        }
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
        SetScore(totalScore);
    }

    private void SetScore(long score)
    {
        scoreText.text = "Score: " + score;
        scoreText.Commit();
    }

    protected override void Start ()
	{
        blocked = 0;
        misses = 0;
	    motherHits = 0;
        totalScore = 0;
        base.Start();
        if (GameManager.IsEndless)
            StartCoroutine(EndlessScoreUpdate());
	}

    private IEnumerator EndlessScoreUpdate()
    {
        while (true)
        {
            if (_gameOver) break;
            var info  = Calculate(false);
            SetScore(info.TotalScore);
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
        var totalToReport = totalScore;
        if (GameManager.IsEndless)
        {
            if (!gameOver)
            {
                totalScore = total;
            }
            totalToReport = totalScore;
        }
        else
        {
            if (!gameOver)
            {
                totalScore += total;                
            }
            totalToReport += total;
        }
        int position;
        if (LeaderBoardManager.CheckHighScore(totalToReport, out position) && gameOver)
            LeaderBoardManager.SetHighScore(GameManager.PlayerName, totalToReport);
        var result = new ScoreInfo
                   {
                       HitsBlocked = blocked,
                       HitsBlockedPts = blockedPts,
                       HitsMissed = misses,
                       HitsMissedPts = missPts,
                       MotherHits = motherHits,
                       MotherHitsPts = motherHitPts,
                       LevelTotal = total,
                       TotalScore = totalToReport,
                       position = position,
                   };
        if (!GameManager.IsEndless || gameOver)
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
