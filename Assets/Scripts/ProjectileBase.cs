using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileBase : MonoBehaviour
{
    public ProjectileColor ProjectileColor;
    private Rigidbody _rigidbody;
    public float Speed;
    public float Velocity;
    private ForceMode _mode;
    public Vector3 initialPosition;
    public float EffectTime = 0;
    public DamageEffect DamageEffect;
    public int Damage;
    public int CurrentRail;
    public AudioClip LaunchSound;
    public ParticleSystem thruster;

    protected void Start ()
	{
        initialPosition = transform.position;
	}
	
	void Update () {
	    
	}

    public void Launch(float speed, ForceMode mode)
    {
        Speed = speed;
        _mode = mode;
        enabled = true;
        gameObject.SetActive(true);
        var r = rigidbody;
        r.velocity = Vector3.zero;
        r.angularVelocity = Vector3.zero;
        r.inertiaTensorRotation = Quaternion.identity;
        r.rotation = Quaternion.identity;
        r.inertiaTensor = new Vector3(1,1,1);
        r.isKinematic = true;
        r.isKinematic = false;
        renderer.enabled = true;
        if (LaunchSound)
        {
            audio.PlayOneShot(LaunchSound);
        }
        if (thruster)
        {
            thruster.Play(true);
        }
    }

    void FixedUpdate()
    {
        rigidbody.AddForce(Vector3.left * Speed, _mode);
        Velocity = rigidbody.velocity.x;
    }

    public void Reset()
    {
        enabled = false;
        renderer.enabled = false;
        transform.position = initialPosition;
        gameObject.SetActive(false);
        if (thruster) thruster.Stop(true);
    }
}

public enum DamageEffect
{
    None,
    Repulse,
    Slow,
    Disable
}
