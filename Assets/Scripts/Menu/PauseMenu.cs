using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public UIButtonHandler ContinueButton;
    public UIButtonHandler MainMenuButton;
    
    private void Start()
    {
        ContinueButton.Click += ContinueButtonOnClick;
    }

    private void ContinueButtonOnClick(object sender, ClickEventArgs clickEventArgs)
    {
        print("Continue Clicked");
        Continue();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Continue();
        }
    }

    private void Continue()
    {
        Messenger.Default.Send(new GameResumeMessage());
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Debug.Log("PauseMenu.Show");
        gameObject.SetActive(true);
    }
}
