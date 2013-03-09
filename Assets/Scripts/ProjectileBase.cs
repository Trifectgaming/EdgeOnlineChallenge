using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileBase : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _speed;
    private ForceMode _mode;
    private bool _enabled;

    void Start ()
	{
	    enabled = false;
        renderer.enabled = false;
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
        if (!enabled) return; 
        rigidbody.AddForce(Vector3.left * _speed, _mode);
    }
}
