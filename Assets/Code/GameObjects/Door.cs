using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Space]
    [SerializeField]
    private GameObject _playerPrefab;
    [Space]
    [SerializeField]
    private bool _isFromPreviousStage = true;

    private Animator _animator;
    private StageManager _stageManager;

    private const string SPAWN_PLAYER = "SpawnPlayer";
    private const string SPAWN_DOOR = "SpawnDoor";

    public bool IsFromPrevioudStage => _isFromPreviousStage;

    private AudioManager _audioManager;

    private Player _player;
    private void Awake()
    {
        _audioManager = GetComponent<AudioManager>();
        _stageManager = gameObject.GetComponentInParent<StageManager>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_stageManager.StageNumber == 0 && !_isFromPreviousStage)
        {
            SpawnDoor();
        }
    }

    public void StartSpawnPlayerProcess()
    {
        if(_animator == null)
        {
            return;
        }

        _animator.SetTrigger(SPAWN_PLAYER);
    }

    public void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(_playerPrefab, this.transform);
        playerGameObject.transform.parent = null;
        _stageManager.Player = playerGameObject.GetComponent<Player>();
        _stageManager.Player.MoveLittleBitToThe(false);
        _stageManager.Player.SetStage(_stageManager.StageNumber);
    }

    private void KillDoor()
    {
        _stageManager.StartStage();
        Destroy(gameObject);
    }

    public void SpawnDoor()
    {
        if (_stageManager.StageNumber == 7)
        {
            _stageManager.Player.WinGame();
            _stageManager.WinGame();
            return;
        }
        else
        {
            _stageManager.WinStage();
            _animator.SetTrigger(SPAWN_DOOR);
        }
    }

    public void GoToNextStage()
    {
        _stageManager.MoveCameraToNextStage();
    }
}
