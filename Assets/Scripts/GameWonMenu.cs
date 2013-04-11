using UnityEngine;
using System.Collections;

public class GameWonMenu : ScoreMenuBase
{
    public string levelToLoad;

    protected override void Continue()
    {
        Application.LoadLevel(levelToLoad);
    }
}
