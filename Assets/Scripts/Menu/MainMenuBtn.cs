using UnityEngine;
using System.Collections;

public class MainMenuBtn : UIButtonHandler
{
    public string levelToLoad;
    public bool isEndless;
    public bool shouldLockCursor;

    // Update is called once per frame
	void Update () {
	    
	}

    protected override void RaiseClick(ClickEventArgs e)
    {
        Application.LoadLevel(levelToLoad);
        GameManager.IsEndless = isEndless;
        if (shouldLockCursor)
            Screen.lockCursor = true;

    }
}
