using UnityEngine;
using System.Collections;

public class ScoreManager : GameSceneObject
{
    public tk2dTextMesh motherScore;
    public tk2dTextMesh hitRating;
    public int deflections;
    public int misses;
    public int motherHits;

    void Awake()
    {
        Messenger.Default.Register<MotherImpactMessage>(this, OnMotherImpact);
        Messenger.Default.Register<ShieldImpactMessage>(this, OnShieldImpact);
    }

    private void OnShieldImpact(ShieldImpactMessage obj)
    {
        if (obj.WasDeflected) deflections++;
        else misses++;
        hitRating.text = "Match/Miss: " + deflections + "/" + misses;
        hitRating.Commit();
    }

    private void OnMotherImpact(MotherImpactMessage obj)
    {
        motherHits++;
        motherScore.text = "Mother Hits: " + motherHits;
        motherScore.Commit();
    }

    protected override void Start ()
	{
        deflections = 0;
        misses = 0;
	    motherHits = 0;
        base.Start();
	}
	
	void Update () {
	
	}
}
