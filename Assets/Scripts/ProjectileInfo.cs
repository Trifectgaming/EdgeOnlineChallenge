using System;
using UnityEngine;

[Serializable]
public class ProjectileInfo
{
    public ProjectileBase Projectile;
    public int bias = 1;
    public float speed = 1;
    public ForceMode mode = ForceMode.Acceleration;
    public ReycleQueue<ProjectileBase> Queue;
}