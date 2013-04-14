using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public UIButtonHandler ContinueButton;
    public UIButtonHandler MainMenuButton;
    
    private void Awake()
    {
        ContinueButton.Click += ContinueButtonOnClick;        
    }

    private void Start()
    {
    }

    private void ContinueButtonOnClick(object sender, ClickEventArgs clickEventArgs)
    {
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