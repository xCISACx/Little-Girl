using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseClickAudio : MonoBehaviour, IPointerClickHandler
{

    public enum EventType
    {
        PointerClick

    }

    [Tooltip("Type of event to listen to.")]
    public EventType Type = EventType.PointerClick;

    private FMODLittleGirl fmod;

    [FMODUnity.EventRef]
    public string EventName;

    void Awake()
    {
        fmod = FindObjectOfType<FMODLittleGirl>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Type == EventType.PointerClick)
        {
            fmod.PlayOneShotAudio(EventName);
        }
    }
}
