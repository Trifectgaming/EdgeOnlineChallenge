using System.Linq;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public ShieldColor color;
    public Transform Transform;
    public AudioClip ImpactSound;
    private tk2dAnimatedSprite _growAnim;
    private tk2dAnimatedSprite _shrinkAnim;
    public bool Started;

    public void Start()
    {
        if (Started) return;
        Started = true;
        Transform = transform;
        var anims = gameObject.GetComponentsInChildren<tk2dAnimatedSprite>();
        Debug.Log(anims.Length + " anims found");
        _shrinkAnim = anims.First(t => t.name == name + "Shrink");
        _shrinkAnim.renderer.enabled = false;
        _growAnim = anims.First(t => t.name == name + "Grow");
        _growAnim.renderer.enabled = false;
    }

    public void Grow()
    {
        Start();
        _growAnim.renderer.enabled = true;
        _shrinkAnim.renderer.enabled = false;
        _growAnim.Play();
    }

    public void Shrink()
    {
        Start();
        _shrinkAnim.renderer.enabled = true;
        _growAnim.renderer.enabled = false;
        _shrinkAnim.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectiles")
        {
            var projectile = collision.gameObject.GetComponent<ProjectileBase>();
            var projectileLocation = projectile.transform.position;
            bool deflected = (int) projectile.ProjectileColor == (int) color;
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
}