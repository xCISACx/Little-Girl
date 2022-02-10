using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMovement : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerGlobalVariables.instance.CanMove = !PlayerGlobalVariables.instance.CanMove;   
        }
    }
}
