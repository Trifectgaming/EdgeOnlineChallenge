using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class MouseSensitivity
{
    public float sensitivityX = 1;
    public float sensitivityY = 1;
}

public class MouseController : GameSceneObject
{
    public MouseSensitivity Sensitivity;
    public float minXOffset;
    public float maxXOffset;
    public float minYOffset;
    public float maxYOffset;
    private float maxY;
    private float minY;
    private Transform _transform;
    private float maxX;
    private float minX;

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
	    _transform = transform;
        maxY = UIHelper.MaxY - maxYOffset;
	    minY = -maxY + minYOffset;
        maxX = UIHelper.MaxX - maxXOffset;
        minX = -maxX + minXOffset;
        base.Start();
	}

    void Update ()
	{
        float moveDownY = 0.0f;
	    var y = Input.GetAxis("Mouse Y");
	    moveDownY += y*Sensitivity.sensitivityY;
	    if (y != 0.0f)
	    {
            if (_transform.position.y > maxY)
            {
                _transform.position = new Vector3(_transform.position.x, maxY);
            }
            if (_transform.position.y < minY)
            {
                _transform.position = new Vector3(_transform.position.x, minY);
            }
            _transform.Translate(Vector3.up * moveDownY);
	    }

        float moveDownX = 0.0f;
        var x = Input.GetAxis("Mouse X");
        moveDownX += x * Sensitivity.sensitivityX;
	    if (x != 0.0f)
	    {
            if (_transform.position.x > maxX)
            {
                _transform.position = new Vector3(maxX,_transform.position.y);
            }
            if (_transform.position.x < minX)
            {
                _transform.position = new Vector3(minX, _transform.position.y);
            }
            _transform.Translate(Vector3.right * moveDownX);
	    }
	}
}
