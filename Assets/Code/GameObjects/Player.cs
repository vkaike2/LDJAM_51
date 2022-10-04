using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Movement _movement;

    private Rigidbody2D _rigidbody;
    private Animator _animator;

    // Animator variables
    private const string HORIZONTAL_VELOCITY = "horizontalVelocity";
    private const string IS_DOWN = "isDown";
    private const string ON_THE_GROUND = "OnTheGround";
    private const string DASH = "Dash";
    private const string DIE = "Die";
    private const string WIN = "Win";

    //Inputs
    private float _horizontalInput;
    private float _verticalInput;
    private float _jumpInput;
    private float _fire3;
    private float _fire1;

    //Layers
    private const int PROJECTILE_LAYER = 6;
    private const int GROUND_LAYER = 7;

    private bool _canControllCharecter = false;
    private Door _doorToNextStage = null;
    private int _currentStage = 0;
    private MyCamera _myCamera;
    private AudioManager _audioManager;

    private bool _isDeath = false;

    private void Awake()
    {
        _audioManager = GetComponent<AudioManager>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _myCamera = Camera.main.gameObject.GetComponent<MyCamera>();
    }

    private void Start()
    {
        _movement.CanDash = true;
        _movement.ResetSpeed();
    }

    private void Update()
    {
        if (_canControllCharecter)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            _jumpInput = Input.GetAxisRaw("Jump");
            _fire3 = Input.GetAxisRaw("Fire3");
            _fire1 = Input.GetAxisRaw("Fire1");
        }
        else
        {
            _horizontalInput = 0;
            _verticalInput = 0;
            _jumpInput = 0;
            _fire3 = 0;
            _fire1 = 0;
        }
    }


    public void SoundWalk()
    {
        string clipName = $"player_step_{Random.Range(0,4)}";
        _audioManager.PlayAudio(clipName);
    }
    private void FixedUpdate()
    {
        MovePlayer();
        GetDown();
        Jump();
        Dash();
        InteractWithDoor();
    }

    public void WinGame()
    {
        _canControllCharecter = false;
        _animator.SetTrigger(WIN);
    }

    private void MovePlayer()
    {

        _animator.SetFloat(HORIZONTAL_VELOCITY, Math.Abs(_horizontalInput));

        if (_horizontalInput == 0 || !_movement.CanMove())
        {
            return;
        }

        _rigidbody.velocity = new Vector2(_horizontalInput * _movement.Speed, _rigidbody.velocity.y);
        FlipSprite();
    }

    private void GetDown()
    {
        if (_verticalInput != -1)
        {
            _animator.SetBool(IS_DOWN, false);
            _movement.IsDown = false;
            return;
        }

        float aceleration = _rigidbody.velocity.y * _movement.Deaceleration;
        if (aceleration > 0)
        {
            aceleration *= -1;
        }
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, aceleration);

        _movement.IsDown = true;
        _animator.SetBool(IS_DOWN, true);
    }

    private void Jump()
    {
        if (!_movement.IsOnTheGround)
        {
            return;
        }
        if (_jumpInput == 0)
        {
            return;
        }
        _audioManager.PlayAudio("player_jump");
        _rigidbody.AddForce(new Vector2(_rigidbody.velocity.x, _movement.JumpForce * 10));

        _movement.IsOnTheGround = false;
        _animator.SetBool(ON_THE_GROUND, false);
    }

    private void Dash()
    {
        if (!_movement.CanDash)
        {
            return;
        }

        if (_horizontalInput == 0 || _fire3 == 0)
        {
            return;
        }

        _audioManager.PlayAudio("player_dash");
        StartCoroutine(GenericCooldown(_movement.DashDuration,
            () =>
            {
                _animator.SetTrigger(DASH);
                _movement.StartDash();
            },
            () => { _movement.ResetSpeed(); }));

        StartCoroutine(GenericCooldown(_movement.DashCooldown,
            () => { _movement.CanDash = false; },
            () => { _movement.CanDash = true; print("can dash"); }));
    }

    private void InteractWithDoor()
    {
        if (_doorToNextStage == null)
        {
            return;
        }

        if (_fire1 == 1)
        {
            if (_currentStage == 0)
            {
                _myCamera.DetachToPlayer();

            }

            _doorToNextStage.GoToNextStage();
            _doorToNextStage = null;
            _animator.SetTrigger(DIE);
            _canControllCharecter = false;
        }
    }

    private void HitByProjectile(Collider2D collision)
    {
        if (collision.gameObject.layer != PROJECTILE_LAYER)
        {
            return;
        }
        //TODO: add more than one life 
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();
        if (projectile != null && projectile.IsDead)
        {
            print("dead");
            return;
        }

        _isDeath = true;
        _canControllCharecter = false;
        _rigidbody.velocity = Vector3.zero;
        GetComponent<Collider2D>().enabled = false;
        Configuration.StartOnStage = _currentStage;
        Configuration.DidYouDied = true;
        _animator.SetTrigger(DIE);
        _audioManager.PlayAudio("player_death");
        _rigidbody.isKinematic = true;
    }

    // Called by the Animator
    private void KillPlayer()
    {
        if (_isDeath)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void CheckIfYouAreCloseToTheDoor(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }
        Door door = collision.gameObject.GetComponent<Door>();
        if (door == null || door.IsFromPrevioudStage)
        {
            return;
        }
        _doorToNextStage = door;
    }

    private void CheckIfYouAreFarFromDoor(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }
        Door doorToNextStage = collision.gameObject.GetComponent<Door>();
        if (doorToNextStage == null)
        {
            return;
        }
        _doorToNextStage = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HitByProjectile(collision.collider);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckIfYouAreFarFromDoor(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckIfYouAreCloseToTheDoor(collision);
        HitByProjectile(collision);
    }

    public void CheckIfIsOnGround(Collider2D collision)
    {
        if (collision == null || collision.gameObject.layer != GROUND_LAYER)
        {
            return;
        }

        _movement.IsOnTheGround = true;
        _animator.SetBool(ON_THE_GROUND, true);
    }

    IEnumerator GenericCooldown(float cooldown, Action doBefore, Action doAfter)
    {
        doBefore();
        yield return new WaitForSeconds(cooldown);
        doAfter();
    }

    private void FlipSprite()
    {
        if (_rigidbody.velocity.x > 0)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void MoveLittleBitToThe(bool left)
    {
        StartCoroutine(MoveForFewSeconds(left, 0.2f));
    }

    IEnumerator MoveForFewSeconds(bool left, float sec)
    {
        _horizontalInput = left ? -1 : 1;
        yield return new WaitForSeconds(sec);
        _horizontalInput = 0;
        _canControllCharecter = true;
    }

    public void SetStage(int stageNumber)
    {
        _currentStage = stageNumber;

        // Tutorial 
        if (stageNumber == 0)
        {
            _myCamera.AttachToPlayer(this.transform);
        }
    }

    [Serializable]
    public class Movement
    {
        [SerializeField]
        private float _initialSpeed = 7;
        [Space]
        [SerializeField]
        private float _jumpForce = 50;
        [SerializeField]
        private float _deaceleration = 2;
        [Space]
        [SerializeField]
        private float _dashCooldown = 3;
        [SerializeField]
        private float _dashDuration = 0.2f;
        [SerializeField]
        private float _dashSpeed = 10;

        public float Speed { get; private set; }
        public float JumpForce => _jumpForce;
        public float Deaceleration => _deaceleration;
        public float DashCooldown => _dashCooldown;
        public float DashDuration => _dashDuration;

        public float CdwToApplyForceOnJump => 0.5f;
        public bool IsGoingUp { get; set; }
        public bool CanDash { get; set; }
        public bool IsDown { get; set; }
        public bool IsOnTheGround { get; set; }



        public void ResetSpeed()
        {
            Speed = _initialSpeed;
        }

        public void StartDash()
        {
            Speed = _dashSpeed;
        }



        public bool CanMove()
        {
            return !IsDown;
        }
    }
}
