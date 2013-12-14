using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[Serializable]
public class ProjectileOffset
{
    public ProjectileColor color;
    public float offset;
    public float zOffset;
}

public class EnemyManager : GameSceneObject
{
    public ProjectileInfo[] Projectiles;
    public int ProjectileQuantity;
    public float launchDelaySeconds = 1f;
    public float positionUpdateDelaySeconds = .5f;
    public ForceMode ForceMode;
    public float ProjectileOffset = 10;
    public List<ProjectileOffset> offsets = new List<ProjectileOffset>();

    private Dictionary<ProjectileColor, Tuple<ProjectileInfo, RecycleQueue<ProjectileBase>>> _projectileQueue; 
    private Queue<ProjectileColor> _projectileColors;
    private Transform _transform;
    private int _currentRail;
    private bool _waveStarted;
    private HashSet<ProjectileColor> fired;
    private ProjectileFirstFiredMessage delayMessage;

    protected override void Start ()
	{
        fired = new HashSet<ProjectileColor>();
        _transform = transform;
        PlaceManagerAtEdge();
        Setup();

        StartCoroutine(LaunchProjectile());
        
        base.Start();
	}

    private void PlaceManagerAtEdge()
    {
        if (UIHelper.MaxX != (_transform.position.x + ProjectileOffset))
        {
            _transform.position = new Vector3(UIHelper.MaxX + ProjectileOffset, _transform.position.y);
        }
    }

    private void Setup()
    {
        if (_projectileQueue == null)
        {
            _projectileQueue = new Dictionary<ProjectileColor, Tuple<ProjectileInfo, RecycleQueue<ProjectileBase>>>();
            foreach (var projectileInfo in Projectiles)
            {
                var queue = new RecycleQueue<ProjectileBase>(ProjectileQuantity, projectileInfo.Projectile,
                                                            (_transform ?? transform).position);
                _projectileQueue.Add(projectileInfo.Projectile.ProjectileColor,
                                     new Tuple<ProjectileInfo, RecycleQueue<ProjectileBase>>(projectileInfo, queue));
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Messenger.Default.Register<WaveBeginMessage>(this, OnWaveBegin);
        Messenger.Default.Register<LevelStartMessage>(this, OnLevelStart);
    }

    private void OnLevelStart(LevelStartMessage obj)
    {
        _waveStarted = false;
        ResetAllProjectiles();   
    }

    private void OnWaveBegin(WaveBeginMessage obj)
    {
        LoadFireQueue(obj.WaveInfo);
        StartWave();
    }

    private void LoadFireQueue(Wave waveInfo)
    {
        ResetAllProjectiles();
        positionUpdateDelaySeconds = waveInfo.PositionUpdateDelaySeconds;
        launchDelaySeconds = waveInfo.LaunchDelaySeconds;
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

    private IEnumerator LaunchProjectile()
    {
        while (true)
        {
            if (_waveStarted)
            {
                if (_projectileColors.Count > 0)
                {
                    UpdatePosition();

                    FireProjectile();
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

    private void FireProjectile()
    {
        var color = _projectileColors.Dequeue();
        var info = _projectileQueue[color];
        var projectileToFire = info.Item2.Next();
        var cInfo = offsets.First(c=>c.color == color);
        projectileToFire.transform.position = new Vector3(_transform.position.x, _transform.position.y + cInfo.offset,
                                                          cInfo.zOffset);
        projectileToFire.Launch(info.Item1.speed, ForceMode);
        projectileToFire.CurrentRail = _currentRail;
        if (!fired.Contains(color))
        {
            Debug.Log("Sending First Fired " + color);
            delayMessage = new ProjectileFirstFiredMessage(color);
            Invoke("SendDelayedMessage", 1);
            fired.Add(color);
        }
    }

    private void UpdatePosition()
    {
        var laneCount = GameManager.GetLanes();
        var changeRail = Random.Range(0, laneCount);
        while (changeRail == _currentRail)
        {
            changeRail = Random.Range(0, laneCount);
        }
        _currentRail = changeRail;
        var newPosition = new Vector3(_transform.position.x, GameManager.GetRails()[_currentRail].Center, 0);
        _transform.position = newPosition;
    }

    private void SendDelayedMessage()
    {
        Messenger.Default.Send(delayMessage);
        Debug.Log("Sent First Fired");
    }

    private void ResetAllProjectiles()
    {
        if (_projectileQueue == null) return;

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

public class ProjectileFirstFiredMessage
{
    public ProjectileColor Color;

    public ProjectileFirstFiredMessage(ProjectileColor color)
    {
        Color = color;
    }
}