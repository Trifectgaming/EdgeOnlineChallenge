using UnityEngine;

public class WingManager : MonoBehaviour
{
    public float ForwardRotation = 330;
    public float AltitudeRotation  = 60;
    public float Speed = .1f;

    public Transform[] wings;
    public Vector3 lastPosition;
    private Transform _transform;
    public float[] orgAngles;

    void Start ()
	{
	    _transform = transform;
        lastPosition = _transform.position;
	}

    private void FixedUpdate()
    {
        var xDifference = lastPosition.x - _transform.position.x;
        var yDifference = lastPosition.y - _transform.position.y;
        bool stationary;
        bool movingUp = false;
        bool MovingFoward = false;

        if (lastPosition == _transform.position)
        {
            stationary = true;
        }
        else
        {
            stationary = false;
            if (xDifference <= 0)
            {
                MovingFoward = true;
            }
            if (yDifference < 0)
            {
                movingUp = true;
            }
        }
        foreach (var wing in wings)
        {
            float s;
            
            var absy = Mathf.Abs(yDifference);
            if (absy > 0)
                s = absy * Speed;
            else
                s = Speed;

            float angle = ForwardRotation;
            if (!stationary)
            {
                if (movingUp)
                {
                    angle += AltitudeRotation;
                }
                else
                {
                    angle -= AltitudeRotation;
                }
            }
            wing.rotation = Quaternion.Slerp(wing.rotation, Quaternion.Euler(0, 0, angle), Time.fixedDeltaTime*s);
        }
        lastPosition = _transform.position;
    }
}
