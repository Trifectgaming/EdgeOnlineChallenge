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
    private bool _isStarted;

    void Awake()
    {
        Messenger.Default.Register<LevelStartMessage>(this, OnLevelStart);
    }

    private void OnLevelStart(LevelStartMessage obj)
    {
        if (!_isStarted) return;
        ResetRailDamage();
    }

    private void ResetRailDamage()
    {
        if (GameManager.GetRails() != null)
        {
            for (int index = 0; index < GameManager.GetRails().Length; index++)
            {
                var rail = GameManager.GetRails()[index];
                rail.DamageTaken = 0;

                DamageDecals[index].gameObject.renderer.enabled = false;

                DamageAnims[index].Stop();
                DamageAnims[index].gameObject.renderer.enabled = false;
            }
        }
    }

    void Start ()
    {
        if (_isStarted) return;
        DamageDecals = gameObject.GetComponentsInChildren<tk2dSprite>()
            .OrderBy(t=>t.name)
            .ToArray();
        DamageAnims = gameObject.GetComponentsInChildren<tk2dAnimatedSprite>()
            .OrderBy(t => t.name)
            .ToArray();
        _isStarted = true;
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
                DamageAnims[projectile.CurrentRail].Play();
                DamageAnims[projectile.CurrentRail].gameObject.renderer.enabled = true;
            }
            if (rail.DamageTaken >= rail.AllowedDamage)
            {
                Messenger.Default.Send(new GameOverMessage());
            }
            Messenger.Default.Send(new MotherImpactMessage
                                       {
                                           ImpactPosition = collision.contacts.First().point
                                       });
            audio.Play();
        }
    }
}