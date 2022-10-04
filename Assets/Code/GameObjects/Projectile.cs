using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private const string DESTROY = "Destroy";
    public bool IsDead { get; set; }

    private const int GROUND_LAYER = 7;

    private AudioManager _audioManager;
    private AudioSource _audioSource;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioManager = GetComponent<AudioManager>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        IsDead = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }

        if (collision.gameObject.layer == GROUND_LAYER)
        {
            IsDead = true;
        }
        if (!_audioSource.isPlaying)
        {
            _audioManager.PlayAudio("projectile_explod");
        }



        StartKillingAnimation();
    }

    public void StartKillingAnimation()
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.isKinematic = true;
        _collider.enabled = false;

        _animator.SetTrigger(DESTROY);
    }


    private void KillObject()
    {
        Destroy(this.gameObject);
    }
}
