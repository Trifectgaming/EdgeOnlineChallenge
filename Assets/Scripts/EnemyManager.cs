using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class EnemyManager : GameSceneObject
{
    public ProjectileInfo[] Projectiles;
    public int ProjectileQuantity;
    public float launchDelaySeconds = 1f;
    public float positionUpdateDelaySeconds = .5f;
    public ForceMode ForceMode;
    public float WaveStartDelay = 1;
    
    private Dictionary<ProjectileColor, Tuple<ProjectileInfo, ReycleQueue<ProjectileBase>>> _projectileQueue; 
    private Queue<ProjectileColor> _projectileColors;
    private Transform _transform;
    private int _currentRail;
    private bool _waveStarted;

    protected override void Start ()
	{
        _transform = transform;
        _projectileQueue = new Dictionary<ProjectileColor, Tuple<ProjectileInfo, ReycleQueue<ProjectileBase>>>();
        foreach (var projectileInfo in Projectiles)
        {
            var queue = new ReycleQueue<ProjectileBase>(ProjectileQuantity, projectileInfo.Projectile, _transform.position);
            _projectileQueue.Add(projectileInfo.Projectile.ProjectileColor, 
                new Tuple<ProjectileInfo, ReycleQueue<ProjectileBase>>(projectileInfo, queue));
        }
        
        StartCoroutine(UpdatePosition());
        StartCoroutine(LaunchProjectile());
        
        base.Start();
	}

    private void Update()
    {
        
    }

    protected override void Awake()
    {
        base.Awake();
        Messenger.Default.Register<WaveBeginMessage>(this, OnWaveBegin);
    }

    private void OnWaveBegin(WaveBeginMessage obj)
    {
        LoadFireQueue(obj.WaveInfo);
        Invoke("StartWave", WaveStartDelay);
    }

    private void LoadFireQueue(Wave waveInfo)
    {
        ResetAllProjectiles();
        var colorChance = new List<ProjectileColor>();
        foreach (var projectile in waveInfo.Projectiles)
        {
            colorChance.Add(projectile.Projectile.ProjectileColor);
            for (int i = 0; i < projectile.bias; i++)
            {
                colorChance.Add(projectile.Projectile.ProjectileColor);                
            }
        }
        _projectileColors = new Queue<ProjectileColor>();
        for (int i = 0; i < waveInfo.Count; i++)
        {
            _projectileColors.Enqueue(colorChance[Random.Range(0, colorChance.Count)]);
        }
    }

    private void StartWave()
    {
        _waveStarted = true;
    }

    private void EndWave()
    {
        _waveStarted = false;
        Messenger.Default.Send(new WaveEndMessage());
    }

    private IEnumerator UpdatePosition()
    {
        while (true)
        {
            if (_waveStarted)
            {
                var laneCount = GameManager.GetLanes();
                _currentRail = Random.Range(0, laneCount);
                var newPosition = new Vector3(_transform.position.x, GameManager.GetRails()[_currentRail].Center ,0);
                _transform.position = newPosition;
            }
            yield return new WaitForSeconds(positionUpdateDelaySeconds);
        }
    }

    private IEnumerator LaunchProjectile()
    {
        while (true)
        {
            if (_waveStarted)
            {
                if (_projectileColors.Count > 0)
                {
                    var color = _projectileColors.Dequeue();
                    var info = _projectileQueue[color];
                    var projectileToFire = info.Item2.Next();
                    projectileToFire.CurrentRail = _currentRail;
                    projectileToFire.transform.position = _transform.position;
                    projectileToFire.Launch(info.Item1.speed, ForceMode);
                }
                else
                {
                    if (CheckIfAllProjectilesHaveImpacted())
                    {
                        EndWave();
                    }
                }
            }
            yield return new WaitForSeconds(launchDelaySeconds);
        }
    }

    private void ResetAllProjectiles()
    {
        foreach (var tuple in _projectileQueue)
        {
            foreach (var projectileBase in tuple.Value.Item2)
            {
                projectileBase.Reset();
            }
        }
    }

    private bool CheckIfAllProjectilesHaveImpacted()
    {
        return _projectileQueue.All(tuple => !tuple.Value.Item2.Any(projectile => projectile.enabled));
    }
}