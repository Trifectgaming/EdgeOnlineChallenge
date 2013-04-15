using UnityEngine;

public class CreditMenu : MonoBehaviour
{
    public UIButtonHandler ContinueButton;

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

    protected void Continue()
    {
        Exit();
    }

    protected void Update()
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

    public void Show()
    {
        gameObject.SetActive(true);
    }
}