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
            var rail = GameManager.GetRails()[projectile.CurrentRail];
            rail.DamageTaken += projectile.Damage;
            if (rail.DamageTaken >= rail.AllowedDamage)
            {
                Messenger.Default.Send(new GameOverMessage());
                rail.DamageTaken = 0;
            }
            Messenger.Default.Send(new MotherImpactMessage());
        }
    }
}

public class GameOverMessage
{
}

public class MotherImpactMessage
{
}
