using System.Linq;
using UnityEngine;
using System.Collections;

public class Mother : MonoBehaviour
{
    public tk2dSprite[] DamageDecals;
    public tk2dAnimatedSprite[] DamageAnims;
    public string spriteNameTemplate;
    public int spriteStart;
    public int spriteEnd;

	// Use this for initialization
	void Start ()
	{
	    DamageDecals = gameObject.GetComponentsInChildren<tk2dSprite>()
            .OrderBy(t=>t.name)
            .ToArray();
        DamageAnims = gameObject.GetComponentsInChildren<tk2dAnimatedSprite>()
            .OrderBy(t => t.name)
            .ToArray();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectiles")
        {
            var projectile = collision.gameObject.GetComponent<ProjectileBase>();
            var impactPosition = projectile.transform.position;
            projectile.Reset();
            
            var rail = GameManager.GetRails()[projectile.CurrentRail];
            var previousDamage = rail.DamageTaken;
            rail.DamageTaken += projectile.Damage;
            if (previousDamage == 0)
            {
                var decal = DamageDecals[projectile.CurrentRail];
                decal.spriteId = decal.GetSpriteIdByName(string.Format(spriteNameTemplate, Random.Range(spriteStart, spriteEnd + 1)));
                decal.gameObject.renderer.enabled = true;
            }
            if (rail.DamageTaken >= 2)
            {
                DamageAnims[projectile.CurrentRail].gameObject.renderer.enabled = true;
            }
            if (rail.DamageTaken >= rail.AllowedDamage)
            {
                Messenger.Default.Send(new GameOverMessage());
                rail.DamageTaken = 0;
            }
            Messenger.Default.Send(new MotherImpactMessage
                                       {
                                           ImpactPosition = impactPosition
                                       });
            audio.Play();
        }
    }
}