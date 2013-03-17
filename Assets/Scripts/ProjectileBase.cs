using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileBase : MonoBehaviour
{
    public ProjectileColor ProjectileColor;
    private Rigidbody _rigidbody;
    private float _speed;
    private ForceMode _mode;
    private bool _enabled;
    public Vector3 initialPosition;


    protected void Start ()
	{
	    //enabled = false;
        renderer.enabled = false;
        initialPosition = transform.position;
	}
	
	void Update () {
	    
	}

    public void Launch(float speed, ForceMode mode)
    {
        _speed = speed;
        _mode = mode;
        enabled = true;
        var r = rigidbody;
        r.velocity = Vector3.zero;
        r.angularVelocity = Vector3.zero;
        r.inertiaTensorRotation = Quaternion.identity;
        r.rotation = Quaternion.identity;
        r.inertiaTensor = Vector3.zero;
        r.isKinematic = true;
        r.isKinematic = false;
        renderer.enabled = true;
    }

    void FixedUpdate()
    {
        rigidbody.AddForce(Vector3.left * _speed, _mode);
    }

    public void Reset()
    {
        renderer.enabled = false;
        transform.position = initialPosition;
        enabled = false;
    }
}
