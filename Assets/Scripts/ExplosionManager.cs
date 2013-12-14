using System;
using UnityEngine;
using System.Collections;

public class ExplosionManager : MonoBehaviour
{
    public int count;
    public tk2dAnimatedSprite prefab;
    public tk2dAnimatedSprite redExplosionPrefab;
    public ParticleSystem motherExplosionPrefabs;
    public ParticleSystem shieldBlockPrefabs;
    private RecycleQueue<tk2dAnimatedSprite> _recycleQueue;
    private RecycleQueue<ParticleSystem> _motherExplosionQueues;
    private RecycleQueue<ParticleSystem> _shieldBlockQueue;
    private RecycleQueue<tk2dAnimatedSprite> _redExplosionQueue;

    void Awake()
    {
        Messenger.Default.Register<ShieldImpactMessage>(this, OnShieldImpact);
        Messenger.Default.Register<MotherImpactMessage>(this, OnMotherImpact);
    }

    void OnDestroy()
    {
        Messenger.Default.Unregister(this);
    }

    private void OnMotherImpact(MotherImpactMessage obj)
    {
            var explosion = _motherExplosionQueues.Next();
            explosion.renderer.enabled = true;
            explosion.transform.position = new Vector3(obj.ImpactPosition.x, obj.ImpactPosition.y, transform.position.z);
            explosion.Play();
    }

    private void OnShieldImpact(ShieldImpactMessage obj)
    {
        if (!obj.WasDeflected)
        {
            var explosion = _recycleQueue.Next();
            explosion.renderer.enabled = true;
            explosion.transform.position = new Vector3(obj.ImpactPosition.x, obj.ImpactPosition.y,
                                                       transform.position.z);
            explosion.Play();
        }
        else
        {
            if (obj.Projectile.ProjectileColor == ProjectileColor.Blue)
            {
                var explosion = _shieldBlockQueue.Next();
                explosion.transform.position = new Vector3(obj.ImpactPosition.x, obj.ImpactPosition.y, transform.position.z);
                explosion.startColor = Color.blue;
                explosion.Play();
            }
            else if (obj.Projectile.ProjectileColor == ProjectileColor.Red)
            {
                var explosion = _redExplosionQueue.Next();
                explosion.renderer.enabled = true;
                explosion.transform.position = new Vector3(obj.ProjectilePosition.x, obj.ProjectilePosition.y,
                                                           transform.position.z);
                explosion.Play();
            }
            else
            {
                var explosion = _shieldBlockQueue.Next();
                explosion.transform.position = new Vector3(obj.ImpactPosition.x, obj.ImpactPosition.y, transform.position.z);
                explosion.startColor = Color.green;
                explosion.Play();
            }
        }
    }

    // Use this for initialization
	void Start ()
	{
        _motherExplosionQueues = new RecycleQueue<ParticleSystem>(count, motherExplosionPrefabs, transform.position);
	    foreach (var system in _motherExplosionQueues)
	    {
	        system.Stop(true);
	    }

        _shieldBlockQueue = new RecycleQueue<ParticleSystem>(count, shieldBlockPrefabs, transform.position);
        foreach (var system in _shieldBlockQueue)
        {
            system.Stop(true);
        }

        _redExplosionQueue = new RecycleQueue<tk2dAnimatedSprite>(count, redExplosionPrefab, transform.position);
        foreach (var system in _redExplosionQueue)
        {
            system.renderer.enabled = false;
            system.animationCompleteDelegate = OnAnimationCompleted;
        }

	    _recycleQueue = new RecycleQueue<tk2dAnimatedSprite>(count, prefab, transform.position);
	    foreach (var tk2DAnimatedSprite in _recycleQueue)
	    {
            tk2DAnimatedSprite.renderer.enabled = false;	       
            tk2DAnimatedSprite.animationCompleteDelegate = OnAnimationCompleted;
	    }
	}

    private void OnAnimationCompleted(tk2dAnimatedSprite sprite, int clipid)
    {
        sprite.renderer.enabled = false;
    }

    // Update is called once per frame
	void Update () {
	
	}
}
