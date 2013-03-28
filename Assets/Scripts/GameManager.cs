using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static int GetLanes()
    {
        return _lanes;
    }

    public static Rail[] GetRails()
    {
        return _rails;
    }

    public PauseMenu PauseMenu;
    public int lanes;
    public int allowedRailDamage;
    public AudioClip BGM;
    public AudioClip EndGameAudio;

    private static int _lanes;
    private static Rail[] _rails;
    
    private void Awake()
    {
        Messenger.Default.Register<GameResumeMessage>(this, OnGameResume);
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
    }

    private void OnGameOver(GameOverMessage obj)
    {
        audio.Stop();
        audio.clip = EndGameAudio;
        audio.Play();
        Pause();
    }

    private void OnGameResume(GameResumeMessage obj)
    {
        Screen.lockCursor = true;
        Time.timeScale = 1;
        SetupBGM();
    }

    private void Start()
    {
        SetupBGM();
        CreateRails();
    }

    private void SetupBGM()
    {
        audio.Stop();
        audio.clip = BGM;
        audio.Play();
    }

    private void CreateRails()
    {
        _lanes = lanes;
        var laneHeight = (UIHelper.MaxY - UIHelper.MinY)/(float) _lanes;
        _rails = new Rail[_lanes];
        for (int r = 0; r < _lanes; r++)
        {
            _rails[r] = new Rail {Start = UIHelper.MaxY - (laneHeight*r)};
            _rails[r].End = _rails[r].Start - laneHeight;
            _rails[r].Center = _rails[r].Start - (laneHeight/2);
            _rails[r].DamageTaken = 0;
            _rails[r].AllowedDamage = allowedRailDamage;
        }
    }

    private void Update()
    {
        if (_lanes != lanes)
            CreateRails();
        if (Input.GetKeyDown("escape"))
        {
            Pause();
        }
        if (Input.GetMouseButtonDown(0) && !IsPaused())
        {
            Screen.lockCursor = true;
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
        Screen.lockCursor = false; 
        Messenger.Default.Send(new GamePausedMessage());
        PauseMenu.Show();
        
    }

    public bool IsPaused()
    {
        return Time.timeScale == 0;
    }

    private void OnApplicationPause(bool paused)
    {
        print("Game paused.");
        Pause();
    }

    void OnDrawGizmosSelected()
    {
        if (_rails == null)
        {
            CreateRails();
        }
        foreach (var rail in _rails)
        {
            Gizmos.DrawLine(new Vector3(UIHelper.MinX, rail.Center, -1), new Vector3(UIHelper.MaxX, rail.Center,-1));
        }
    }
}

[Serializable]
public class Rail
{
    public float Start;
    public float End;
    public float Center;
    public int DamageTaken;
    public int AllowedDamage;
}

public class GameResumeMessage { }
public class GamePausedMessage { }