using System.Collections;
using UnityEngine;

public abstract class ScoreMenuBase : MonoBehaviour
{
    public UIButtonHandler ContinueButton;
    public UILabel HitsBlockedText;
    public UILabel HitsMissedText;
    public UILabel MotherHitsText;
    public UILabel TotalText;
    protected bool IsScoring;
    protected float UpdateDelay;
    public float updateDelay = .5f;

    protected virtual void Start()
    {
        ContinueButton.Click += OnContinueButton;
    }

    protected void OnContinueButton(object sender, ClickEventArgs e)
    {
        ContinueButton.IsEnabled = false;
        if (IsScoring)
        {
            UpdateDelay = 0;
            Invoke("Continue", 1);
        }
        else
        {
            Continue();
        }
    }

    protected abstract void Continue();

    protected virtual void Update() { }

    public void Show(ScoreInfo levelScore)
    {
        ContinueButton.IsEnabled = true;
        Screen.lockCursor = false;
        HitsBlockedText.text = string.Empty;
        HitsMissedText.text = string.Empty;
        MotherHitsText.text = string.Empty;
        TotalText.text = string.Empty;
        UpdateDelay = updateDelay;
        gameObject.SetActive(true);
        StartCoroutine(UpdateScore(levelScore));
    }

    public IEnumerator UpdateScore(ScoreInfo levelScore)
    {
        IsScoring = true;
        HitsBlockedText.text = levelScore.HitsBlockedPts + " (" + levelScore.HitsBlocked + ")";
        yield return new WaitForSeconds(UpdateDelay);
        HitsMissedText.text = levelScore.HitsMissedPts + " (" + levelScore.HitsMissed + ")";
        yield return new WaitForSeconds(UpdateDelay);
        MotherHitsText.text = levelScore.MotherHitsPts + " (" + levelScore.MotherHits + ")";
        yield return new WaitForSeconds(UpdateDelay);
        TotalText.text = levelScore.Total.ToString();
        IsScoring = false;
    }
}

public class ScoreMenu : ScoreMenuBase
{
    protected override void Continue()
    {
        Screen.lockCursor = true;
        Messenger.Default.Send(new LevelStartMessage());
        gameObject.SetActive(false);
    }
}