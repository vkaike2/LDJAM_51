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

    List<AudioManager> _audioManagers = new List<AudioManager>();

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");
    }

    public void AddAudioManager(AudioManager manager)
    {
        _audioManagers.Add(manager);
    }

    public void MuteUnmute()
    {
        Configuration.IsMuted = !Configuration.IsMuted;
        _animator.SetBool(MUTED, Configuration.IsMuted);

        if (Configuration.IsMuted)
        {
            foreach (var audioManager in _audioManagers)
            {
                if (audioManager != null)
                    audioManager.Mute();
            }
        }
        else
        {
            foreach (var audioManager in _audioManagers)
            {
                if (audioManager != null)
                    audioManager.Unmute();
            }
        }
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

    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


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


    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
