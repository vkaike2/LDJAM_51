using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraigthEnemy : Mob
{
    [SerializeField]
    private bool _aimingRight = true;

    private AudioManager _audioManager;
    private void Awake()
    {
        base.InstatiateAttributes();
        _audioManager = GetComponent<AudioManager>();
    }

    public override void StartEveryAction()
    {
        if (_cdwWaitBeforeShoot != 0)
        {
            StartCoroutine(AddDelayBeforeStart());
        }
        else
        {
            StartCoroutine(StartShooting());
        }
    }

    public override void StopEveryAciton()
    {
        StopAllCoroutines();

        foreach (var projectile in _everyProjectile)
        {
            if (projectile == null)
            {
                continue;
            }

            projectile.StartKillingAnimation();
            _shouldStop = true;
        }
    }

    private void Shoot()
    {
        if (_shouldStop)
        {
            return;
        }
        _audioManager.PlayAudio("roof_robot_shoot");
        _animator.ResetTrigger(ATK);
        _animator.SetTrigger(ATK);

        GameObject projectile = Instantiate(_projectilePrefab, _projectileStartPoint);
        Rigidbody2D rigidbody2D = projectile.GetComponent<Rigidbody2D>();
        Vector2 forceDirection;
        if (_aimingRight)
        {
            forceDirection = new Vector2(_projectileSpeed, 0);
        }
        else
        {
            forceDirection = new Vector2(-_projectileSpeed, 0);
        }

        rigidbody2D.velocity = forceDirection;
        _everyProjectile.Add(projectile.GetComponent<Projectile>());
    }

    IEnumerator AddDelayBeforeStart()
    {
        float cdw = _cdwWaitBeforeShoot;

        while (cdw > 0)
        {
            cdw -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(StartShooting());
    }

    IEnumerator StartShooting()
    {
        if (_shouldStartShooting)
        {
            Shoot();

        }

        yield return new WaitForSecondsRealtime(_cdwPerShoot);

        _shouldStartShooting = true;
        StartCoroutine(StartShooting());
    }
}
