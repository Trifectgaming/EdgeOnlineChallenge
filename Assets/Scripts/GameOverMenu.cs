using System;
using UnityEngine;
using System.Collections;

public class GameOverMenu : EndGameMenuBase
{
    public UIButtonHandler RetryButton;
    protected override void Awake()
    {
        base.Awake();
        RetryButton.Click += RetryButtonOnClick;
    }

    protected override void Showing()
    {
        base.Showing();
        PositionLabel.text = string.Empty;
    }

    private void RetryButtonOnClick(object sender, ClickEventArgs clickEventArgs)
    {
        Screen.lockCursor = true;
        Messenger.Default.Send(new LevelRetryMessage());
        gameObject.SetActive(false);
    }
}

public class LevelRetryMessage { }
