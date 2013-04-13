using UnityEngine;
using System.Collections;

public class MainMenuBtn : UIButtonHandler
{
    public string levelToLoad;
    public bool shouldLockCursor;

    // Update is called once per frame
	void Update () {
	    
	}

    protected override void RaiseClick(ClickEventArgs e)
    {
        base.RaiseClick(e);
        if (shouldLockCursor)
            Screen.lockCursor = true;
        Application.LoadLevel(levelToLoad);
    }
}
