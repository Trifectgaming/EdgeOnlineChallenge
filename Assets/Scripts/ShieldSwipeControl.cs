using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ShieldSwipeControl : ShieldControlBase
{
    public float swipeDelta = 100;
    Dictionary<int,Vector2> fingerPositions = new Dictionary<int, Vector2>(); 
    void Update ()
	{
	    if (Input.touches.Length <= 1) return;
        for (int index = 1; index < Input.touches.Length; index++)
	    {
	        var screenTouch = Input.touches[index];
	        switch (screenTouch.phase)
	        {
	            case TouchPhase.Began:
	                fingerPositions[screenTouch.fingerId] = screenTouch.position;
	                break;
	            case TouchPhase.Ended:
	                Vector2 startPosition;
	                if (fingerPositions.TryGetValue(screenTouch.fingerId, out startPosition))
	                {
	                    fingerPositions.Remove(screenTouch.fingerId);
                        var delta = (startPosition - screenTouch.position).y;
                        Debug.Log("Finger " + screenTouch.fingerId + " Swiped for " + delta);
	                    if (Mathf.Abs(delta) >= swipeDelta)
	                    {
	                        if (delta > 1)
	                        {
                                SpinShieldRight();
	                        }
	                        else
	                        {
                                SpinShieldLeft();	                            
	                        }
	                    }
	                }
	                break;
	            case TouchPhase.Canceled:
                    Debug.Log("Swipe Cancelled with finger " + screenTouch.fingerId);
                    fingerPositions.Remove(screenTouch.fingerId);
	                break;
	        }
	    }
	}
}
