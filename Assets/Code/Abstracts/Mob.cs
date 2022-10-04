using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mob : MonoBehaviour
{
    [SerializeField]
    protected GameObject _projectilePrefab;
    [Space]
    [SerializeField]
    protected Transform _projectileStartPoint;

    [Header("configurations")]
    [SerializeField]
    protected float _projectileSpeed = 3;
    [SerializeField]
    protected float _cdwPerShoot = 1;
    [SerializeField]
    protected bool _shouldStartShooting = true;
    [SerializeField]
    protected float _cdwWaitBeforeShoot = 0f;

    protected Animator _animator;

    protected const string ATK = "Atk";
    protected List<Projectile> _everyProjectile;

    protected bool _shouldStop = false;

    protected void InstatiateAttributes()
    {
        _animator = GetComponent<Animator>();
        _everyProjectile = new List<Projectile>();
    }

    public abstract void StartEveryAction();
    public abstract void StopEveryAciton();
}
