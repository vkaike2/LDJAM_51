using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncy : Mob
{
    [SerializeField]
    private Vector2 _startDirection = new Vector2(1,-1);

    private Rigidbody2D _rigidbody2D;
    private Vector2 _lastVelocity;

    private const int GROUND_LAYER = 7;

    [SerializeField]
    private AudioSource _audioSource;

    private AudioManager _audioManager;

    private const string DIE = "Die";

    private void Awake()
    {
        _audioManager = GetComponent<AudioManager>();
        base.InstatiateAttributes();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _lastVelocity = _rigidbody2D.velocity;
    }

    public override void StartEveryAction()
    {
        //_audioManager.PlayLoopAudio("enemy_floating");
        _audioSource.Play();
        _rigidbody2D.velocity = new Vector2(_projectileSpeed * _startDirection.x, _projectileSpeed* _startDirection.y);
    }

    public override void StopEveryAciton()
    {
        _rigidbody2D.velocity = Vector2.zero;
        _animator.SetTrigger(DIE);
    }

    private void KillObject()
    {
        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeDirection(collision);
    }

    private void ChangeDirection(Collision2D collision)
    {
        if (collision == null)
        {
            return;
        }

        if (collision.gameObject.layer != GROUND_LAYER)
        {
            return;
        }

        _audioManager.PlayAudio("enemy_hiting_wall");

        float speed = _lastVelocity.magnitude;
        Vector2 direction = Vector2.Reflect(_lastVelocity.normalized, collision.contacts[0].normal);

        _rigidbody2D.velocity = direction * speed;
    }
}
