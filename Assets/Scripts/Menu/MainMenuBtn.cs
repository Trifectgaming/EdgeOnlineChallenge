using UnityEngine;
using System.Collections;

public class MainMenuBtn : UIButtonHandler
{
    public string levelToLoad;
    public bool shouldLockCursor;

    protected override void RaiseClick(ClickEventArgs e)
    {
        base.RaiseClick(e);
        if (shouldLockCursor)
            Screen.lockCursor = true;
        if (levelToLoad == "MenuScene")
        {
            AdManager.TryShowAd(AdManager.MainMenuTravels, () => Application.LoadLevel(levelToLoad));
        }
        else
        {
            Application.LoadLevel(levelToLoad);            
        }
    }
}
