using UnityEngine;
using System.Collections;

public class GameOverMenu : ScoreMenuBase
{
	// Update is called once per frame
    protected override void Continue()
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
