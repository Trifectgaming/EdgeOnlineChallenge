using System.Linq;
using UnityEngine;
using System.Collections;

public class TapController : ControllerBase
{
    public ShieldTapControl ShieldTapControl;
    public Vector2 position;
    public float positionChange;
    public float journeyLength;
    public float speed = 1;
    public Vector2 offset = new Vector2(5,0);
    public bool rememberOffset;
    
	void Update ()
	{
	    if (Input.touches.Length == 0) return;
	    foreach (var screenTouch in Input.touches)
	    {
            if (ShieldTapControl.HitTest(screenTouch.position)) continue;
            if (screenTouch.phase == TouchPhase.Canceled || screenTouch.phase == TouchPhase.Ended) continue;
	        
            var pos = (Vector2)Camera.main.ScreenToWorldPoint(screenTouch.position);
            if (rememberOffset && screenTouch.phase == TouchPhase.Began)
            {
                offset = new Vector2(transform.position.x - pos.x, 0);
            }
            var newPosition = pos + offset;
            //if (newPosition != position)
            //{
            //    positionChange = Time.time;
            //    journeyLength = Vector2.Distance(transform.position, newPosition);
            //}
            position = newPosition;

            //var distCovered = (Time.time - positionChange) * speed;
	        //var fracJourney = distCovered/journeyLength;
            myTransform.position = position;

            var y = position.y;
	        //float moveDownY = y;
	        if (y != 0.0f)
	        {
	            if (myTransform.position.y > maxY)
	            {
	                myTransform.position = new Vector3(myTransform.position.x, maxY);
	            }
	            if (myTransform.position.y < minY)
	            {
	                myTransform.position = new Vector3(myTransform.position.x, minY);
	            }
	        }

	        var x = position.x;
	        //float moveDownX = x;
	        if (x != 0.0f)
	        {
	            if (myTransform.position.x > maxX)
	            {
	                myTransform.position = new Vector3(maxX, myTransform.position.y);
	            }
	            if (myTransform.position.x < minX)
	            {
	                myTransform.position = new Vector3(minX, myTransform.position.y);
	            }
	        }
            break;
	    }
	}
}
