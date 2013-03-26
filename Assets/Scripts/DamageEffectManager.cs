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
        //StartCoroutine(DelayForEffect());
    }

    private void OnShieldImpact(ShieldImpactMessage obj)
    {
        if (!obj.WasDeflected)
        {
            Debug.Log("Missed");
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
    //protected override void OnGameLateResume(GameLateResumeMessage obj)
    {
        if (!Initialized)
        {
            if (Drone == null)
            {
                var parent = transform.parent;
                Drone = parent.GetComponent<Drone>();
            }
            if (Drone != null && Drone.controller != null)
            {
                previousSensitivity = Drone.controller.Sensitivity;
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
        Debug.Log("Expiring " + CurrentEffect);
        CurrentEffect = DamageEffect.None;
        Drone.controller.Sensitivity = previousSensitivity;
        effectMaterial.material.SetColor("_TintColor", _originalColor);
    }

    void ApplyEffect()
    {
        switch (CurrentEffect)
        {
            case DamageEffect.None:
                return;
            case DamageEffect.Disable:
                Drone.controller.Sensitivity = DisableEffect;
                effectMaterial.material.SetColor("_TintColor", _effectColor);
                break;
            case DamageEffect.Repulse:
                //Drone.controller.Sensitivity = Repulseffect;
                var distanceCovered = (Time.time - _effectTime)*repulseSpeed;
                var journey = distanceCovered/repulseDistance;
                Drone.transform.position = Vector3.Lerp(
                    impactDronePosition,
                    targetDronePosition,
                   journey);
                effectMaterial.material.SetColor("_TintColor", _effectColor);
                break;
            case DamageEffect.Slow:
                Drone.controller.Sensitivity = SlowEffect;
                effectMaterial.material.SetColor("_TintColor", _effectColor);
                break;
        }
        Debug.Log("Applying " + CurrentEffect);
    }
}