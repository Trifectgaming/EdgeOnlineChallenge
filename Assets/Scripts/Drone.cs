using UnityEngine;

public class Drone : GameSceneObject
{
    public tk2dAnimatedSprite sprite;
    public ShieldManager shieldManager;
    public MouseController controller;

    protected override void Start ()
    {
        controller = GetComponent<MouseController>();
        base.Start();
    }

    protected override void OnGamePaused(GamePausedMessage obj)
    {
        base.OnGamePaused(obj);
        sprite.enabled = false;
    }

    protected override void OnGameResume(GameResumeMessage obj)
    {
        base.OnGameResume(obj);
        sprite.enabled = true;
    }

    private void Update()
    {

    }
}