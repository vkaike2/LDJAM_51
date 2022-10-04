using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofRobot : Mob
{
    private List<Action> _atkPatterns = new List<Action>();
    private int _currentPattern = 0;


    [SerializeField]
    private bool _usingSpecificPattern = false;
    [SerializeField]
    private int _useThisPattern = 0;
    [SerializeField]
    private bool _useInvertedPattern = false;

    private void Awake()
    {
        base.InstatiateAttributes();

        //First Pattern
        _atkPatterns.Add(() =>
        {
            if (_useInvertedPattern)
            {
                ShootSecondPattern();
            }
            else
            {
                ShootFirstPattern();
            }

        });
        //Second Pattern
        _atkPatterns.Add(() =>
        {
            if (_useInvertedPattern)
            {
                ShootFirstPattern();
            }
            else
            {
                ShootSecondPattern();
            }

        });
    }


    public override void StartEveryAction()
    {
        if (_cdwWaitBeforeShoot != 0)
        {
            StartCoroutine(AddDelayBeforeStart());
        }
        else
        {
            StartCoroutine(StartAtkPattern());
        }
    }

    public override void StopEveryAciton()
    {
        StopAllCoroutines();

        _shouldStop = true;
        foreach (var projectile in _everyProjectile)
        {
            if (projectile == null)
            {
                continue;
            }

            projectile.StartKillingAnimation();
        }

    }

    public void ShootFirstPattern()
    {
        _animator.ResetTrigger(ATK);
        _animator.SetTrigger(ATK);

        ShootProjectile(new Vector2(0, -_projectileSpeed));
        ShootProjectile(new Vector2(_projectileSpeed, -_projectileSpeed));
        ShootProjectile(new Vector2(-_projectileSpeed, -_projectileSpeed));
    }

    public void ShootSecondPattern()
    {
        _animator.ResetTrigger(ATK);
        _animator.SetTrigger(ATK);

        ShootProjectile(new Vector2(_projectileSpeed * 1.5f, -_projectileSpeed));
        ShootProjectile(new Vector2(-_projectileSpeed * 1.5f, -_projectileSpeed));
        ShootProjectile(new Vector2(_projectileSpeed * 0.5f, -_projectileSpeed));
        ShootProjectile(new Vector2(-_projectileSpeed * 0.5f, -_projectileSpeed));
    }

    private void ShootProjectile(Vector2 forceDirection)
    {
        GameObject projectile = Instantiate(_projectilePrefab, _projectileStartPoint);
        Rigidbody2D rigidbody2D = projectile.GetComponent<Rigidbody2D>();

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
        StartCoroutine(StartAtkPattern());
    }

    IEnumerator StartAtkPattern()
    {
        if (_shouldStartShooting && !_shouldStop)
        {
            if (_usingSpecificPattern)
            {
                print(gameObject.name);
                _atkPatterns[_useThisPattern]();
            }
            else
            {
                if (_currentPattern == _atkPatterns.Count)
                {
                    _currentPattern = 0;
                }
                _atkPatterns[_currentPattern]();
                _currentPattern++;
            }
        }

        yield return new WaitForSeconds(_cdwPerShoot);

        if (!_shouldStartShooting && !_shouldStop)
        {
            if (_usingSpecificPattern)
            {
                print(gameObject.name);
                _atkPatterns[_useThisPattern]();
            }
            else
            {
                if (_currentPattern == _atkPatterns.Count)
                {
                    _currentPattern = 0;
                }
                _atkPatterns[_currentPattern]();
                _currentPattern++;
            }
        }

        StartCoroutine(StartAtkPattern());
    }

}
