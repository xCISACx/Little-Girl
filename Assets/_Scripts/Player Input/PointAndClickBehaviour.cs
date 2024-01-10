using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PointAndClickBehaviour : MonoBehaviour
{
    public static PointAndClickBehaviour instance;
    public Vector3 mousePos;
    public Vector3 destinationPos;
    public bool moving;
    public LayerMask MovementMask;
    public LayerMask NoMovementMask;

    public NavMeshAgent _navMeshAgent;
    private Animator anim;

    public bool CanPlayWalkingAnimation = true;

    private StudioEventEmitter _footstepEvent;

    // Start is called before the first frame update
    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        
        _navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        _footstepEvent = GetComponent<StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            Ray ray = PlayerGlobalVariables.instance.CurrentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.cyan);

            if (PlayerGlobalVariables.instance.CanMove)
            {
                if (!GameManager.instance.currentInteractable)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, MovementMask))
                    {
                        if (NoMovementMask.Includes(hit.collider.gameObject.layer))
                        {
                            return;
                        }
                        moving = true;

                        //Start playing the footstep event
                        _footstepEvent.Play();

                        destinationPos = hit.point;
                    }
                }
            }
        }

        //WORKING
        //_navMeshAgent.SetDestination(destinationPos);

        //TEST - Path check
        NavMeshPath path = new NavMeshPath();
        _navMeshAgent.CalculatePath(destinationPos, path);
        if(path.status == NavMeshPathStatus.PathComplete)
        {
            if (PlayerGlobalVariables.instance.CanMove)
            {
                _navMeshAgent.SetDestination(destinationPos);   
            }
        }

        if (CanPlayWalkingAnimation)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                moving = false;

                //Stop playing the footstep event
                _footstepEvent.Stop();
            }
            else
            {
                moving = true;

                //Start playing the footstep event
                _footstepEvent.Play();
            }
            
            anim.SetBool("moving", moving);
        }
        else
        {
            DisableWalkingAnimation();
        }
    }

    private void DisableWalkingAnimation()
    {
        //Makes sure the animation stops if the component is disabled
        moving = false;
        anim.SetBool("moving", moving);
    }
}
