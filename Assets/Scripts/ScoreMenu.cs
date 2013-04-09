using UnityEngine;
using System.Collections;

public class ScoreMenu : MonoBehaviour
{
    public UILabel HitsBlockedText;
    public UILabel HitsMissedText;
    public UILabel MotherHitsText;
    public UILabel TotalText;
    public float updateDelay = .5f;
    private float _updateDelay;
    public UIButtonHandler ContinueButton;
    private bool _isScoring;

    // Use this for initialization
	void Start ()
	{
	    ContinueButton.Click += OnContinueButton;
	}

    private void OnContinueButton(object sender, ClickEventArgs e)
    {
        print("ContinueButton Clicked");
        Screen.lockCursor = true;
        ContinueButton.IsEnabled = false;
        if (_isScoring)
        {
            _updateDelay = 0;
            Invoke("LevelStart", 2);
        }
        else
        {
            LevelStart();
        }
    }

    private void LevelStart()
    {
        Messenger.Default.Send(new LevelStartMessage());
        gameObject.SetActive(false);
    }

    // Update is called once per frame
	void Update () {
	
	}

    public void Show(ScoreInfo levelScore)
    {
        Screen.lockCursor = false;
        HitsBlockedText.text = string.Empty;
        HitsMissedText.text = string.Empty;
        MotherHitsText.text = string.Empty;
        TotalText.text = string.Empty;
        _updateDelay = updateDelay;
        gameObject.SetActive(true);
        StartCoroutine(UpdateScore(levelScore));
    }

    public IEnumerator UpdateScore(ScoreInfo levelScore)
    {
        
        _isScoring = true;
        HitsBlockedText.text = levelScore.HitsBlockedPts + " (" + levelScore.HitsBlocked +")";
        yield return new WaitForSeconds(_updateDelay);
        HitsMissedText.text = levelScore.HitsMissedPts + " (" + levelScore.HitsMissed + ")";
        yield return new WaitForSeconds(_updateDelay);
        MotherHitsText.text = levelScore.MotherHitsPts + " (" + levelScore.MotherHits + ")";
        yield return new WaitForSeconds(_updateDelay);
        TotalText.text = levelScore.Total.ToString();
        _isScoring = false;
    }
}
