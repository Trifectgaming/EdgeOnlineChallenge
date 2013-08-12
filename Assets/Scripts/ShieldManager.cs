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
    public GUITexture SpinLeft;
    public GUITexture SpinRight;

    private Shield currentShield;
    private int currentIndex;  
    private Transform _transform;
    private Shield previousShield;
    public int LastFinger = -1;
    
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
                Debug.Log("Left Hit Test: " + SpinLeft.HitTest(Input.mousePosition));
                Debug.Log("Right Hit Test: " + SpinRight.HitTest(Input.mousePosition));
                if (SpinLeft && SpinLeft.HitTest(Input.mousePosition))
                {
                    SetCurrent(GetNextShield());
                    RestoreAlpha(SpinLeft);
                }
                else if (SpinRight && SpinRight.HitTest(Input.mousePosition))
                {
                    SetCurrent(GetPreviousShield());
                    RestoreAlpha(SpinRight);
                }
                else
                {
                    SetCurrent(GetNextShield());
                }
                StartCoroutine(UnphaseButtons());
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
                if (touch.fingerId == LastFinger)
	            {
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        LastFinger = -1;
                    Debug.Log("Finger " + touch.fingerId + " was last finger.");
	                continue;
	            }
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    continue;
	            if (Touchpad.Touchpads.Any(t => t.HitTest(touch.position)))
	                continue;
	            if (SpinLeft && SpinLeft.HitTest(touch.position))
	            {
	                SetCurrent(GetNextShield());
                    RestoreAlpha(SpinLeft);
	            }
                if (SpinRight && SpinRight.HitTest(touch.position))
                {
                    SetCurrent(GetPreviousShield());
                    RestoreAlpha(SpinRight);
                }
                LastFinger = touch.fingerId;
	        }
            StartCoroutine(UnphaseButtons());
	    }
        _transform.rotation = Quaternion.Slerp(_transform.rotation, finalRotation, Time.deltaTime * rotationRate);
	}

    IEnumerator UnphaseButtons()
    {
        yield return new WaitForSeconds(1);
        ReduceAlpha(SpinLeft, .2f);
        ReduceAlpha(SpinRight, .2f);
    }

    private void RestoreAlpha(GUITexture texture)
    {
        texture.color = new Color(texture.color.r, texture.color.g, texture.color.b, 1);
    }

    private void ReduceAlpha(GUITexture texture, float a)
    {
        texture.color = new Color(texture.color.r, texture.color.g, texture.color.b, a);
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