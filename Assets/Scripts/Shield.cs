using UnityEngine;

public class Shield : MonoBehaviour
{
    public ShieldColor color;
    public Transform Transform;
    public AudioClip ImpactSound;

    private void Start()
    {
        Transform = transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectiles")
        {
            var projectile = collision.gameObject.GetComponent<ProjectileBase>();
            bool deflected = (int) projectile.ProjectileColor == (int) color;
            projectile.Reset();
            Messenger.Default.Send(new ShieldImpactMessage
                {
                    WasDeflected = deflected,
                    Projectile = projectile,
                });
            if (ImpactSound != null) audio.PlayOneShot(ImpactSound);
        }
    }
}

public class ShieldImpactMessage
{
    public bool WasDeflected;

    public ProjectileBase Projectile;
}