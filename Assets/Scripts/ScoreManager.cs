using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{

    public tk2dTextMesh motherScore;
    public tk2dTextMesh hitRating;
    public int deflections;
    public int misses;
    public int motherHits;

    void Awake()
    {
        
    }

    private void OnShieldImpact(ShieldImpactMessage obj)
    {
        Debug.Log("Shield Impact");
        if (obj.WasDeflected) deflections++;
        else misses++;
        hitRating.text = "Success/Failure: " + deflections + "/" + misses;
        hitRating.Commit();
    }

    private void OnMotherImpact(MotherImpactMessage obj)
    {
        Debug.Log("Mother Impact");
        motherHits++;
        motherScore.text = "Mother Hits: " + motherHits;
        motherScore.Commit();
    }

    // Use this for initialization
	void Start ()
	{
        Messenger.Default.Register<MotherImpactMessage>(this, OnMotherImpact);
        Messenger.Default.Register<ShieldImpactMessage>(this, OnShieldImpact);
	    deflections = 0;
        misses = 0;
	    motherHits = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
