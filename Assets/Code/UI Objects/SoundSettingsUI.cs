using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class SoundSettingsUI : MonoBehaviour
{
    private Animator _animator;

    int UILayer;
    private const string MUTED = "Muted";
    private const string MOUSE_OVER = "MouseOver";

    List<AudioManager> _audioManagers;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");   
    }

    public void MuteUnmute()
    {
        Configuration.IsMuted = !Configuration.IsMuted;
        _animator.SetBool(MUTED, Configuration.IsMuted);
        _audioManagers = GameObject.FindObjectsOfType<AudioManager>().ToList();

        if (Configuration.IsMuted)
        {
            foreach (var audioManager in _audioManagers)
            {
                audioManager.Mute();
            }
        }
        else
        {
            foreach (var audioManager in _audioManagers)
            {
                audioManager.Unmute();
            }
        }
        //_audioListener.enabled = !Configuration.IsMuted;
    }

    private void FixedUpdate()
    {
        if (IsPointerOverUIElement(GetEventSystemRaycastResults()))
        {
            _animator.SetBool(MOUSE_OVER, true);
        }
        else
        {
            _animator.SetBool(MOUSE_OVER, false);
        }
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
