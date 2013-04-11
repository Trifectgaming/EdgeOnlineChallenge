using System.Collections;
using UnityEngine;
using System.Xml.Serialization;

public class ScoreManager : GameSceneObject
{
    public tk2dTextMesh scoreText;
    public int BlockedAdj;
    public int MissAdj;
    public int MotherHitAdj;
    public int totalScore;
    public int blocked;
    public int misses;
    public int motherHits;
    public float scoreUpdateDelay;

    protected override void Awake()
    {
        base.Awake();
        Messenger.Default.Register<MotherImpactMessage>(this, OnMotherImpact);
        Messenger.Default.Register<ShieldImpactMessage>(this, OnShieldImpact);
        Messenger.Default.Register<LevelStartMessage>(this, OnLevelStartMessage);
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
            Calculate();
            SetScore();
            yield return new WaitForSeconds(scoreUpdateDelay);
        }
    }

    public ScoreInfo Calculate()
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
        var result = new ScoreInfo
                   {
                       HitsBlocked = blocked,
                       HitsBlockedPts = blockedPts,
                       HitsMissed = misses,
                       HitsMissedPts = missPts,
                       MotherHits = motherHits,
                       MotherHitsPts = motherHitPts,
                       Total = total
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
    public int Total;
}
