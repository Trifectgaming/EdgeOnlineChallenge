using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour
{
    public float rotationsPerMinute = .5f;


    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(6.0f * rotationsPerMinute * Time.deltaTime, transform.rotation.y, transform.rotation.z);
	}
}
