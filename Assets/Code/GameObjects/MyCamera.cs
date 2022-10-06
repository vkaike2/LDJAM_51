using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    private Animator _animator;

    private const string STAGE = "Stage";


    private List<StageManager> _stagesManager = new List<StageManager>();

    private Transform _playerTransform;
    private Vector2 _offset;

    private const string ANIMATION_STAGE_MOVING = "Stage{0} Moving";
    private const string ANIMATION_STAGE_IDLE = "Stage{0} Idle";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _stagesManager = GameObject.FindObjectsOfType<StageManager>().ToList();
        if (Configuration.DidYouDied)
        {
            RessurectAt(Configuration.StartOnStage);
        }
    }

    private void Update()
    {
        if (!Configuration.DidYouDied && !Configuration.GameHasStarted)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                ForceStage(0);
                Configuration.GameHasStarted = true;
            }
        }
    }

    private void FixedUpdate()
    {
        ManageAttachmentToPlayer();
    }

    public void AttachToPlayer(Transform transform)
    {
        _animator.enabled = false;
        _playerTransform = transform;
        _offset = transform.localPosition - this.transform.position;
    }

    public void DetachToPlayer()
    {
        _playerTransform = null;
        _animator.enabled = true;
        _animator.Play("Stage1 Moving");
    }

    private void ManageAttachmentToPlayer()
    {
        if (_playerTransform == null)
        {
            return;
        }
        this.transform.localPosition = new Vector3(_playerTransform.transform.position.x - _offset.x, _playerTransform.transform.position.y - _offset.y, this.transform.position.z);
    }


    public void RessurectAt(int stageNumber)
    {
        _animator.Play(string.Format(ANIMATION_STAGE_IDLE, stageNumber));
        StageManager currentStage = _stagesManager.Where(e => e.StageNumber == stageNumber).FirstOrDefault();
        currentStage.SpawnnDoorIfYouDied();
    }

    public void ForceStage(int stageNumber)
    {
        _animator.Play($"Stage{stageNumber} Moving");
    }

    public void ChangeStage(int stage)
    {
        _animator.SetInteger(STAGE, stage);
    }

    public void StartStage(int stage)
    {
        StageManager currentStage = _stagesManager.Where(e => e.StageNumber == stage).FirstOrDefault();
        currentStage.SpawnDoor();
    }

    public class Stage
    {
        [SerializeField]
        private Vector3 _cameraPosition;

        public Vector3 CameraPosition => _cameraPosition;
    }
}
