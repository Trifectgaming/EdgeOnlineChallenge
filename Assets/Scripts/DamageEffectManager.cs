using System;
using System.Collections;
using UnityEngine;

public class DamageEffectManager : GameSceneObject
{
    public DamageEffect CurrentEffect;
    public float EffectExpirationTime;
    public MouseController MouseController;
    public TapController TapController;
    public bool Initialized;
    public MeshRenderer effectMaterial;
    public Color effectRed;
    public Color effectBlue;
    public Color effectGreen;

    private Color _originalColor;
    private Color _effectColor;
    private bool _started;

    protected override void Awake()
    {
        base.Awake();
        Messenger.Default.Register<ShieldImpactMessage>(this, OnShieldImpact);
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
            _originalColor = effectMaterial.material.GetColor("_TintColor");
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
            switch (obj.Projectile.ProjectileColor)
            {
                case ProjectileColor.Blue:
                    _effectColor = effectBlue;
                    break;
                case ProjectileColor.Green:
                    _effectColor = effectGreen;
                    break;
                case ProjectileColor.Red:
                    _effectColor = effectRed;
                    break;
            }
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
            effectMaterial.material.SetColor("_TintColor", _originalColor);
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
                effectMaterial.material.SetColor("_TintColor", _effectColor);
                break;
            case DamageEffect.Repulse:
                break;
            case DamageEffect.Slow:
                break;
        }
    }
}