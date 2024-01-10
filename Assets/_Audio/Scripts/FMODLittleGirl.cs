using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class FMODLittleGirl : MonoBehaviour
{
    private Dictionary<string, FMOD.Studio.EventInstance> fmodEvents;

    [FMODUnity.EventRef]
    public string Menu;
    FMOD.Studio.EventInstance MenuEvent;
    [FMODUnity.EventRef]
    public string Puzzle;

    public List<GameObject> timelines;
    
    FMOD.Studio.EventInstance PuzzleEvent;

    FMOD.Studio.EventInstance currentSoundtrack;
    string currentAudioString;

    public string CurrentSnapshotPath = "";
    private FMOD.Studio.EventInstance _currentSnapshotInstance;

    //The floor material types
    private string[] _floorTypes = new string[]
                                                {
                                                    "Earth",
                                                    "Stone",
                                                    "Grass",
                                                    "Puddle",
                                                    "Wood"
                                                };

    //The walking sound effect event
    private StudioEventEmitter _walkingEvent;

    // Start is called before the first frame update
    void Start()
    {
        fmodEvents = new Dictionary<string, FMOD.Studio.EventInstance>();
        _walkingEvent = GameObject.FindGameObjectWithTag("Player")?.GetComponent<StudioEventEmitter>();
    }

    public void PlayAudio(string eventName)
    {
        if (!fmodEvents.ContainsKey(eventName))
        {
            fmodEvents[eventName] = FMODUnity.RuntimeManager.CreateInstance(eventName);
        }

        PlayAudio(fmodEvents[eventName], eventName);
    }

    public FMOD.Studio.EventInstance GetInstanceByEventName(string eventName)
    {
        if (!fmodEvents.ContainsKey(eventName))
        {
            fmodEvents[eventName] = FMODUnity.RuntimeManager.CreateInstance(eventName);
        }
        return fmodEvents[eventName];
    }

    public void PlayOneShotAudio(string eventName)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventName);
    }

    public void PlayAudio(FMOD.Studio.EventInstance instance, string eventName)
    {
        var playbackState = PlaybackState(instance);

        switch (playbackState)
        {
            case FMOD.Studio.PLAYBACK_STATE.STOPPED:
                instance.start();
                instance.release();
                break;
            case FMOD.Studio.PLAYBACK_STATE.STOPPING:
                instance.clearHandle();
                instance.stop(0);
                break;
        }
    }

    public void CreateAndPlayAudio(string eventName)
    {
        FMOD.Studio.EventInstance instance;
        instance = FMODUnity.RuntimeManager.CreateInstance(eventName);

        if (PlaybackState(instance) == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            // UnityEngine.Debug.Log("Playing..." + eventName);
        }
        if (PlaybackState(instance) == FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            instance.start();
            instance.release();
        }
        if (PlaybackState(instance) == FMOD.Studio.PLAYBACK_STATE.STOPPING)
        {
            instance.clearHandle();
            instance.stop(0);
        }
    }

    public FMOD.Studio.PLAYBACK_STATE PlaybackState(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE pS;
        instance.getPlaybackState(out pS);
        return pS;
    }

    public void StopAllBusEvents(string busName)  //todo:  StopAllEvents("bus:/Soundtrack");
    {
        FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus(busName);
        bus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void SetParameter(string eventName, string parameterName, float value)
    {
        var instance = GetInstanceByEventName(eventName);
        instance.setParameterByName(parameterName, value);
    }

    public void ChangeSnapshot(string snapshotPath)
    {
        //Check if there is any snapshot active
        if (CurrentSnapshotPath != "")
        {
            //Stop the current snapshot
            _currentSnapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _currentSnapshotInstance.release();
        }

        //Save the current snapshot's path
        CurrentSnapshotPath = snapshotPath;

        //Create an instance of the new snapshot
        _currentSnapshotInstance  = RuntimeManager.CreateInstance(snapshotPath);

        //Start the instance
        _currentSnapshotInstance.start();
    }

    public void ChangeFloorType(string floorType)
    {
        //Get the index of the material
        int floorTypeIndex = System.Array.IndexOf(_floorTypes, floorType);

        //Set the parameter
        _walkingEvent.Params[0].Value = floorTypeIndex;
    }
}