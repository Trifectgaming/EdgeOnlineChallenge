using System.Linq;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public ShieldColor color;
    public Transform Transform;
    public AudioClip ImpactSound;
    private tk2dAnimatedSprite _growAnim;
    private tk2dAnimatedSprite _shrinkAnim;

    private void Start()
    {
        Transform = transform;
        var anims = gameObject.GetComponentsInChildren<tk2dAnimatedSprite>();
        _shrinkAnim = anims.First(t => t.name == name + "Shrink");
        _shrinkAnim.renderer.enabled = false;
        _growAnim = anims.First(t => t.name == name + "Grow");
        _growAnim.renderer.enabled = false;
    }

    public void Grow()
    {
        _growAnim.renderer.enabled = true;
        _shrinkAnim.renderer.enabled = false;
        _growAnim.Play();
    }

    public void Shrink()
    {
        _shrinkAnim.renderer.enabled = true;
        _growAnim.renderer.enabled = false;
        _shrinkAnim.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectiles")
        {
            var projectile = collision.gameObject.GetComponent<ProjectileBase>();
            bool deflected = (int) projectile.ProjectileColor == (int) color;
            var impactPosition = projectile.transform.position;
            projectile.Reset();
            Messenger.Default.Send(new ShieldImpactMessage
                {
                    WasDeflected = deflected,
                    Projectile = projectile,
                    ImpactPosition =  impactPosition
                });
            if (ImpactSound != null) audio.PlayOneShot(ImpactSound);
        }
    }
}