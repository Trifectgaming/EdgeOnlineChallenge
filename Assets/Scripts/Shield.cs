using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(tk2dAnimatedSprite))]
public class Shield : MonoBehaviour
{
    public ShieldColor color;
    public Transform Transform;
    public AudioClip ImpactSound;
    public bool Started;
    private int _growDamageTaken;
    private int _shrinkDamageTaken;
    private tk2dAnimatedSprite _animation;
    private int _growAnimationClipId;
    private int _shrinkAnimationClipId;
    private int _growClipLength;
    private int _shrinkClipLength;
    private string _growAnimationKey;

    public void Start()
    {
        if (Started) return;
        Started = true;
        Transform = transform;
        _growAnimationKey = color + "ShieldGrow";
        var shrinkAnimationKey = color + "ShieldShrink";
        _animation = GetComponent<tk2dAnimatedSprite>();
        _growAnimationClipId = _animation.GetClipIdByName(_growAnimationKey);
        _shrinkAnimationClipId = _animation.GetClipIdByName(shrinkAnimationKey);
        _growDamageTaken = _growClipLength = _animation.anim.clips[_growAnimationClipId].frames.Length - 1;
        _shrinkClipLength = _animation.anim.clips[_shrinkAnimationClipId].frames.Length - 1;
    }

    public void Reset()
    {
        Start();
        _growDamageTaken = _growClipLength;
        _shrinkDamageTaken = 0;
    }

    public void Grow()
    {
        Start();
        _animation.PlayFromFrame(_growAnimationClipId, 0, _growDamageTaken);
    }

    public void Shrink()
    {
        Start();
        _animation.PlayFromFrame(_shrinkAnimationClipId, _shrinkDamageTaken);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectiles")
        {
            var projectile = collision.gameObject.GetComponent<ProjectileBase>();
            var projectileLocation = projectile.transform.position;
            bool deflected = (int) projectile.ProjectileColor == (int) color;
            if (!deflected)
            {
                AddDamage();
            }
            projectile.Reset();
            Messenger.Default.Send(new ShieldImpactMessage
                {
                    WasDeflected = deflected,
                    Projectile = projectile,
                    ImpactPosition =  collision.contacts.First().point,
                    ProjectilePosition = projectileLocation
                });
            if (ImpactSound != null) audio.PlayOneShot(ImpactSound);
        }
    }

    private void AddDamage()
    {
        --_growDamageTaken;
        ++_shrinkDamageTaken;
        if (_growDamageTaken < 0)
        {
            _growDamageTaken = 0;
        }
        if (_shrinkDamageTaken > _shrinkClipLength)
        {
            _shrinkDamageTaken = _shrinkClipLength;
        }
        if (_animation.CurrentClip.name == _growAnimationKey)
        {
            Grow();
        }
        else
        {
            Shrink();
        }
    }
}