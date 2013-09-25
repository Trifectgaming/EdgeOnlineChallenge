using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class ShieldManager : GameSceneObject
{
    public float rotationRate = 1;
    public float shieldCycleDelay = .2f;
    public Vector3 activeShield;
    public Vector3 deactiveShield;
    public Quaternion finalRotation;
    public Shield[] shields;
    public float changeRate;
    
    private Shield currentShield;
    private int currentIndex;  
    private Transform _transform;
    private Shield previousShield;
    
    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        Messenger.Default.Register<LevelStartMessage>(this, OnLevelStart);
    }

    private void OnLevelStart(LevelStartMessage obj)
    {
        StartCoroutine(CycleShields());
    }

    private IEnumerator CycleShields()
    {
        for (int i = 0; i < shields.Length; i++)
        {
            var shield = GetNextShield();
            shield.Reset();
            SetCurrent(shield);
            yield return new WaitForSeconds(shieldCycleDelay);
        }
    }

    protected override void Start ()
	{
	    _transform = transform;
        base.Start();
	}

    public void SetCurrent(Shield shield)
    {
        previousShield = currentShield;
        currentShield = shield;
        if (previousShield != null) previousShield.Shrink();
        currentShield.Grow();
        float angle;
        Vector3 axis;
        currentShield.transform.localRotation.ToAngleAxis(out angle, out axis);
        finalRotation = Quaternion.AngleAxis(angle * 2, Vector3.forward);
    }

    // Update is called once per frame
	void Update () {
	    _transform.rotation = Quaternion.Slerp(_transform.rotation, finalRotation, Time.deltaTime * rotationRate);
	}

    public Shield GetPreviousShield()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = shields.Length - 1;
        }
        return shields[currentIndex];
    }

    public Shield GetNextShield()
    {
        currentIndex++;
        if (currentIndex > shields.Length - 1)
        {
            currentIndex = 0;
        }
        return shields[currentIndex];
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}

public enum ShieldColor
{
    Red,
    Green,
    Blue,
}


public enum ProjectileColor
{
    Red,
    Green,
    Blue,
}