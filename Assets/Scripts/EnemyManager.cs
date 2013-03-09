using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public ProjectileInfo[] Projectiles;
    public int ProjectileQuantity;
    public float launchDelaySeconds = 1f;
    public float positionUpdateDelaySeconds = .5f;
    public float Resolution;
    private float _orthoSize;
    private List<ProjectileInfo> queues;
    private Transform _transform;
    private float _maxY;
    private float _minY;

    void Start ()
	{
        _transform = transform;
        _orthoSize = Camera.mainCamera.orthographicSize;
        _maxY = _orthoSize;
        _minY = -_orthoSize;
        queues = new List<ProjectileInfo>(Projectiles.Length + Projectiles.Sum(p => p.bias));
        foreach (var projectileInfo in Projectiles)
        {
            projectileInfo.Queue = new ReycleQueue<ProjectileBase>(ProjectileQuantity, projectileInfo.Projectile, _transform.position);
            for (int i = 0; i < projectileInfo.bias; i++)
            {
                queues.Add(projectileInfo);
            }
        }
        Debug.Log("Queue size: " + queues.Count);
        StartCoroutine(StartWave());
        StartCoroutine(UpdatePosition());
	}

    private IEnumerator UpdatePosition()
    {
        while (true)
        {
            var newPosition = new Vector3(_transform.position.x, Random.Range(_minY, _maxY), 0);
            _transform.position = newPosition;
            yield return new WaitForSeconds(positionUpdateDelaySeconds);
        }
    }
    
    IEnumerator StartWave()
    {
        while (true)
        {
            var seed = Random.Range(0, queues.Count);
            if (queues.Count == 0) yield break;

            Debug.Log("Getting queue " + seed);
            var info = queues[seed];
            var projectileToFire = info.Queue.Next();
            projectileToFire.transform.position = _transform.position;
            projectileToFire.Launch(info.speed, info.mode);
            yield return new WaitForSeconds(launchDelaySeconds);
        }
    }
}