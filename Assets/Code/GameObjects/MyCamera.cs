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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //Cursor.visible = false;
        if (Configuration.DidYouDied)
        {
            ForceStage(Configuration.StartOnStage);
        }
        _stagesManager = GameObject.FindObjectsOfType<StageManager>().ToList();
    }

    private void Update()
    {
        if (!Configuration.DidYouDied)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ForceStage(0);
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
    }

    private void ManageAttachmentToPlayer()
    {
        if (_playerTransform == null)
        {
            return;
        }
        this.transform.localPosition = new Vector3(_playerTransform.transform.position.x - _offset.x, _playerTransform.transform.position.y - _offset.y, this.transform.position.z);
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
