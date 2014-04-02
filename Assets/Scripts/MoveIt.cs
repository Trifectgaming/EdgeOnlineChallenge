using UnityEngine;
using System.Collections;

public class MoveIt : MonoBehaviour
{
    public float speed;
    private Transform t;
    void Start ()
    {
        t = transform;
    }
	
	void FixedUpdate () {
	    t.Translate(speed, 0, 0);
	}

    void OnBecameInvisible()
    {
        Debug.Log(Time.time);
    }
}
