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
    
    private List<ProjectileInfo> _queues;
    private Transform _transform;
    private int currentRail;

    protected override void Start ()
	{
        _transform = transform;
        
        _queues = new List<ProjectileInfo>(Projectiles.Length + Projectiles.Sum(p => p.bias));
        foreach (var projectileInfo in Projectiles)
        {
            projectileInfo.Queue = new ReycleQueue<ProjectileBase>(ProjectileQuantity, projectileInfo.Projectile, _transform.position);
            for (int i = 0; i < projectileInfo.bias; i++)
            {
                _queues.Add(projectileInfo);
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
                currentRail = Random.Range(0, laneCount);
                var newPosition = new Vector3(_transform.position.x, GameManager.GetRails()[currentRail].Center ,0);
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
                var projectileToFire = info.Queue.Next();
                projectileToFire.CurrentRail = currentRail;
                projectileToFire.transform.position = _transform.position;
                projectileToFire.Launch(info.speed, info.mode);
            }
            yield return new WaitForSeconds(launchDelaySeconds);
        }
    }
}