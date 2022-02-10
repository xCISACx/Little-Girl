using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class HighlightBehaviour : MonoBehaviour
{
    public Stopwatch Timer;
    public ParticleSystem ParticleSystem;
    public Vector3 offset;
    public bool PickedUp;
    public bool CanHighlight;
    public double elapsedSeconds;
    public float TimeToWait;

    // Start is called before the first frame update
    void Start()
    {
        Timer = new Stopwatch();
        transform.position = transform.position + offset;
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
        //TODO: START TIMER WHEN PLAYER ENTERS ROOM.
        Timer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PickedUp)
        {
            if (Timer.Elapsed.TotalSeconds > 5)
            {
                CanHighlight = true;
            }

            if (CanHighlight)
            {
                CanHighlight = false;
                ParticleSystem.Play();
                //TODO: TRY TO RE-IMPLEMENT COROUTINE LATER 
                //StartCoroutine(WaitForParticleSystem());

                if (Timer.Elapsed.Seconds > TimeToWait + ParticleSystem.main.duration)
                {
                    Timer.Restart();
                }
            }
        }
        
        elapsedSeconds = Timer.Elapsed.TotalSeconds;
        Debug.Log(Timer.Elapsed.Seconds);
    }

    IEnumerator WaitForParticleSystem()
    {
        yield return new WaitForSeconds(ParticleSystem.main.duration);
        Timer.Restart();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + offset, 0.2f);
    }
#endif
}