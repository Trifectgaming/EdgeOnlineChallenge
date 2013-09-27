using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public static class AdManager
{
    public const string MainMenuTravels = "Main_Menu";
    public const string BetweenLevels = "Between_Levels";
    public static Action Closed;
    
    private static readonly Dictionary<string, AdExpectation> CanShow = new Dictionary<string, AdExpectation>();
    private static bool _initialized;
    private static string _adShowing = string.Empty;

    public static void Initialize()
    {
        if (_initialized) return;
        EditorHelper.ExecuteIfNotInEditor(() => HeyzapAds.start(HeyzapAds.FLAG_NO_OPTIONS));
        EditorHelper.ExecuteIfNotInEditor(() => HeyzapAds.setDisplayListener(OnAdChanged));
        RegisterAd(BetweenLevels, 5);
        RegisterAd(MainMenuTravels, 2);
        EditorHelper.ExecuteIfNotInEditor(() => HZInterstitialAd.fetch("Default"), 
            () => {
                      foreach (var adExpectation in CanShow)
                      {
                          adExpectation.Value.IsReady = true;
                      }
            });
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
                _adShowing = adName;
                Closed = dismissed;
                EditorHelper.ExecuteIfNotInEditor(() => HZInterstitialAd.show("Default"), () =>
                                                                                           {
                                                                                               Closed = null;
                                                                                               if (dismissed != null)
                                                                                                   dismissed();
                                                                                           });
            }
            else
            {
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
        LogHandler.Handle("Ad " + _adShowing + " has changed to " + state);
        if (state.Equals("available", StringComparison.InvariantCultureIgnoreCase))
        {
            foreach (var adExpectation in CanShow)
            {
                adExpectation.Value.IsReady = true;                
            }
        }
        else if (state.Equals("hide", StringComparison.InvariantCultureIgnoreCase) || state.Equals("click", StringComparison.InvariantCultureIgnoreCase))
        {
            _adShowing = string.Empty;
            if (Closed != null)
            {
                Closed();
                Closed = null;
            }
        }
    }

    private static void RegisterAd(string adName, int showFrequency)
    {
        CanShow[adName] = new AdExpectation(showFrequency);
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
        var ex = new ExceptionMessage(exception);
        if (!Exceptions.Contains(ex))
        {
            Exceptions.Add(ex);
        }
        else
        {
            var oldEx = Exceptions.IndexOf(ex);
            if (oldEx >= 0)
            {
                ex.Count += Exceptions[oldEx].Count;
                Exceptions.RemoveAt(oldEx);
            }
            Exceptions.Add(ex);
        }
    }

    public static void Handle(string exception)
    {
        AddException(exception);
    }
}

public struct ExceptionMessage
{
    public bool Equals(ExceptionMessage other)
    {
        return string.Equals(Message, other.Message);
    }

    public override int GetHashCode()
    {
        return (Message != null ? Message.GetHashCode() : 0);
    }

    public string Message;
    public DateTime Occurance;
    public int Count;

    public ExceptionMessage(string exception)
    {
        Message = exception;
        Occurance = DateTime.UtcNow;
        Count = 1;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is ExceptionMessage && Equals((ExceptionMessage) obj);
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
