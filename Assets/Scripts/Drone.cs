using System;
using UnityEngine;
using System.Collections;

public class Drone : GameSceneObject
{
    public tk2dAnimatedSprite sprite;

    protected override void Start ()
    {
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
