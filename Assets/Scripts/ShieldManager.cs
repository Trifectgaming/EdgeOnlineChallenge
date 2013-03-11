using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ShieldManager : MonoBehaviour
{
    public float rotationRate = 1;
    public Vector3 activeShield;
    public Vector3 deactiveShield;
    private Shield currentShield;
    private int currentIndex;
    public Quaternion finalRotation;
    public Shield[] shields;
    private Transform _transform;
    private Shield previousShield;
    public float changeRate;
    // Use this for initialization
	void Start ()
	{
	    _transform = transform;
        foreach (Shield t in shields)
        {
            t.Transform.localScale = new Vector3(0.2f, 1, 1);
        }

        SetCurrent(shields[0]);
	}

    private void SetCurrent(Shield shield)
    {
        previousShield = currentShield;
        currentShield = shield;
        //currentShield.Transform.localScale = new Vector3(0.2f, 4, 1);
        float angle;
        Vector3 axis;
        currentShield.Transform.localRotation.ToAngleAxis(out angle, out axis);
        finalRotation = Quaternion.AngleAxis(angle * 2, Vector3.forward);
    }

    // Update is called once per frame
	void Update () {
        
        if (Input.GetMouseButtonDown(0))
        {
            SetCurrent(GetNextShield());
        }
        else if (Input.GetMouseButtonDown(1))
        {
            SetCurrent(GetPreviousShield());            
        }
	    if (previousShield != null)
	    {
	        previousShield.Transform.localScale = Vector3.Lerp(previousShield.Transform.localScale, deactiveShield, Time.deltaTime * changeRate);
	    }
        currentShield.Transform.localScale = Vector3.Lerp(currentShield.Transform.localScale, activeShield, Time.deltaTime * changeRate);
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
}

public enum ShieldColor
{
    Red,
    Green,
    Blue,
}
