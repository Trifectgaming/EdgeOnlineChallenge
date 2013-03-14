using UnityEngine;
using System.Collections;

public class Mother : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectiles")
        {
            var projectile = collision.gameObject.GetComponent<ProjectileBase>();
            projectile.Reset();
            Messenger.Default.Send(new MotherImpactMessage());
        }
    }
}

public class MotherImpactMessage
{
}
