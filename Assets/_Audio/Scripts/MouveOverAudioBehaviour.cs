using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouveOverAudioBehaviour : MonoBehaviour, IPointerEnterHandler
{

    public enum EventType
    {
        PointerEnter,
        MouseOver

    }

    [Tooltip("Type of event to listen to.")]
    public EventType Type = EventType.PointerEnter;

    private FMODLittleGirl fmod;

    [FMODUnity.EventRef]
    public string EventName;

    void Awake()
    {
        fmod = FindObjectOfType<FMODLittleGirl>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Type == EventType.PointerEnter)
        {
            fmod.PlayOneShotAudio(EventName);
        }
    }

    private void OnMouseOver()
    {
        if (Type == EventType.MouseOver)
        {
            fmod.PlayOneShotAudio(EventName);
        }
    }

}
