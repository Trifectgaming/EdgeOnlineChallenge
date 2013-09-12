using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

    protected List<Action<ScoreInfo>> AdditionalActions;

    protected virtual void Awake()
    {
        AdditionalActions = new List<Action<ScoreInfo>>();
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
        Showing();
        StartCoroutine(UpdateScore(levelScore));
    }

    protected virtual void Showing()
    {
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
        TotalText.text = levelScore.LevelTotal.ToString(CultureInfo.InvariantCulture);
        foreach (var additionalActions in AdditionalActions)
        {
            yield return new WaitForSeconds(UpdateDelay);
            additionalActions(levelScore);
        }
        IsScoring = false;
    }
}

public class ScoreMenu : ScoreMenuBase
{
    protected override void Continue()
    {
        AdManager.TryShowAd(AdManager.BetweenLevels,
                            () =>
                                {
                                    Screen.lockCursor = true;
                                    Messenger.Default.Send(new LevelStartMessage());
                                    gameObject.SetActive(false);
                                });
    }
}