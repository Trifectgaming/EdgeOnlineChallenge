using System.Linq;
using Holoville.HOTween;
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

    void OnDestroy()
    {
        Messenger.Default.Unregister(this);
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
                var decal = DamageDecals[index];
                RemoveTween(decal);
                decal.gameObject.renderer.enabled = true;
            }
        }
    }

    private static void RemoveTween(tk2dSprite decal)
    {
        HOTween.GetTweenersByTarget(decal, true).ForEach(t=>t.Rewind());
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
        foreach (var decal in DamageDecals)
        {
            var tween = HOTween.To(decal, .5f, "color", Color.red);
            if (tween == null)
            {
                Debug.LogError("Crap screwed up the tween!");
            }
            else
            {
                tween.loops = -1;
                tween.loopType = LoopType.Yoyo;
                tween.easeType = EaseType.Linear;
                tween.Pause();
            }
        }
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
            rail.DamageTaken += projectile.Damage;
            var decal = DamageDecals[projectile.CurrentRail];
            if (rail.DamageTaken == 1)
            {
                HOTween.GetTweenersByTarget(decal, true).ForEach(t => t.Play());                
            }
            if (rail.DamageTaken == 2)
            {
                RemoveTween(decal);
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