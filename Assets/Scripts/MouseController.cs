using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class MouseSensitivity
{
    public float sensitivityX = 1;
    public float sensitivityY = 1;
}

public class ControllerBase : GameSceneObject
{
    public float minXOffset;
    public float maxXOffset;
    public float minYOffset;
    public float maxYOffset;
    protected float maxY;
    protected float minY;
    protected Transform myTransform;
    protected float maxX;
    protected float minX;

    protected override void Awake()
    {
        base.Awake();
        Messenger.Default.Register<LevelStartMessage>(this, OnLevelStart);
        Messenger.Default.Register<LevelEndMessage>(this, OnLevelEnd);
        Messenger.Default.Register<GameOverMessage>(this, OnGameOver);
        Messenger.Default.Register<GameWonMessage>(this, OnGameWon);
    }

    private void OnGameWon(GameWonMessage obj)
    {
        enabled = false;
    }

    private void OnGameOver(GameOverMessage obj)
    {
        enabled = false;
    }

    private void OnLevelEnd(LevelEndMessage obj)
    {
        enabled = false;
    }

    private void OnLevelStart(LevelStartMessage obj)
    {
        enabled = true;
    }

    protected override void Start ()
    {
        myTransform = transform;
        maxY = UIHelper.MaxY - maxYOffset;
        minY = -maxY + minYOffset;
        maxX = UIHelper.MaxX - maxXOffset;
        minX = -maxX + minXOffset;
        base.Start();
    }
}

public class MouseController : ControllerBase
{
    public MouseSensitivity Sensitivity;

    void Update ()
    {
        if (Input.touchCount > 0) return;
        float moveDownY = 0.0f;
	    var y = Input.GetAxis("Mouse Y");
	    moveDownY += y*Sensitivity.sensitivityY;
	    if (y != 0.0f)
	    {
            if (myTransform.position.y > maxY)
            {
                myTransform.position = new Vector3(myTransform.position.x, maxY, myTransform.position.z);
            }
            if (myTransform.position.y < minY)
            {
                myTransform.position = new Vector3(myTransform.position.x, minY, myTransform.position.z);
            }
            myTransform.Translate(Vector3.up * moveDownY);
	    }

        float moveDownX = 0.0f;
        var x = Input.GetAxis("Mouse X");
        moveDownX += x * Sensitivity.sensitivityX;
	    if (x != 0.0f)
	    {
            if (myTransform.position.x > maxX)
            {
                myTransform.position = new Vector3(maxX, myTransform.position.y, myTransform.position.z);
            }
            if (myTransform.position.x < minX)
            {
                myTransform.position = new Vector3(minX, myTransform.position.y, myTransform.position.z);
            }
            myTransform.Translate(Vector3.right * moveDownX);
	    }
	}
}
