using UnityEngine;
using System.Collections;

public class TouchController : ControllerBase
{
    public Touchpad moveTouchpad;
    public float speed = 4;
    public Vector3 velocity;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnGamePaused(GamePausedMessage obj)
    {
        base.OnGamePaused(obj);
    }

    protected override void OnGameResume(GameResumeMessage obj)
    {
        base.OnGameResume(obj);
    }

	// Update is called once per frame
    private void Update()
    {
        var movement = myTransform.TransformDirection(moveTouchpad.position);

        // We only want horizontal movement
        movement.z = 0;
        movement.Normalize();
        
        // Apply movement from move joystick
        var absJoyPos = new Vector2(Mathf.Abs(moveTouchpad.position.x), Mathf.Abs(moveTouchpad.position.y));
        
        if (absJoyPos.y > absJoyPos.x)
        {
            movement *= speed*absJoyPos.y;
        }
        else
        {
            movement *= speed*absJoyPos.x;
        }

        movement += velocity;
        movement *= Time.deltaTime;
        float moveDownY = 0.0f;
        var y = movement.y;
        moveDownY += y; // *Sensitivity.sensitivityY;
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
            //myTransform.Translate(Vector3.up * moveDownY);
        }

        float moveDownX = 0.0f;
        var x = movement.x;
        moveDownX += x; // *Sensitivity.sensitivityX;
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
            //myTransform.Translate(Vector3.right * moveDownX);
        }
        myTransform.position += movement;
    }
}
