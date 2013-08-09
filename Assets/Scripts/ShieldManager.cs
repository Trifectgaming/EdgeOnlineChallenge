using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ShieldManager : GameSceneObject
{
    public float rotationRate = 1;
    public float shieldCycleDelay = .2f;
    public Vector3 activeShield;
    public Vector3 deactiveShield;
    private Shield currentShield;
    private int currentIndex;
    public Quaternion finalRotation;
    public Shield[] shields;
    private Transform _transform;
    private Shield previousShield;
    public float changeRate;
    private decimal _lastFinger;
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
            SetCurrent(shield);
            yield return new WaitForSeconds(shieldCycleDelay);
        }
    }

    protected override void Start ()
	{
	    _transform = transform;
        base.Start();
	}

    private void SetCurrent(Shield shield)
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
	    if (Input.touchCount == 0)
	    {
	        if (Input.GetMouseButtonDown(0))
	        {
	            SetCurrent(GetNextShield());
	        }
	        else if (Input.GetMouseButtonDown(1))
	        {
	            SetCurrent(GetPreviousShield());
	        }
	    }
	    else
	    {
	        var count = Input.touchCount;
	        for (int i = 0; i < count; i++)
	        {
	            var touch = Input.touches[i];
	            if (touch.fingerId == _lastFinger)
	            {
	                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
	                    _lastFinger = -1;
	                continue;
	            }
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    continue;
	            foreach (var touchpad in Touchpad.Touchpads)
	            {
	                if (touchpad.IsLatched(touch.fingerId))
                        continue;
	                SetCurrent(GetNextShield());
	                _lastFinger = touch.fingerId;
	            }
	        }
	    }
        _transform.rotation = Quaternion.Slerp(_transform.rotation, finalRotation, Time.deltaTime * rotationRate);
	}

    private Shield GetPreviousShield()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = shields.Length - 1;
        }
        return shields[currentIndex];
    }

    private Shield GetNextShield()
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