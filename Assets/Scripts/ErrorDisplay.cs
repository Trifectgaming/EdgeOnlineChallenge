using UnityEngine;
using System.Collections;

public class ErrorDisplay : MonoBehaviour {
    public UIButtonHandler ContinueButton;
    public UITable ContentControl;
    public ErrorContent ContentTemplate;

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

    private void OnEnable()
    {
        if (ContentControl.children.Count < LogHandler.Exceptions.Count)
            for (int index = ContentControl.children.Count; index < LogHandler.Exceptions.Count; index++)
            {
                var exceptionMessage = LogHandler.Exceptions[index];
                var content = ((ErrorContent) Instantiate(ContentTemplate));
                content.transform.parent = ContentControl.transform;
                content.transform.localScale = new Vector3(1, 1, 1);
                content.transform.position = new Vector3(0,0,-2);
                content.SetContent(exceptionMessage);
            }
        ContentControl.repositionNow = true;
    }
}