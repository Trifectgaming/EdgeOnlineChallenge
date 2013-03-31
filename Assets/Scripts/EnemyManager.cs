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
    
    private List<Tuple<ProjectileInfo, ReycleQueue<ProjectileBase>>> _queues;
    private Transform _transform;
    private int _currentRail;

    protected override void Start ()
	{
        _transform = transform;

        _queues = new List<Tuple<ProjectileInfo, ReycleQueue<ProjectileBase>>>(Projectiles.Length + Projectiles.Sum(p => p.bias));
        foreach (var projectileInfo in Projectiles)
        {
            var queue = new ReycleQueue<ProjectileBase>(ProjectileQuantity, projectileInfo.Projectile, _transform.position);
            for (int i = 0; i < projectileInfo.bias; i++)
            {
                _queues.Add(new Tuple<ProjectileInfo, ReycleQueue<ProjectileBase>>(projectileInfo, queue));
            }
        }

        StartCoroutine(UpdatePosition());
        StartCoroutine(StartWave());
        
        base.Start();
	}

    private IEnumerator UpdatePosition()
    {
        while (true)
        {
            if (enabled)
            {
                var laneCount = GameManager.GetLanes();
                _currentRail = Random.Range(0, laneCount);
                var newPosition = new Vector3(_transform.position.x, GameManager.GetRails()[_currentRail].Center ,0);
                _transform.position = newPosition;
            }
            yield return new WaitForSeconds(positionUpdateDelaySeconds);
        }
    }
    
    IEnumerator StartWave()
    {
        while (true)
        {
            if (enabled)
            {
                var seed = Random.Range(0, _queues.Count);
                if (_queues.Count == 0) yield break;

                var info = _queues[seed];
                var projectileToFire = info.Item2.Next();
                projectileToFire.CurrentRail = _currentRail;
                projectileToFire.transform.position = _transform.position;
                projectileToFire.Launch(info.Item1.speed, ForceMode);
            }
            yield return new WaitForSeconds(launchDelaySeconds);
        }
    }
}