using UnityEngine;

public class GameSceneObject : MonoBehaviour
{
    protected virtual void Awake()
    {
        Messenger.Default.Register<GameResumeMessage>(this, OnGameResume);
        Messenger.Default.Register<GamePausedMessage>(this, OnGamePaused);
    }

    protected virtual void OnGamePaused(GamePausedMessage obj)
    {
        enabled = false;
    }

    protected virtual void OnGameResume(GameResumeMessage obj)
    {
        enabled = true;
    }

    protected virtual void Start()
    {
    }

    protected void OnDestroy()
    {
        Messenger.Default.Unregister(this);
    }
}