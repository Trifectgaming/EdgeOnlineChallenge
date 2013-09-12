using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public static class AdManager
{
    public const string MainMenuTravels = "Main_Menu_Travels";
    public const string BetweenLevels = "Between_Levels";
    
    private static readonly Dictionary<string, AdExpectation> CanShow = new Dictionary<string, AdExpectation>();
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized) return;
        EditorHelper.ExecuteIfNotInEditor(() => HeyzapAds.start(HeyzapAds.FLAG_NO_OPTIONS));
        EditorHelper.ExecuteIfNotInEditor(() => HeyzapAds.setDisplayListener(OnAdChanged));
        RegisterAd(BetweenLevels, 5);
        RegisterAd(MainMenuTravels, 2);
        _initialized = true;
    }

    public static void TryShowAd(string adName, Action dismissed = null)
    {
        if (!_initialized)
        {
            LogHandler.Handle("Tried to show ad " + adName + " without initializing the AdManager.");
            return;
        }
        try
        {
            AdExpectation expectation;
            if (CanShow.TryGetValue(adName, out expectation) && expectation.ShouldShow())
            {
                expectation.Closed = dismissed;
                EditorHelper.ExecuteIfNotInEditor(() => HZInterstitialAd.show(adName), () =>
                                                                                           {
                                                                                               expectation.Closed = null;
                                                                                               if (dismissed != null)
                                                                                                   dismissed();
                                                                                           });
            }
            else
            {
                if (expectation != null)
                {
                    Debug.Log("Expectation not met, ad will show in " + expectation.WillShowIn() + " more event occurances.");
                }
                if (dismissed != null)
                {
                    dismissed();
                }
            }
        }
        catch (Exception e)
        {
            LogHandler.Handle(e);
        }
    }

    private static void OnAdChanged(string state, string tag)
    {
        if (state.Equals("available", StringComparison.InvariantCultureIgnoreCase))
        {
            CanShow[tag].IsReady = true;
        }
        else if (state.Equals("hide", StringComparison.InvariantCultureIgnoreCase))
        {
            var callback = CanShow[tag].Closed;
            if (callback != null)
            {
                callback();
            }
        }
    }

    private static void RegisterAd(string adName, int showFrequency)
    {
        CanShow[adName] = new AdExpectation(showFrequency);
        EditorHelper.ExecuteIfNotInEditor(() => HZInterstitialAd.fetch(adName), () => CanShow[adName].IsReady = true);
    }

    private class AdExpectation
    {
        public AdExpectation(int showFrequency)
        {
            _showFrequency = showFrequency;
        }

        private readonly int _showFrequency;
        private int _eventOccurance;
        public bool IsReady;

        public Action Closed;

        public bool ShouldShow()
        {
            ++_eventOccurance;
            bool result = false;
            if (_showFrequency <= _eventOccurance && IsReady)
            {
                result = true;
                _eventOccurance = 0;
            }
            return result;
        }

        public int WillShowIn()
        {
            return (_showFrequency - _eventOccurance);
        }
    }
}

public class LogHandler
{
    public static List<ExceptionMessage> Exceptions = new List<ExceptionMessage>();
    
    public static bool HasErrors
    {
        get { return Exceptions.Any(); }
    }

    public static void ClearErrors()
    {
        Exceptions.Clear();
    }

    public static void Handle(Exception exception)
    {
        if (Application.isEditor)
        {
            Debug.LogException(exception);
        }
        else
        {
            Handle(exception.ToString());
        }
    }

    private static void AddException(string exception)
    {
        Exceptions.Add(new ExceptionMessage(exception));
    }

    public static void Handle(string exception)
    {
        AddException(exception);
    }
}

public struct ExceptionMessage
{
    public string Message;
    public DateTime Occurance;

    public ExceptionMessage(string exception)
    {
        Message = exception;
        Occurance = DateTime.UtcNow;
    }
}

public static class EditorHelper
{
    public static void ExecuteIfNotInEditor(Expression<Action> attempt, Action alternate = null)
    {
        try
        {
            if (!Application.isEditor)
            {
                attempt.Compile()();
            }
            else
            {
                Debug.Log("Execution requested of: " + attempt.Body);
                if (alternate != null)
                    alternate();
            }
        }
        catch (Exception e)
        {
            LogHandler.Handle(e);
        }
    }
}
