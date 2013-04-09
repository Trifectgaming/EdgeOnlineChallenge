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
    public GameOverMenu GameOverMenu;
    public GameWonMenu GameWonMenu;
    public ScoreMenu ScoreMenu;
    public ScoreManager ScoreManager;
    public int lanes;
    public int allowedRailDamage;
    private int _currentLevel = 0;
    private int _currentWaveCount = 0;
    private AudioClip BGM;
    public AudioClip EndGameAudio;
    public AudioClip WonGameAudio;
    public MeshRenderer Background;
    public float WaveDelaySeconds = 1;
    public GameObject WaveText;
    public bool isEndless;

    public Level[] Levels;

    private static int _lanes;
    private static Rail[] _rails;
    private tk2dTextMesh _waveText;
    private Animation _waveAnimation;
    private bool _isPaused;
    private bool _isScoring;
    public Wave CurrentWave;
    private bool _isGameOver;

    protected virtual void Awake()
    {
        Messenger.Default.Register<GameResumeMessage>(this, OnGameResume);
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
        Messenger.Default.Register<GameWonMessage>(this, OnGameWon);
        Messenger.Default.Register<WaveEndMessage>(this, OnWaveEnd);
        Messenger.Default.Register<LevelEndMessage>(this, OnLevelEnd);
        Messenger.Default.Register<LevelStartMessage>(this, OnLevelStart);
    }

    private void OnLevelEnd(LevelEndMessage obj)
    {
        _isScoring = true;
        var levelScore = ScoreManager.Calculate();
        ScoreMenu.Show(levelScore);
    }

    private void OnGameWon(GameWonMessage obj)
    {
        _isGameOver = true;
        Screen.lockCursor = false;
        BGM = WonGameAudio;
        SetupBGM();
        GameWonMenu.Show();
    }

    private void OnLevelStart(LevelStartMessage obj)
    {
        _isScoring = false;        
        var level = Levels[_currentLevel];
        _currentWaveCount = 0;        
        BGM = level.BGM;
        SetupBGM();
        Background.material = level.Background;
        if (!isEndless)
        {
            CurrentWave = Levels[_currentLevel].Waves[_currentWaveCount];
        }
        else
        {
            CurrentWave = Levels[_currentLevel].Waves[2];
        }
        SendWaveMessage();
    }

    private void OnWaveEnd(WaveEndMessage obj)
    {
        if (_isGameOver) return;
        if (!isEndless)
        {
            var nextWave = _currentWaveCount + 1;
            if (nextWave < Levels[_currentLevel].Waves.Length)
            {
                _currentWaveCount = nextWave;
                CurrentWave = Levels[_currentLevel].Waves[_currentWaveCount];
                Debug.Log("Wave " + _currentWaveCount + " of level " + _currentLevel + " started.");
                SendWaveMessage();
            }
            else
            {
                var nextLevel = _currentLevel + 1;
                if (nextLevel < Levels.Length)
                {
                    _currentLevel = nextLevel;
                    _isPaused = true;
                    Messenger.Default.Send(new LevelEndMessage());
                }
                else
                {
                    BGM = WonGameAudio;
                    SetupBGM();
                    Messenger.Default.Send(new GameWonMessage());
                }
            }
        }
        else
        {
            IncrementEndlessWave();
        }
    }

    private void OnGameOver(GameOverMessage obj)
    {
        GameOver();
    }

    private void GameOver()
    {
        _isGameOver = true; 
        Screen.lockCursor = false;
        BGM = EndGameAudio;
        SetupBGM();
        GameOverMenu.Show();
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
        Messenger.Default.Send(new LevelStartMessage());
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
        
        if (!isEndless)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                    SendLevelStart(i - 1);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                IncrementEndlessWave();
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
                DecrementEndlessWave();
        }

        if (_isGameOver || _isScoring) return;
        if (Input.GetMouseButtonDown(0) && !IsPaused())
        {
            Screen.lockCursor = true;
        }
        if (Input.GetKeyDown("escape"))
        {
            Pause();
        }
    }

    private void DecrementEndlessWave()
    {
        _currentWaveCount--;
        var original = CurrentWave;
        CurrentWave = new Wave
        {
            Count = original.Count - 2,
            LaunchDelaySeconds = original.LaunchDelaySeconds + .05f,
            PositionUpdateDelaySeconds = original.PositionUpdateDelaySeconds + .1f,
            Projectiles = original.Projectiles,
        };
        SendWaveMessage();
    }

    private void IncrementEndlessWave()
    {
        _currentWaveCount++;
        var original = CurrentWave;
        CurrentWave = new Wave
                           {
                               Count = original.Count + 2,
                               LaunchDelaySeconds = original.LaunchDelaySeconds  - .05f,
                               PositionUpdateDelaySeconds = original.PositionUpdateDelaySeconds - .01f,
                               Projectiles = original.Projectiles,
                           };
        SendWaveMessage();
    }

    private void SendLevelStart(int i)
    {
        _currentLevel = i;
        _currentWaveCount = 0;
        if (Levels.Length == 0 || 
            _currentLevel > Levels.Length - 1 || 
            Levels[_currentLevel].Waves.Length == 0 || 
            _currentWaveCount > Levels[_currentLevel].Waves.Length - 1) return;
        
        Messenger.Default.Send(new LevelStartMessage());
        SendWaveMessage();
    }

    private void SendWaveMessage()
    {
        if (!isEndless)
        {
            _waveText.text = "Level " + (_currentLevel + 1) + "\nWave " + (_currentWaveCount + 1) + "\nBegin...";
        }
        else
        {
            _waveText.text = "Wave " + (_currentWaveCount + 1) + "\nBegin...";
        }
        _waveText.Commit();
        _waveAnimation.Play();
        Invoke("FireWaveMessage", 2);
    }

    private void FireWaveMessage()
    {
        Messenger.Default.Send(new WaveBeginMessage
                                   {
                                       WaveInfo = CurrentWave,
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

    void OnDrawGizmos()
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