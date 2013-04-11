using UnityEngine;
using System.Collections;

public class GameOverMenu : ScoreMenuBase
{
    public string levelToLoad;
    // Update is called once per frame
    protected override void Continue()
    {
        Application.LoadLevel(levelToLoad);
    }
}
