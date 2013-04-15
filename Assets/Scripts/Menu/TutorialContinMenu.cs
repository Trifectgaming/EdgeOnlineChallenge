using UnityEngine;

public class TutorialContinMenu : TutorialMenu
{
    private Tutorial _currentTutorial;

    private void Awake()
    {
        ContinueButton.Click += ContinueButtonOnClick;
    }

    private void Start()
    {

    }

    protected void ContinueButtonOnClick(object sender, ClickEventArgs clickEventArgs)
    {
        Continue();
    }

    protected override void Continue()
    {
        Reset();
        switch (_currentTutorial)
        {
            case Tutorial.Controls:
                RedTutorial.alpha = 255;
                _currentTutorial = Tutorial.Red;
                break;
            case Tutorial.Red:
                GreenTutorial.alpha = 255;
                _currentTutorial = Tutorial.Green;
                break;
            case Tutorial.Green:
                BlueTutorial.alpha = 255;
                _currentTutorial = Tutorial.Blue;
                break;
            case Tutorial.Blue:
                Exit();
                break;
        }
    }

    override protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }

    private void Exit()
    {
        Messenger.Default.Send(new GameResumeMessage());
        gameObject.SetActive(false);
    }

    public override void Show(Tutorial tutorial)
    {
        base.Show(tutorial);
        _currentTutorial = tutorial;
    }
}