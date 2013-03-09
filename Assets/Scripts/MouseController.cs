using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour
{
    public float sensitivityY = 1;
    public float sensitivityX = 1;
    private float maxY;
    private float minY;
    private Transform _transform;
    private float maxX;
    private float minX;

    void Start ()
	{
	    _transform = transform;
	    maxY = Camera.mainCamera.orthographicSize;
	    minY = -maxY;
        maxX = Camera.mainCamera.GetScreenWidth()/Camera.mainCamera.GetScreenHeight() * maxY;
        minX = -maxX;
	}

    void Update ()
	{
        Screen.lockCursor = true;
        if (Input.GetKeyDown("escape"))
            Screen.lockCursor = false;
        float moveDownY = 0.0f;
	    var y = Input.GetAxis("Mouse Y");
	    moveDownY += y*sensitivityY;
	    if (y != 0.0f)
	    {
            //if (_transform.position.y > maxY)
            //{
            //    _transform.position = new Vector3(_transform.position.x, maxY);
            //}
            //if (_transform.position.y < minY)
            //{
            //    _transform.position = new Vector3(_transform.position.x, minY);
            //}
            _transform.Translate(Vector3.up * moveDownY);
	    }

        float moveDownX = 0.0f;
        var x = Input.GetAxis("Mouse X");
        moveDownX += x * sensitivityX;
	    if (x != 0.0f)
	    {
            _transform.Translate(Vector3.right * moveDownX);
	    }
	}
}
