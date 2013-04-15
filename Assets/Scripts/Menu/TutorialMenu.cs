using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    public UIButtonHandler ContinueButton;
    public UISprite RedTutorial;
    public UISprite GreenTutorial;
    public UISprite BlueTutorial;
    public UISprite ControlTutorial;

    private void Awake()
    {
        ContinueButton.Click += ContinueButtonOnClick;
    }

    private void Start()
    {
        
    }

    protected void Reset()
    {
        RedTutorial.alpha = 0;
        GreenTutorial.alpha = 0;
        BlueTutorial.alpha = 0;
        ControlTutorial.alpha = 0;
    }

    private void ContinueButtonOnClick(object sender, ClickEventArgs clickEventArgs)
    {
        Continue();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Continue();
        }
    }

    protected virtual void Continue()
    {
        Messenger.Default.Send(new GameResumeMessage());
        gameObject.SetActive(false);
    }

    public virtual void Show(Tutorial tutorial)
    {
        Messenger.Default.Send(new GamePausedMessage());
        Reset();
        gameObject.SetActive(true);
        switch (tutorial)
        {
            case Tutorial.Controls:
                ControlTutorial.alpha = 255;
                break;
            case Tutorial.Red:
                RedTutorial.alpha = 255;
                break;
            case Tutorial.Green:
                GreenTutorial.alpha = 255;
                break;
            case Tutorial.Blue:
                BlueTutorial.alpha = 255;
                break;
        }
    }
}