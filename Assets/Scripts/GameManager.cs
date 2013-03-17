using System.Linq;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public PauseMenu PauseMenu;
    public GameObject[] GameSceneObjects;
    public float previousFixedDelta;

    private void Awake()
    {
        Messenger.Default.Register<GameResumeMessage>(this, OnGameResume);
    }

    private void OnGameResume(GameResumeMessage obj)
    {
        Screen.lockCursor = true;
        Time.timeScale = 1;
    }

    private void Start()
    {
        
    }
    
    private void DisableGameScene()
    {
        foreach (var gameSceneObject in GameSceneObjects)
        {
            gameSceneObject.SetActive(false);
        }
    }

    private void EnableGameScene()
    {
        foreach (var gameSceneObject in GameSceneObjects)
        {
            gameSceneObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Screen.lockCursor = false;
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
}

public class GameResumeMessage { }
public class GamePausedMessage { }