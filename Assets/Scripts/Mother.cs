using System.Linq;
using UnityEngine;
using System.Collections;

public class Mother : MonoBehaviour
{
    public tk2dSprite[] DamageDecals;
    public string spriteNameTemplate;
    public int spriteStart;
    public int spriteEnd;
    private bool _isStarted;
    private Transform _transform;
    private float _motherWidth;

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

                DamageDecals[index].gameObject.renderer.enabled = true;
            }
        }
    }

    void Start ()
    {
        if (_isStarted) return;
        _motherWidth = GetComponent<tk2dSprite>().GetBounds().center.x *2;
        _transform = transform;
        PlaceMotherAtEdge();
        DamageDecals = gameObject.GetComponentsInChildren<tk2dSprite>()
            .Where(t => t.name.Contains("-"))
            .OrderBy(t => t.name)
            .ToArray();
        _isStarted = true;
	}
	
	void Update ()
	{
	    PlaceMotherAtEdge();
	}

    private void PlaceMotherAtEdge()
    {
        if (UIHelper.MinX != (_transform.position.x + _motherWidth))
        {
            _transform.position = new Vector3(UIHelper.MinX - _motherWidth, _transform.position.y);
        }
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
            if (previousDamage >= 1)
            {
                var decal = DamageDecals[projectile.CurrentRail];
                //decal.spriteId = decal.GetSpriteIdByName(string.Format(spriteNameTemplate, Random.Range(spriteStart, spriteEnd + 1)));
                decal.gameObject.renderer.enabled = false;
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