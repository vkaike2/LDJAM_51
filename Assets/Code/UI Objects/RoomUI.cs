using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RoomUI : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField]
    private float _cdwPerNumber = 2;

    [Header("[UI] countdown")]
    [SerializeField]
    private TMP_Text _textCountDown;

    private Animator _textCountDownAnimator;
    [Space]
    [SerializeField]
    private UnityEvent _onEndCdw;


    // Animator variables
    private const string PULSE = "Pulse";

    private void Awake()
    {
        _textCountDownAnimator = _textCountDown.gameObject.GetComponent<Animator>();
    }

    public void StartProcess()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        float cdw = 10;
        SetText(cdw.ToString());
        while (cdw > 0)
        {
            yield return new WaitForSeconds(_cdwPerNumber);
            cdw -= 1;
            SetText(cdw.ToString());
        }

        yield return new WaitForSeconds(0.5f);
        if (_onEndCdw != null)
        {
            _onEndCdw.Invoke();
        }
    }

    private void SetText(string value)
    {
        _textCountDownAnimator.ResetTrigger(PULSE);
        _textCountDownAnimator.SetTrigger(PULSE);
        _textCountDown.SetText(value.PadLeft(2, '0'));
    }


}
