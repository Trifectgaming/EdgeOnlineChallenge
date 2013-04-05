using System;
using UnityEngine;

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
    public int currentLevel = 0;
    public int currentWave = 0;
    private AudioClip BGM;
    public AudioClip EndGameAudio;
    public MeshRenderer Background;
    public float WaveDelaySeconds = 1;
    public GameObject WaveText;

    public Level[] Levels;

    private static int _lanes;
    private static Rail[] _rails;
    private tk2dTextMesh _waveText;
    private Animation _waveAnimation;
    private bool _isPaused;

    private void Awake()
    {
        Messenger.Default.Register<GameResumeMessage>(this, OnGameResume);
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
        Messenger.Default.Register<WaveEndMessage>(this, OnWaveEnd);
        Messenger.Default.Register<LevelStartMessage>(this, OnLevelStart);
    }

    private void OnLevelStart(LevelStartMessage obj)
    {
        var level = Levels[currentLevel];
        currentWave = 0;        
        BGM = level.BGM;
        SetupBGM();
        Background.material = level.Background;
        SendWaveMessage();
    }

    private void OnWaveEnd(WaveEndMessage obj)
    {
        var nextWave = currentWave + 1;
        if (nextWave < Levels[currentLevel].Waves.Length)
        {
            currentWave = nextWave;
            Debug.Log("Wave " + currentWave + " of level " + currentLevel + " started.");
            SendWaveMessage();
        }
        else
        {
            var nextLevel = currentLevel + 1;
            if (nextLevel < Levels.Length)
            {
                currentLevel = nextLevel;
                Messenger.Default.Send(new LevelEndMessage());
                Messenger.Default.Send(new LevelStartMessage());
            }
            else
            {
                Messenger.Default.Send(new GameWonMessage());
            }
        }
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
        _isPaused = false;
    }

    private void Start()
    {
        _waveText = WaveText.GetComponent<tk2dTextMesh>();
        _waveAnimation = WaveText.GetComponent<Animation>();
        OnLevelStart(null);
        SetupBGM();
        CreateRails();
        Time.timeScale = 1;
        Debug.Log("GameManager Started");
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
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
                SendLevelStart(i - 1);
        }
    }

    private void SendLevelStart(int i)
    {
        currentLevel = i;
        currentWave = 0;
        if (Levels.Length == 0 || 
            currentLevel > Levels.Length - 1 || 
            Levels[currentLevel].Waves.Length == 0 || 
            currentWave > Levels[currentLevel].Waves.Length - 1) return;
        
        SendWaveMessage();
    }

    private void SendWaveMessage()
    {
        _waveText.text = "Level " + (currentLevel + 1) + "\nWave " + (currentWave + 1) + "\nBegin...";
        _waveText.Commit();
        _waveAnimation.Play();
        Invoke("FireWaveMessage", 2);
    }

    private void FireWaveMessage()
    {
        Messenger.Default.Send(new WaveBeginMessage
                                   {
                                       WaveInfo = Levels[currentLevel].Waves[currentWave]
                                   });
    }

    private void Pause()
    {
        Time.timeScale = 0;
        Screen.lockCursor = false; 
        Messenger.Default.Send(new GamePausedMessage());
        PauseMenu.Show();
        _isPaused = true;
    }

    public bool IsPaused()
    {
        return Time.timeScale == 0 || _isPaused;
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
public class Level
{
    public Wave[] Waves;
    public AudioClip BGM;
    public Material Background;
}

[Serializable]
public class Wave
{
    public ProjectileInfo[] Projectiles;
    public int Count;
    public float LaunchDelaySeconds;
    public float PositionUpdateDelaySeconds;
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