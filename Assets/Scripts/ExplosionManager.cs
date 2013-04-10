using System;
using UnityEngine;
using System.Collections;

public class ExplosionManager : MonoBehaviour
{
    public int count;
    public tk2dAnimatedSprite prefab;
    private RecycleQueue<tk2dAnimatedSprite> _recycleQueue;

    void Awake()
    {
        Messenger.Default.Register<ShieldImpactMessage>(this, OnShieldImpact);
        Messenger.Default.Register<MotherImpactMessage>(this, OnMotherImpact);
    }

    private void OnMotherImpact(MotherImpactMessage obj)
    {
        
            var explosion = _recycleQueue.Next();
            explosion.renderer.enabled = true;
            explosion.transform.position = obj.ImpactPosition;
            explosion.Play();
    }

    private void OnShieldImpact(ShieldImpactMessage obj)
    {
        if (!obj.WasDeflected)
        {
            var explosion = _recycleQueue.Next();
            explosion.renderer.enabled = true;
            explosion.transform.position = obj.ImpactPosition;
            explosion.Play();
        }
    }

    // Use this for initialization
	void Start () {
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
