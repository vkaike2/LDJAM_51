using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Space]
    [SerializeField]
    private int _stageNumber;
    [Space]
    [Header("Objects")]
    [SerializeField]
    private Door _initialDoor;
    [SerializeField]
    private Door _doorToNextStage;
    [SerializeField]
    List<Mob> _mobs;

    public int StageNumber => _stageNumber;

    private Animator _animator;

    private RoomUI _roomUI;
    private MyCamera _myCamera;
    public Player Player { get; set; }

    private AudioManager _audioManager;
    private AudioSource _audioSource;

    private const string WIN = "Win";

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioManager = GetComponent<AudioManager>();
        _animator = GetComponent<Animator>();
        _roomUI = GetComponent<RoomUI>();
        _mobs = GetComponentsInChildren<Mob>().ToList();
    }

    private void Start()
    {
        _myCamera = Camera.main.gameObject.GetComponent<MyCamera>();
    }

    public void SpawnDoor()
    {
        if (StageNumber == 0)
        {
            _audioManager.PlayAudio("tutorial_theme");
        }
        else
        {
            _audioManager.PlayAudio("stage_music");
        }
        _initialDoor.StartSpawnPlayerProcess();
    }

    public void StartStage()
    {
        foreach (var mob in _mobs)
        {
            if (mob == null) continue;

            mob.StartEveryAction();
        }
        _roomUI.StartProcess();
    }

    // Called by the end of the countdown
    public void FinishStage()
    {
        foreach (var robot in _mobs)
        {
            if (robot == null) continue;

            robot.StopEveryAciton();
        }
        _doorToNextStage.SpawnDoor();
    }

    public void MoveCameraToNextStage()
    {
        _myCamera.ChangeStage(StageNumber + 1);
    }

    public void WinStage()
    {
        if (StageNumber != 0 && StageNumber != 7)
        {
            _audioManager.PlayAudio("win_stage");
        }
    }

    internal void WinGame()
    {
        _audioManager.PlayLoopAudio("win_theme");
        _animator.SetTrigger(WIN);
    }
}
