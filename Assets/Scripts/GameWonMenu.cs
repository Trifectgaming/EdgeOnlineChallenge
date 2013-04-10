using UnityEngine;
using System.Collections;

public class GameWonMenu : ScoreMenuBase
{
	protected override void Continue()
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
