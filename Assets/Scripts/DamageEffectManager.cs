using System.Collections;
using UnityEngine;

public class DamageEffectManager : GameSceneObject
{
    public DamageEffect CurrentEffect;
    public float EffectExpirationTime;
    public Vector3 ProjectileImpact;
    public Vector3 ImpactForce;
    public MouseSensitivity SlowEffect;
    public Drone Drone;
    public MouseController MouseController;
    public TapController TapController;
    public MouseSensitivity previousSensitivity;
    public bool Initialized;
    public MouseSensitivity DisableEffect;
    public MouseSensitivity Repulseffect;
    public float repulseDistance;
    public MeshRenderer effectMaterial;
    public Color effectRed;
    public Color effectBlue;
    public Color effectGreen;

    public float repulseSpeed;
    private Vector3 impactDronePosition;
    private Vector3 targetDronePosition;
    private float _effectTime;
    private Color _originalColor;
    private Color _effectColor;

    protected override void Awake()
    {
        base.Awake();
        Messenger.Default.Register<ShieldImpactMessage>(this, OnShieldImpact);
    }

    protected override void Start()
    {
        base.Start();
        CurrentEffect = DamageEffect.None;
        _originalColor = effectMaterial.material.GetColor("_TintColor");
    }

    private void OnShieldImpact(ShieldImpactMessage obj)
    {
        if (!obj.WasDeflected)
        {
            impactDronePosition = Drone.transform.position;
            targetDronePosition = new Vector3(impactDronePosition.x - repulseDistance, impactDronePosition.y, impactDronePosition.z);
            CurrentEffect = obj.Projectile.DamageEffect;
            EffectExpirationTime = Time.time + obj.Projectile.EffectTime;
            _effectTime = Time.time;
            ProjectileImpact = obj.Projectile.transform.position;
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

    void Update()
    {
        if (!Initialized)
        {
            if (Drone == null)
            {
                var parent = transform.parent;
                Drone = parent.GetComponent<Drone>();
                MouseController = parent.GetComponent<MouseController>();
                TapController = parent.GetComponent<TapController>();
            }
            if (Drone != null && MouseController != null)
            {
                previousSensitivity = MouseController.Sensitivity;
                Initialized = true;
            }
        }
        if (Initialized)
        {
            ApplyEffect();
            if (Time.time > EffectExpirationTime)
                RemoveEffect();
        }
    }

    private void RemoveEffect()
    {
        if (MouseController)
        {
            CurrentEffect = DamageEffect.None;
            MouseController.enabled = true;
            MouseController.Sensitivity = previousSensitivity;
            effectMaterial.material.SetColor("_TintColor", _originalColor);
        }
    }

    void ApplyEffect()
    {
        switch (CurrentEffect)
        {
            case DamageEffect.None:
                return;
            case DamageEffect.Disable:
                if (MouseController != null)
                {
                    MouseController.enabled = false;
                }
                if (TapController != null)
                {
                    TapController.enabled = false;
                }
                effectMaterial.material.SetColor("_TintColor", _effectColor);
                break;
            case DamageEffect.Repulse:
                var distanceCovered = (Time.deltaTime - _effectTime)*repulseSpeed;
                var journey = distanceCovered/repulseDistance;
                Drone.transform.position = Vector3.Lerp(
                    impactDronePosition,
                    targetDronePosition,
                   journey);
                effectMaterial.material.SetColor("_TintColor", _effectColor);
                break;
            case DamageEffect.Slow:
                if (MouseController != null) MouseController.Sensitivity = SlowEffect;
                effectMaterial.material.SetColor("_TintColor", _effectColor);
                break;
        }
    }
}