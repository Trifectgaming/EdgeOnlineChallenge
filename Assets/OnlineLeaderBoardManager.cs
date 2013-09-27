using System;
using UnityEngine;
using System.Collections;

public static class OnlineLeaderBoardManager
{
    public const string EndlessBoard = "fk3";
	
    public static void Show(string leaderBoard)
    {
        try
        {
            EditorHelper.ExecuteIfNotInEditor(() => Heyzap.showLeaderboards(leaderBoard));
        }
        catch (Exception e)
        {
            LogHandler.Handle(e);
        }
    }

    public static void Submit(int totalScore, string leaderBoard)
    {
        try
        {
            EditorHelper.ExecuteIfNotInEditor(() => Heyzap.submitScore(totalScore, totalScore + " points", leaderBoard));
        }
        catch (Exception e)
        {
            LogHandler.Handle(e);
        }
    }
}
