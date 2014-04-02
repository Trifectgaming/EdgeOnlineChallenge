using System;
using System.Collections;
using UnityEngine;

public class DamageEffectManager : GameSceneObject
{
    public DamageEffect CurrentEffect;
    public float EffectExpirationTime;
    public MouseController MouseController;
    public TapController TapController;
    public tk2dAnimatedSprite damageAnimation;
    public bool Initialized;
    private bool _started;

    protected override void Awake()
    {
        base.Awake();
        Messenger.Default.Register<ShieldImpactMessage>(this, OnShieldImpact);
        damageAnimation.animationCompleteDelegate += (sprite, id) =>
                                                         {
                                                             damageAnimation.renderer.enabled = false;
                                                         };
    }

    void OnDestroy()
    {
        Messenger.Default.Unregister(this);
    }

    protected override void Start()
    {
        try
        {
            if (_started) return;
            base.Start();
            var parent = transform.parent;
            MouseController = parent.GetComponent<MouseController>();
            TapController = parent.GetComponent<TapController>();
            CurrentEffect = DamageEffect.None;
            damageAnimation.renderer.enabled = false;
            damageAnimation.Stop();
            _started = true;
        }
        catch (Exception e)
        {
            LogHandler.Handle(e);
        }
    }

    private void OnShieldImpact(ShieldImpactMessage obj)
    {
        if (!obj.WasDeflected)
        {
            CurrentEffect = obj.Projectile.DamageEffect;
            EffectExpirationTime = Time.time + obj.Projectile.EffectTime;
            damageAnimation.renderer.enabled = true;            
            damageAnimation.Play();
        }
    }

    private void Update()
    {
        ApplyEffect();
        if (Time.time > EffectExpirationTime)
        {
            RemoveEffect();
        }
    }

    private void RemoveEffect()
    {
        Start();
        if (MouseController)
        {
            CurrentEffect = DamageEffect.None;
            MouseController.enabled = true;
        }
        if (TapController)
        {
            TapController.enabled = true;
        }
    }

    void ApplyEffect()
    {
        Start();
        switch (CurrentEffect)
        {
            case DamageEffect.None:
                return;
            case DamageEffect.Disable:
                if (MouseController)
                {
                    MouseController.enabled = false;
                }
                if (TapController)
                {
                    TapController.enabled = false;
                }
                break;
            case DamageEffect.Repulse:
                break;
            case DamageEffect.Slow:
                break;
        }
    }
}