using UnityEngine;
using System.Collections;

public class GameWonMenu : EndGameMenuBase
{
   
}

public abstract class EndGameMenuBase : ScoreMenuBase
{
    public string levelToLoad;
    public UILabel PositionLabel;

    protected override void Awake()
    {
        base.Awake();
        AdditionalActions.Add(si =>
                                  {
                                      string message = string.Empty;
                                      if (si.Position == 0)
                                      {
                                          message = string.Format("New High Score with {0}pts.", si.TotalScore);
                                      }
                                      else if (si.Position > 0)
                                      {
                                          message = string.Format("You placed {0} with {1}pts.", GetPosition(si), si.TotalScore); 
                                      }
                                      else
                                      {
                                          message = "Goob Job!";
                                      }
                                      PositionLabel.text = message;
                                  });
    }

    private static string GetPosition(ScoreInfo si)
    {
        var position = si.Position + 1;
        switch (position)
        {
            case 2:
                return "2nd";
            case 3:
                return "3rd";
            default:
                return position + "th";
        }
    }

    protected override void Continue()
    {
        Application.LoadLevel(levelToLoad);
    } 
} 
