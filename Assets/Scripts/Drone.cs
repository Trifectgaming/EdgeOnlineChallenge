using System;
using UnityEngine;
using System.Collections;

public class Drone : GameSceneObject
{
    public tk2dAnimatedSprite sprite;
    private MouseController _controller;
    private ShieldManager _shieldManager;
    private tk2dAnimatedSprite _sprite;

    private void OnAwake()
    {
       Messenger.Default.Register<ShieldImpactMessage>(this, OnShieldImpact);
    }

    private void OnShieldImpact(ShieldImpactMessage obj)
    {
        if (!obj.WasDeflected)
        {
            if (obj.Projectile.DamageEffect == DamageEffect.Slow)
            {
                
            }
            else if (obj.Projectile.DamageEffect == DamageEffect.Repulse)
            {
                
            }
            else if (obj.Projectile.DamageEffect == DamageEffect.Disable)
            {
                 
            }
        }
    }

    protected override void Start ()
    {
        base.Start();
        _controller = GetComponent<MouseController>();
        _shieldManager = GetComponentInChildren<ShieldManager>();
        _sprite = GetComponentInChildren<tk2dAnimatedSprite>();
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
